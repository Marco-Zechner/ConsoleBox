namespace MarcoZechner.ConsoleBox;

internal class SeperatorLine : PanelBase
{
    private char seperatorChar = '+';
    public char SeperatorChar
    {
        get => seperatorChar;
        set => seperatorChar = value;
    }
    public SeperatorLine() {
        FixedSize = 1;
    }

    public SeperatorLine(char seperatorChar) {
        this.seperatorChar = seperatorChar;
        FixedSize = 1;
    }

    public override void Render(int top, int left, int width, int height, RenderBuffer buffer)
    {
        for (int y = top; y < top + height; y++)
        {
            buffer.Write(left, y, new string(seperatorChar, width));
        }
    }
}