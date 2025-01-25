namespace MarcoZechner.ConsoleBox;

public abstract class PanelBase : IRenderable {
    private float relativeSize = 1;
    public float RelativeSize {
        get => relativeSize;
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Relative size must be greater than 0.");
            }
            relativeSize = value;
        }
    }

    public abstract void Render(int top, int left, int width, int height);
}