namespace MarcoZechner.ConsoleBox;

internal class SeperatorLine : PanelBase
{
    private readonly char? seperatorChar = null;
    public char? SeperatorChar
    {
        get => seperatorChar;
    }
    public SeperatorLine() {
        RelativeSize = 0;
    }

    public SeperatorLine(char? seperatorChar) {
        this.seperatorChar = seperatorChar;
    }

    public override void Render(int top, int left, int width, int height, ConsoleBuffer? buffer = null)
    {
        throw new NotImplementedException();
    }
}