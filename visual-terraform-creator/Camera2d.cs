using Godot;

public partial class Camera2d : Camera2D
{
	[Export]
	public float ZoomSpeed = 0.1f;

	private bool _isPanning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		HandleCameraZoom(@event);
		HandleCameraPanning(@event);
	}

	private void HandleCameraPanning(InputEvent @event)
	{
		if (@event.IsActionPressed("CameraPanningOn"))
		{
			_isPanning = true;
		}
		if (@event.IsActionReleased("CameraPanningOn"))
		{
			_isPanning = false;
		}
		if (_isPanning && @event is InputEventMouseMotion mouseMotion)
		{
			Position -= (mouseMotion.Relative / Zoom);
		}
	}

	private void HandleCameraZoom(InputEvent @event)
	{
		if (@event.IsAction("CameraZoomIn"))
		{
			Zoom = new Vector2(Zoom.X + ZoomSpeed, Zoom.Y + ZoomSpeed);
		}
		if (@event.IsAction("CameraZoomOut"))
		{
			var newZoom = new Vector2(Zoom.X - ZoomSpeed, Zoom.Y - ZoomSpeed);
			if (newZoom.X > 0 && newZoom.Y > 0)
			{
				Zoom = newZoom;
			}
		}
	}
}
