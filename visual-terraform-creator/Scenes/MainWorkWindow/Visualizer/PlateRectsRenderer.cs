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
		_drawableNode.DrawRect(plateTypeARect, rectColor, filled: false);

		Rect2 plateTypeBRect = new Rect2(new Vector2(0, 0), new Vector2(100, rectBHeight));
		_drawableNode.DrawRect(plateTypeBRect, rectColor, filled: false);

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
