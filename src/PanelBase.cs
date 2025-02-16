namespace MarcoZechner.ConsoleBox;

public abstract class PanelBase : IRenderable {
    protected float relativeSize = 1;
    public float RelativeSize {
        get => relativeSize;
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Relative size must be greater than 0.");
            }
            relativeSize = value;
            fixedSize = 0;
        }
    }
    protected int fixedSize = 0;
    public int FixedSize {
        get {
            if (fixedSize > 0) {
                relativeSize = 0;
            }
            return fixedSize;
        }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Min size must be greater than 0.");
            }
            fixedSize = value;
            relativeSize = 0;
        }
    }


    public string PanelName { init; get; } = "";

    public abstract void Render(int top, int left, int width, int height, RenderBuffer buffer);
}