using Microsoft.Xna.Framework;

namespace SystemX.Camera2D
{
    public interface I_2dCamera
    {
        Vector2 Position { get; }
        Vector2 BestPosition { set; }
        Vector2 DisplaySize { get; set;}
        Rectangle CameraBounds { get; set;}
    }
}