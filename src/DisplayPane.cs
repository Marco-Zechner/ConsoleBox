namespace MarcoZechner.ConsoleBox;

public sealed class DisplayPane : PanelBase
{
    private string content = "";
    public string Content
    {
        get => content;
        set => content = value;
    }

    public override void Render(int top, int left, int width, int height, RenderBuffer buffer)
    {
        if (width < 1 || height < 1)
        {
            return;
        }

        List<string> lines = [.. content.Split('\n', height)];
        for (int i = 0; i < lines.Count && lines.Count <= height; i++) {
            if (lines[i].Length > width) {
                lines.Insert(i + 1, lines[i][width..]);
                lines[i] = lines[i][..width];
            }
            lines[i] = lines[i].PadRight(width);
        }
        for (int i = 0; i < height; i++)
        {
            string line = i < lines.Count ? lines[i] : new string(' ', width);
            buffer.Write(left, top + i, line);
        }
    }
}