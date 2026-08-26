using Godot;
using System;
using VTСLib;

namespace VisualTerraformCreator.Scenes.MainWorkWindow.Visualizer;

/// <summary>
/// Renders plate rects for <see cref="Visualizer.Visualizer"/>
/// </summary>
public class PlateRectsRenderer
{
	/// <summary>
	/// Class that stores data about control points and plate they are sitting on;
	/// </summary>
	public class ControlPointsOnPlate
	{
		/// <summary>
		/// Control point on top of a plate that defines terrain changes
		/// </summary>
		public ControlPoint TopControlPoint;
		/// <summary>
		/// Control point on side of a plate that defines terrain changes
		/// </summary>
		public ControlPoint SideControlPoint;
		/// <summary>
		/// Plate on witch control points are sitting.
		/// </summary>
		public ITectonicPlateType ControlPointPlate;
	}

	/// <summary>
	/// Left plate to draw
	/// </summary>
	private ITectonicPlateType _plateTypeA;
	/// <summary>
	/// Right plate to draw
	/// </summary>
	private ITectonicPlateType _plateTypeB;
	/// <summary>
	/// Boundry type to draw
	/// </summary>
	private PlateBoundaryType _plateBoundaryType;
	/// <summary>
	/// Node2d to draw
	/// </summary>
	private readonly Node2D _drawableNode;
	private ControlPointsOnPlate? _controlPointsOnPlate;

	public PlateRectsRenderer(ITectonicPlateType plateTypeA, ITectonicPlateType plateTypeB, PlateBoundaryType plateBoundaryType, Node2D drawableNode)
	{
		_plateTypeA = plateTypeA;
		_plateTypeB = plateTypeB;
		_plateBoundaryType = plateBoundaryType;
		_drawableNode = drawableNode;
	}

	/// <summary>
	/// Dwar plates. Expected to be called in _Draw()
	/// </summary>
	/// <returns>Rects that represend two drawed plates</returns>
	public (Rect2 plateTypeARect, Rect2 plateTypeBRect) Draw()
	{
		Color rectColor = new Color(1, 1, 1);
		(int rectAHeight, int rectBHeight) = GetRectsHeight(_plateTypeA, _plateTypeB, -100);

		Rect2 plateTypeARect = new Rect2(new Vector2(0, 0), new Vector2(-100, rectAHeight));
		DrawRect(plateTypeARect, _plateTypeA, _controlPointsOnPlate);

		Rect2 plateTypeBRect = new Rect2(new Vector2(0, 0), new Vector2(100, rectBHeight));
		DrawRect(plateTypeBRect, _plateTypeB, _controlPointsOnPlate);

		DrawBoundry(rectAHeight, rectBHeight);

		return (plateTypeARect, plateTypeBRect);
	}

	public void SetPlateTypeA(ITectonicPlateType plateType)
	{
		_plateTypeA = plateType;
		_drawableNode.QueueRedraw();
	}

	public void SetPlateTypeB(ITectonicPlateType plateType)
	{
		_plateTypeB = plateType;
		_drawableNode.QueueRedraw();
	}

	public void SetPlateBoundaryType(PlateBoundaryType plateBoundaryType)
	{
		_plateBoundaryType = plateBoundaryType;
		_drawableNode.QueueRedraw();
	}

	public void SetControlPointsOnPlate(ControlPoint topControlPoint, ControlPoint sideControlPoint)
	{
		_controlPointsOnPlate = new()
		{
			TopControlPoint = topControlPoint,
			SideControlPoint = sideControlPoint,
			ControlPointPlate = sideControlPoint.Position.X < topControlPoint.Position.Y ? _plateTypeB : _plateTypeA
		};
		_drawableNode.QueueRedraw();
	}

	private void DrawRect(Rect2 rect, ITectonicPlateType drawingPlate, ControlPointsOnPlate? controlPointsOnPlate)
	{
		Color rectColor = new Color(1, 1, 1);
		if (controlPointsOnPlate == null || drawingPlate != controlPointsOnPlate.ControlPointPlate)
		{
			_drawableNode.DrawRect(rect, rectColor, filled: false);
			return;
		}

		Vector2 bottomLineFrom = rect.Position;
		Vector2 bottomLineTo = new(bottomLineFrom.X + rect.Size.X, bottomLineFrom.Y);
		_drawableNode.DrawLine(bottomLineFrom, bottomLineTo, rectColor);
		if (controlPointsOnPlate.SideControlPoint.Position.X > controlPointsOnPlate.TopControlPoint.Position.X)
		{
			Vector2 rightLineFrom = new(rect.Position.X + rect.Size.X, rect.Position.Y);
			Vector2 rightLineTo = rect.Position + rect.Size;
			_drawableNode.DrawLine(rightLineFrom, rightLineTo, rectColor);

			Vector2 leftLineFrom = rect.Position;
			Vector2 leftLineTo = new(rect.Position.X, rect.Position.Y + controlPointsOnPlate.SideControlPoint.CenterPosition.Y);
			_drawableNode.DrawLine(leftLineFrom, leftLineTo, rectColor);

			Vector2 topLineFrom = controlPointsOnPlate.TopControlPoint.CenterPosition;
			Vector2 topLineTo = rect.Position + rect.Size;
			_drawableNode.DrawLine(topLineFrom, topLineTo, rectColor);
		}
		else
		{
			Vector2 leftLineFrom = rect.Position;
			Vector2 leftLineTo = new(rect.Position.X, rect.Position.Y + rect.Position.Y);
			_drawableNode.DrawLine(leftLineFrom, leftLineTo, rectColor);

			Vector2 rightLineFrom = new(rect.Position.X + rect.Size.X, rect.Position.Y);
			Vector2 rightLineTo = new(rect.Position.X + rect.Size.X, rect.Position.Y + controlPointsOnPlate.SideControlPoint.CenterPosition.Y);
			_drawableNode.DrawLine(rightLineFrom, rightLineTo, rectColor);

			Vector2 topLineFrom = new(rect.Position.X, rect.Position.Y + rect.Size.Y);
			Vector2 topLineTo = controlPointsOnPlate.TopControlPoint.CenterPosition;
			_drawableNode.DrawLine(topLineFrom, topLineTo, rectColor);
		}

		_drawableNode.DrawLine(controlPointsOnPlate.TopControlPoint.CenterPosition, controlPointsOnPlate.SideControlPoint.CenterPosition, rectColor);
	}

	private void DrawBoundry(int rectAHeight, int rectBHeight)
	{
		int boundryMax = Math.Max(rectAHeight, rectBHeight);
		Color color = GetBoundryColor(_plateBoundaryType);
		_drawableNode.DrawLine(new Vector2(0, boundryMax), Vector2.Zero, color);
	}

	private Color GetBoundryColor(PlateBoundaryType plateBoundaryType)
	{
		return plateBoundaryType switch
		{
			PlateBoundaryType.Convergent => new Color(1, 0, 0),
			PlateBoundaryType.Divergent => new Color(0, 0, 1),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private (int rectAHeight, int rectBHeight) GetRectsHeight(ITectonicPlateType plateTypeA, ITectonicPlateType plateTypeB, int baseHeight)
	{
		if (plateTypeA.MinHeight > plateTypeB.MaxHeight)
		{
			return (baseHeight, baseHeight / 2);
		}
		if (plateTypeB.MinHeight > plateTypeA.MaxHeight)
		{
			return (baseHeight / 2, baseHeight);
		}

		return (baseHeight, baseHeight);
	}
}
