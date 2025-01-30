namespace MarcoZechner.ConsoleBox;

public class BoxPane : PanelBase {
    public bool DoubleLines { get; set; } = false;
    public string Title { get; set; } = "";
    public float TitleAlignment { get; set; } = 0.5f;
    public PanelBase? Content { get; set; } = null;

    public override void Render(int top, int left, int width, int height, RenderBuffer buffer)
    {
        if (Content == null)
        {
            return;
        }

        if (width < 3 || height < 3)
        {
            return;
        }

        int titleWidth = Math.Min(Title.Length, width-2);
        int titleLeft = left + (int)(width * TitleAlignment) - titleWidth / 2;
        titleLeft = Math.Max(1, Math.Min(titleLeft, left + width - titleWidth));

        RenderEdge(top, left, width, height, buffer, DoubleLines, Title[..titleWidth], titleLeft - left);

        Content.Render(top+1, left+1, width-2, height-2, buffer);
    }

    private static void RenderEdge(int top, int left, int width, int height, RenderBuffer buffer, bool doubleLines = false, string title = "", int leftOffset = 0)
    {
        buffer.Write(left, top, doubleLines ? "╔" : "┌");
        buffer.Write(left + width - 1, top, doubleLines ? "╗" : "┐");
        buffer.Write(left, top + height - 1, doubleLines ? "╚" : "└");
        buffer.Write(left + width - 1, top + height - 1, doubleLines ? "╝" : "┘");

        string topText = title.PadLeft(leftOffset - 1 + title.Length, doubleLines ? '═' : '─').PadRight(width - 2, doubleLines ? '═' : '─');
        buffer.Write(left + 1, top, topText);
        buffer.Write(left + 1, top + height - 1, new string(doubleLines ? '═' : '─', width - 2));

        for (int y = top + 1; y < top + height - 1; y++)
        {
            buffer.Write(left, y, doubleLines ? "║" : "│");
            buffer.Write(left + width - 1, y, doubleLines ? "║" : "│");
        }
    }
}