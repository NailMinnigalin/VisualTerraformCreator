using Godot;
using VisualTerraformCreator.Abstract;

public partial class ControlPoint : Control
{
	public enum AllowedMovingAxisEnum
	{
		X,
		Y
	}

	private static string _controlPointScenePath = "res://Scenes/ControlPoint/ControlPoint.tscn";

	private AllowedMovingAxisEnum AllowedMovingAxis;
	private float MaxPosition;
	private float MinPosition;

	private bool _isMouseInside = false;
	private bool _isMovingControl = false;
	private bool _isDragControlPoint = false;

	private IZoomProvider _zoomProvider = default!;

	public Vector2 CenterPosition 
	{ 
		get => Position + CenterShift;
		set => Position = value - CenterShift;
	}

	public static ControlPoint Instantiate(bool isVisible, Vector2 position, Node parent, Vector2 scale)
	{
		PackedScene packedScene = GD.Load<PackedScene>(_controlPointScenePath);
		ControlPoint controlPoint = packedScene.Instantiate<ControlPoint>();
		parent.AddChild(controlPoint);
		controlPoint.Visible = isVisible;
		controlPoint.Scale = scale;
		controlPoint.CenterPosition = position;

		return controlPoint;
	}

	public void InitActualControlPoint(AllowedMovingAxisEnum allowedMovingAxis, float positionRangeStart, float positionRangeEnd, IZoomProvider zoomProvider)
	{
		AllowedMovingAxis = allowedMovingAxis;
		MaxPosition = positionRangeStart > positionRangeEnd ? positionRangeStart : positionRangeEnd;
		MinPosition = positionRangeStart > positionRangeEnd ? positionRangeEnd : positionRangeStart;

		MouseEntered += HandleMouseEntered;
		MouseExited += HandleMouseExited;

		_zoomProvider = zoomProvider;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("DragControlPoint"))
		{
			_isDragControlPoint = true;
		}
		else if (@event.IsActionReleased("DragControlPoint"))
		{
			_isDragControlPoint = false;
		}

		if (_isMouseInside && _isDragControlPoint)
		{
			_isMovingControl = true;
		}
		else if (_isMovingControl && !_isDragControlPoint)
		{
			_isMovingControl = false;
		}

		if (_isMovingControl && @event is InputEventMouseMotion inputEventMouseMotion)
		{
			MoveControlPoint(inputEventMouseMotion);
		}
	}

	private void MoveControlPoint(InputEventMouseMotion inputEventMouseMotion)
	{
		var motion = inputEventMouseMotion.Relative / _zoomProvider.CurrentZoom;
		motion = ClearForbiddenMotion(motion);
		var newCenterPosition = CenterPosition + motion;
		newCenterPosition = RestrictAllowedMotion(newCenterPosition);

		CenterPosition = newCenterPosition;
	}

	private Vector2 RestrictAllowedMotion(Vector2 newCenterPosition)
	{
		if (AllowedMovingAxis == AllowedMovingAxisEnum.X)
		{
			if (newCenterPosition.X > MaxPosition)
				newCenterPosition.X = MaxPosition;
			else if (newCenterPosition.X < MinPosition)
				newCenterPosition.X = MinPosition;
		}
		if (AllowedMovingAxis == AllowedMovingAxisEnum.Y)
		{
			if (newCenterPosition.Y > MaxPosition)
				newCenterPosition.Y = MaxPosition;
			else if (newCenterPosition.Y < MinPosition)
				newCenterPosition.Y = MinPosition;
		}

		return newCenterPosition;
	}

	private Vector2 ClearForbiddenMotion(Vector2 motion)
	{
		if (AllowedMovingAxis == AllowedMovingAxisEnum.X)
		{
			motion.Y = 0;
		}
		else if (AllowedMovingAxis == AllowedMovingAxisEnum.Y)
		{
			motion.X = 0;
		}

		return motion;
	}

	private void HandleMouseExited()
	{
		_isMouseInside = false;
	}

	private void HandleMouseEntered()
	{
		_isMouseInside = true;
	}

	private Vector2 CenterShift => (Size * Scale / 2);
}
