namespace MarcoZechner.ConsoleBox
{
    public class SplitPane : PanelBase
    {
        private Orientation orientation;
        public Orientation Orientation {
            get => orientation;
            set => orientation = value;
        }
        private List<PanelBase> panels = [];
        public List<PanelBase> Panels {
            get => panels;
            set => panels = value;
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

            if (panels[index] is SeperatorLine) {
                return 1;
            }

            totalSize -= panels.Where(p => p is SeperatorLine).Count();
            float totalRelativeSize = panels.Where(p => p is not SeperatorLine).Sum(p => p.RelativeSize);
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
                    if (panels[i] is SeperatorLine) {
                        RenderVerticalLine(x, top, top + height, ((SeperatorLine)panels[i]).SeperatorChar ?? '│');
                        x++;
                        continue;
                    }

                    panels[i].Render(top, x, PanelSize(i, width), height);
                    x += PanelSize(i, width);
                }
                return;
            }

            int y = top;
            for (int i = 0; i < panels.Count; i++) {
                if (panels[i] is SeperatorLine) {
                    RenderHorizontalLine(y, left, left + width, ((SeperatorLine)panels[i]).SeperatorChar ?? '─');
                    y++;
                    continue;
                }

                panels[i].Render(y, left, width, PanelSize(i, height));
                y += PanelSize(i, height);
            }
        }

        private static void RenderVerticalLine(int x, int top, int bottom, char seperatorChar = '│') {
            for (int y = top; y < bottom; y++) {
                if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), $"X must be greater than 0. X: {x}");
                if (x >= Console.WindowWidth) throw new ArgumentOutOfRangeException(nameof(x), $"X must be less than the console window width. X: {x}, Console.WindowWidth: {Console.WindowWidth}");
                Console.SetCursorPosition(x, y);
                Console.Write(seperatorChar);
            }
        }

        private static void RenderHorizontalLine(int y, int left, int right, char seperatorChar = '─') {
            Console.SetCursorPosition(left, y);
            Console.Write(new string(seperatorChar, right - left));
        }

        public void AddSeperator(char? seperatorChar = null)
        {
            panels.Add(new SeperatorLine(seperatorChar));
        }
    }
}