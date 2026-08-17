using Godot;
using System;
using VTСLib;

public partial class Visualizer : Node2D
{
	private ITectonicPlateType _plateTypeA;
	private ITectonicPlateType _plateTypeB;
	private PlateBoundaryType _plateBoundaryType;

	public override void _Ready()
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (_plateTypeA == null || _plateTypeB == null) return;

		Color rectColor = new Color(1, 1, 1);
		(int rectAHeight, int rectBHeight) = GetRectsHeight(_plateTypeA, _plateTypeB, -100);

		Rect2 plateTypeARect = new Rect2(new Vector2(0, 0), new Vector2(-100, rectAHeight));
		DrawRect(plateTypeARect, rectColor, filled: false);

		Rect2 plateTypeBRect = new Rect2(new Vector2(0, 0), new Vector2(100, rectBHeight));
		DrawRect(plateTypeBRect, rectColor, filled: false);

		DrawBoundry(rectAHeight, rectBHeight);
	}

	public void SetPlateTypeA(ITectonicPlateType plateType)
	{
		_plateTypeA = plateType;
		QueueRedraw();
	}

	public void SetPlateTypeB(ITectonicPlateType plateType)
	{
		_plateTypeB = plateType;
		QueueRedraw();
	}

	public void SetPlateBoundaryType(PlateBoundaryType plateBoundaryType)
	{
		_plateBoundaryType = plateBoundaryType;
		QueueRedraw();
	}

	private void DrawBoundry(int rectAHeight, int rectBHeight)
	{
		int boundryMax = Math.Max(rectAHeight, rectBHeight);
		Color color = GetBoundryColor(_plateBoundaryType);
		DrawLine(new Vector2(0, boundryMax), Vector2.Zero, color);
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
			return(baseHeight, baseHeight / 2);
		}
		if (plateTypeB.MinHeight > plateTypeA.MaxHeight)
		{
			return (baseHeight / 2, baseHeight);
		}

		return (baseHeight, baseHeight);
	}
}