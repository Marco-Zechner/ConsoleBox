namespace MarcoZechner.ConsoleBox;

public class FloatingPane : IRenderable {
    private float leftEdgePercent = 0.25f;
    public float LeftEdgePercent
    {
        get => leftEdgePercent;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1.");
            }
            if (value + WidthPercent > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value + WidthPercent must be less than or equal to 1.");
            }
            leftEdgePercent = value;
        }
    }
    private float topEdgePercent = 0.25f;
    public float TopEdgePercent
    {
        get => topEdgePercent;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1.");
            }
            if (value + HeightPercent > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value + HeightPercent must be less than or equal to 1.");
            }
            topEdgePercent = value;
        }
    }
    private float widthPercent = 0.5f;
    public float WidthPercent
    {
        get => widthPercent;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1.");
            }
            if (value + LeftEdgePercent > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value + LeftEdgePercent must be less than or equal to 1.");
            }
            widthPercent = value;
        }
    }
    private float heightPercent = 0.5f;
    public float HeightPercent
    {
        get => heightPercent;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1.");
            }
            if (value + TopEdgePercent > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value + TopEdgePercent must be less than or equal to 1.");
            }
            heightPercent = value;
        }
    }

    public SplitPane Pane { get; } = new();

    public void Render(int top, int left, int width, int height)
    {
        int x = left + (int)(leftEdgePercent * width);
        int y = top + (int)(topEdgePercent * height);
        int w = (int)(widthPercent * width);
        int h = (int)(heightPercent * height);

        if (w < 3 || h < 3)
        {
            return;
        }

        RenderEdge(y, x, w, h);
        x++;
        y++;
        w-=2;
        h-=2;
        Pane.Render(y, x, w, h);
    }

    private static void RenderEdge(int top, int left, int width, int height)
    {
        Console.SetCursorPosition(left, top);
        Console.Write("┌");
        Console.SetCursorPosition(left + width - 1, top);
        Console.Write("┐");
        Console.SetCursorPosition(left, top + height - 1);
        Console.Write("└");
        Console.SetCursorPosition(left + width - 1, top + height - 1);
        Console.Write("┘");

        Console.SetCursorPosition(left + 1, top);
        Console.Write(new string('─', width - 2));
        Console.SetCursorPosition(left + 1, top + height - 1);
        Console.Write(new string('─', width - 2));

        for (int y = top + 1; y < top + height - 1; y++)
        {
            Console.SetCursorPosition(left, y);
            Console.Write("│");
            Console.SetCursorPosition(left + width - 1, y);
            Console.Write("│");
        }
    }
}