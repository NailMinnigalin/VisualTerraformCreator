using Godot;

namespace VisualTerraformCreator.Abstract
{
	public interface IZoomProvider
	{
		public Vector2 CurrentZoom { get; }
	}
}
