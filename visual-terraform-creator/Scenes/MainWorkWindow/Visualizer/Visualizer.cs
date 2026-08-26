using Godot;
using VisualTerraformCreator.Abstract;
using VTСLib;

namespace VisualTerraformCreator.Scenes.MainWorkWindow.Visualizer;

public partial class Visualizer : Node2D
{
	private Rect2 _plateTypeARect;
	private Rect2 _plateTypeBRect;
	private PlateRectsRenderer? _plateRectsRenderer = default!;
	private ControlPointController _controlPointPlacementController = default!;
	private ControlPoint? _topControlPoint;
	private ControlPoint? _sideControlPoint;

	public void Initialize(IZoomProvider zoomProvider, ITectonicPlateType plateTypeA, ITectonicPlateType plateTypeB, PlateBoundaryType plateBoundaryType)
	{
		_plateRectsRenderer = new(plateTypeA, plateTypeB, plateBoundaryType, this);
		_controlPointPlacementController = new(this, zoomProvider);
	}

	public override void _Ready()
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (_plateRectsRenderer == null) return;

		(_plateTypeARect, _plateTypeBRect) = _plateRectsRenderer.Draw();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			_controlPointPlacementController.ShowControlPointShadow(_plateTypeARect, _plateTypeBRect);
		}
		else if (@event.IsActionPressed("AddControlPoint"))
		{
			var controlPoints = _controlPointPlacementController.AddControlPoint();
			if (_plateRectsRenderer != null && controlPoints.topControlPoint != null && controlPoints.sideControlPoint != null)
			{
				_topControlPoint = controlPoints.topControlPoint;
				_sideControlPoint = controlPoints.sideControlPoint;
				_plateRectsRenderer.SetControlPointsOnPlate(_topControlPoint, _sideControlPoint);
				_topControlPoint.OnMoveAction += (ControlPoint controlPoint) => _plateRectsRenderer.SetControlPointsOnPlate(_topControlPoint, _sideControlPoint);
				_sideControlPoint.OnMoveAction += (ControlPoint controlPoint) => _plateRectsRenderer.SetControlPointsOnPlate(_topControlPoint, _sideControlPoint);
				GD.Print($"Top control point:{_topControlPoint.CenterPosition}");
				GD.Print($"Side control point:{_sideControlPoint.CenterPosition}");
			}
		}
	}

	public void SetPlateTypeA(ITectonicPlateType plateType) => _plateRectsRenderer?.SetPlateTypeA(plateType);

	public void SetPlateTypeB(ITectonicPlateType plateType) => _plateRectsRenderer?.SetPlateTypeB(plateType);

	public void SetPlateBoundaryType(PlateBoundaryType plateBoundaryType) => _plateRectsRenderer?.SetPlateBoundaryType(plateBoundaryType);
}