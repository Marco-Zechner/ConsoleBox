namespace MarcoZechner.ConsoleBox
{
    public class SplitPane : PanelBase
    {
        private Orientation orientation;
        public Orientation Orientation {
            get => orientation;
            set => orientation = value;
        }
        private readonly List<PanelBase> panels = [];
        public void AddPanel(PanelBase panel){
            panels.Add(panel);
        }
        public void RemovePanel(PanelBase panel){
            panels.Remove(panel);
        }
        public int PanelSize(int index, int totalSize) {
            if (index < 0 || index >= panels.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
            }
            if (totalSize <= 0) {
                throw new ArgumentOutOfRangeException(nameof(totalSize), "Total size must be greater than 0.");
            }
            if (panels.Count == 1) {
                return totalSize;
            }

            totalSize -= panels.Count - 1;
            float totalRelativeSize = panels.Sum(p => p.RelativeSize);
            List<int> sizes = [.. panels.Select(p => (int)(p.RelativeSize / totalRelativeSize * totalSize))];
            sizes[^1] += totalSize - sizes.Sum();
            return sizes[index];
        }

        public override void Render(int top, int left, int width, int height)
        {
            if (panels.Count == 0) {
                return;
            }
            if (orientation == Orientation.Horizontal) {
                int x = left;
                for (int i = 0; i < panels.Count; i++) {
                    panels[i].Render(top, x, PanelSize(i, width), height);
                    if (i == panels.Count - 1) {
                        break;
                    }
                    x += PanelSize(i, width);
                    RenderVerticalLine(x, top, top + height);
                    x++;
                }
                return;
            }

            int y = top;
            for (int i = 0; i < panels.Count; i++) {
                panels[i].Render(y, left, width, PanelSize(i, height));
                if (i == panels.Count - 1) {
                    break;
                }
                y += PanelSize(i, height);
                RenderHorizontalLine(y, left, left + width);
                y++;
            }
        }

        private static void RenderVerticalLine(int x, int top, int bottom) {
            for (int y = top; y < bottom; y++) {
                if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), $"X must be greater than 0. X: {x}");
                if (x >= Console.WindowWidth) throw new ArgumentOutOfRangeException(nameof(x), $"X must be less than the console window width. X: {x}, Console.WindowWidth: {Console.WindowWidth}");
                Console.SetCursorPosition(x, y);
                Console.Write("│");
            }
        }

        private static void RenderHorizontalLine(int y, int left, int right) {
            Console.SetCursorPosition(left, y);
            Console.Write(new string('─', right - left));
        }
    }
}