using System.Text;

namespace MarcoZechner.ConsoleBox;

public class RenderBuffer(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public char?[,] Buffer { get; } = new char?[height, width];

    public RenderBuffer(RenderBuffer original) : this(original.Width, original.Height)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Buffer[y, x] = original.Buffer[y, x];
            }
        }
    }

    public RenderBuffer(int width, int height, char? filler) : this(width, height)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Buffer[y, x] = filler;
            }
        }
    }

    public void Clear(char? clearChar = ' ') {
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                Buffer[y, x] = clearChar;
            }
        }
    }

    public void Write(int x, int y, string text) {
        if (x >= Width || y >= Height || text == null || text.Length == 0 || y < 0 || x + text.Length < 0) {
            return;
        }

        if (text.Contains('\n')) {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                Write(x, y + i, lines[i]);
            }
            return;
        }

        text = text.Replace("\t", "    ");

        for (int i = 0; i < text.Length; i++) {
            if (x + i >= Width) {
                break;
            }
            if (x + i < 0) {
                continue;
            }
            Buffer[y, x + i] = text[i];
        }
    }

    public void Write(int x, int y, char c) {
        if (x >= Width || y >= Height || y < 0 || x < 0 || c == '\0') {
            return;
        }
        Buffer[y, x] = c;
    }

    public static bool GetChanges(RenderBuffer before, RenderBuffer after, out RenderBuffer changes) {
        bool hasChanges = false;
        changes = new(after.Width, after.Height);
        for (int y = 0; y < after.Height; y++) {
            for (int x = 0; x < after.Width; x++) {
                if (y >= before.Height || x >= before.Width) {
                    changes.Buffer[y, x] = after.Buffer[y, x];
                    hasChanges = true;
                    continue;
                }

                if (before.Buffer[y, x] != after.Buffer[y, x] && after.Buffer[y, x] != null) {	
                    changes.Buffer[y, x] = after.Buffer[y, x];
                    hasChanges = true;
                }
            }
        }
        return hasChanges;
    }

    public void Render() {
        List<(int x, int y, StringBuilder sb)> lines = [];
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (Buffer[y, x] == null) {
                    continue;
                }

                if (lines.Count == 0 || lines[^1].y != y || lines[^1].x != x - lines[^1].sb.Length) {
                    lines.Add((x, y, new()));
                }

                lines[^1].sb.Append(Buffer[y, x]);
            }
        }

        foreach (var (x, y, sb) in lines) {
            Console.SetCursorPosition(x, y);
            Console.Write(sb.ToString());
        }
    }
}