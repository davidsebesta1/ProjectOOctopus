
namespace ProjectOOctopus.Events
{
    public class ColorChangedEvent : EventArgs
    {
        public Color Color;

        public ColorChangedEvent(Color color)
        {
            Color = color;
        }
    }
}
