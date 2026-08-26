using Godot;
using System;
using VisualTerraformCreator.Abstract;

namespace VisualTerraformCreator.Scenes.MainWorkWindow.Visualizer
{
	public class ControlPointController
	{
		/// <summary>
		/// Class for storing data abot sticked control shadow
		/// </summary>
		private class StickedControlPointShadow
		{
			/// <summary>
			/// Shadow's sticked point
			/// </summary>
			public Vector2 StickedPoint;
			/// <summary>
			/// Shadow's sticked rect
			/// </summary>
			public Rect2 Rect;
		}

		private readonly Node2D _mainNode;
		private readonly ControlPoint _controlPointShadow;
		private readonly IZoomProvider _zoomProvider;

		private StickedControlPointShadow? _stickedControlPointShadow = null;

		private bool _isControlPointsSet = false;

		private const float _controlPointScale = 0.1f;
		private const float _nearPositionThreshold = 20.0f;

		public ControlPointController(Node2D mainNode, IZoomProvider zoomProvider)
		{
			_mainNode = mainNode;
			_controlPointShadow = ControlPoint.Instantiate(false, new(0, 0), mainNode, new(_controlPointScale, _controlPointScale));
			_zoomProvider = zoomProvider;
		} 

		public void ShowControlPointShadow(Rect2 plateTypeARect, Rect2 plateTypeBRect)
		{
			if (_isControlPointsSet) return;

			Vector2 localMousePosition = _mainNode.GetLocalMousePosition();
			bool isNearTopSideOfRectA = IsPositionNearTopSideOfRect(localMousePosition, plateTypeARect, _nearPositionThreshold, out Vector2 RectAPointP);
			bool isNearTopSideOfRectB = IsPositionNearTopSideOfRect(localMousePosition, plateTypeBRect, _nearPositionThreshold, out Vector2 RectBPointP);

			if (isNearTopSideOfRectA)
			{
				ShowShadowOnPointP(RectAPointP, plateTypeARect);
			}
			else if (isNearTopSideOfRectB)
			{
				ShowShadowOnPointP(RectBPointP, plateTypeBRect);
			}
			else
			{
				_controlPointShadow.Visible = false;
				_stickedControlPointShadow = null;
			}
		}

		public void AddControlPoint()
		{
			if (_isControlPointsSet) return;
			if (_stickedControlPointShadow == null) return;

			_controlPointShadow.Visible = false;
			_isControlPointsSet = true;
			ControlPoint controlPoint = ControlPoint.Instantiate(true, _stickedControlPointShadow.StickedPoint, _mainNode, new(_controlPointScale, _controlPointScale));
			controlPoint.InitActualControlPoint(ControlPoint.AllowedMovingAxisEnum.X, _stickedControlPointShadow.Rect.Position.X, _stickedControlPointShadow.Rect.End.X, _zoomProvider);

			controlPoint = ControlPoint.Instantiate(true, _stickedControlPointShadow.Rect.Position + new Vector2(0, _stickedControlPointShadow.Rect.Size.Y), _mainNode, new(_controlPointScale, _controlPointScale));
			controlPoint.InitActualControlPoint(ControlPoint.AllowedMovingAxisEnum.Y, _stickedControlPointShadow.Rect.Position.Y, _stickedControlPointShadow.Rect.End.Y, _zoomProvider);
		}

		private void ShowShadowOnPointP(Vector2 RectPointP, Rect2 rect)
		{
			_controlPointShadow.CenterPosition = RectPointP;
			_controlPointShadow.Visible = true;

			_stickedControlPointShadow ??= new();
			_stickedControlPointShadow.StickedPoint = RectPointP;
			_stickedControlPointShadow.Rect = rect;
		}

		private bool IsPositionNearTopSideOfRect(Vector2 localMousePosition, Rect2 rect, float nearPositionThreshold, out Vector2 pointP)
		{
			Vector2 pointA = new(rect.Position.X, rect.Position.Y + rect.Size.Y);
			Vector2 pointB = new(rect.Position.X + rect.Size.X, rect.Position.Y + rect.Size.Y);

			float lengthMP = CalculateDistanceBetweenMousePositionAndLineAB(localMousePosition, pointA, pointB, out pointP);
			return lengthMP <= nearPositionThreshold;
		}

		/// <summary>
		/// Calculate distance between mouse position and line AB
		/// </summary>
		/// <param name="localMousePosition">Mouse position local to line's view</param>
		/// <param name="pointA">Line's point A</param>
		/// <param name="pointB">Line's point B</param>
		/// <param name="pointP">Nearest point on line AB to <paramref name="localMousePosition"/></param>
		/// <returns>Distance to nearest point on line AB</returns>
		private static float CalculateDistanceBetweenMousePositionAndLineAB(Vector2 localMousePosition, Vector2 pointA, Vector2 pointB, out Vector2 pointP)
		{
			Vector2 vecAB = pointB - pointA;
			Vector2 vecAM = localMousePosition - pointA;

			var t = (vecAB.Dot(vecAM)) / (Math.Pow(vecAB.Length(), 2));
			t = Math.Clamp(t, 0, 1);

			var xp = pointA.X + t * (pointB.X - pointA.X);
			var yp = pointA.Y + t * (pointB.Y - pointA.Y);
			pointP = new((float)xp, (float)yp);
			Vector2 vecMP = pointP - localMousePosition;

			var lengthMP = vecMP.Length();
			return lengthMP;
		}
	}
}
