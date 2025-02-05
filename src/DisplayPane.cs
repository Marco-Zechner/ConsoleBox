namespace MarcoZechner.ConsoleBox;

public sealed class DisplayPane : PanelBase
{
    private string content = "";
    public string Content
    {
        get => content;
        set => content = value;
    }

    private bool truncate = false;
    public bool Truncate
    {
        get => truncate;
        set => truncate = value;
    }

    public int ContentWidth => content.Split('\n').Max(x => x.Length);

    private int horizontalOffset = 0;
    public int HorizontalOffset
    {
        get => horizontalOffset;
        set => horizontalOffset = value;
    }

    public override void Render(int top, int left, int width, int height, RenderBuffer buffer)
    {
        if (width < 1 || height < 1)
        {
            return;
        }

        content = content.Replace("\t", "    ").Replace("\r\n", "\n").Replace("\r", "\n");
        horizontalOffset = Math.Max(0, Math.Min(horizontalOffset, ContentWidth - width));

        List<string> lines = [.. content.Split('\n', height)];
        for (int i = 0; i < lines.Count && lines.Count <= height; i++) {
            if (!truncate) {
                if (lines[i].Length > width) {
                    lines.Insert(i + 1, lines[i][width..]);
                    lines[i] = lines[i][..width];
                }
                lines[i] = lines[i].PadRight(width);
                continue;
            }

            lines[i] = lines[i].PadRight(Math.Max(ContentWidth, width))[horizontalOffset..(width + horizontalOffset)];
        }
        for (int i = 0; i < height; i++)
        {
            string line = i < lines.Count ? lines[i] : new string(' ', width);
            buffer.Write(left, top + i, line);
        }
    }
}