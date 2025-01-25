namespace MarcoZechner.ConsoleBox;

public class BoxPane : PanelBase {
    public bool DoubleLines { get; set; } = false;
    public string Title { get; set; } = "";
    public float TitleAlignment { get; set; } = 0.5f;
    public PanelBase? Content { get; set; } = null;

    public override void Render(int top, int left, int width, int height)
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

        RenderEdge(top, left, width, height, DoubleLines, Title[..titleWidth], titleLeft - left);

        Content.Render(top+1, left+1, width-2, height-2);
    }

    private static void RenderEdge(int top, int left, int width, int height, bool doubleLines = false, string title = "", int leftOffset = 0)
    {
        Console.SetCursorPosition(left, top);
        Console.Write(doubleLines ? "╔" : "┌");
        Console.SetCursorPosition(left + width - 1, top);
        Console.Write(doubleLines ? "╗" : "┐");
        Console.SetCursorPosition(left, top + height - 1);
        Console.Write(doubleLines ? "╚" : "└");
        Console.SetCursorPosition(left + width - 1, top + height - 1);
        Console.Write(doubleLines ? "╝" : "┘");

        Console.SetCursorPosition(left + 1, top);
        string topText = title.PadLeft(leftOffset - 1 + title.Length, doubleLines ? '═' : '─').PadRight(width - 2, doubleLines ? '═' : '─');
        Console.Write(topText);
        Console.SetCursorPosition(left + 1, top + height - 1);
        Console.Write(new string(doubleLines ? '═' : '─', width - 2));

        for (int y = top + 1; y < top + height - 1; y++)
        {
            Console.SetCursorPosition(left, y);
            Console.Write(doubleLines ? "║" : "│");
            Console.SetCursorPosition(left + width - 1, y);
            Console.Write(doubleLines ? "║" : "│");
        }
    }
}