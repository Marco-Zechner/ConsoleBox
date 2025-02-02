namespace MarcoZechner.ConsoleBox
{
    public class SplitPane : PanelBase
    {
        public Orientation Orientation { get; set; }
        public List<PanelBase> Panels { get; set; } = [];
        public FloatingPane? Floating { get; set; } = null;


        public int PanelSize(int index, int totalSize) {
            if (index < 0 || index >= Panels.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index out of range in {PanelName}");
            }
            if (totalSize <= 0) {
                throw new ArgumentOutOfRangeException(nameof(totalSize), $"Total size must be greater than 0 in {PanelName}");
            }
            if (Panels.Count == 1) {
                return totalSize;
            }

            if (Panels[index] is SeperatorLine) {
                return 1;
            }

            totalSize -= Panels.Where(p => p is SeperatorLine).Count();
            float totalRelativeSize = Panels.Where(p => p is not SeperatorLine).Sum(p => p.RelativeSize);
            List<int> sizes = [.. Panels.Select(p => (int)((p is not SeperatorLine ? p.RelativeSize : 0) / totalRelativeSize * totalSize))];
            sizes[^1] += totalSize - sizes.Sum();
            return sizes[index];
        }

        private static void RenderVerticalLine(int x, int top, int bottom, char seperatorChar, RenderBuffer buffer) {
            for (int y = top; y < bottom; y++) {
                if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), $"X must be greater than 0. X: {x}");
                if (x >= Console.WindowWidth) throw new ArgumentOutOfRangeException(nameof(x), $"X must be less than the console window width. X: {x}, Console.WindowWidth: {Console.WindowWidth}");
                buffer.Write(x, y, seperatorChar.ToString());
            }
        }

        private static void RenderHorizontalLine(int y, int left, int right, char seperatorChar, RenderBuffer buffer) {
            buffer.Write(left, y, new string(seperatorChar, right - left));
        }

        public void AddSeperator(char? seperatorChar = null)
        {
            Panels.Add(new SeperatorLine(seperatorChar));
        }

        public override void Render(int top, int left, int width, int height, RenderBuffer buffer)
        {
            if (Panels.Count == 0 && Floating?.Pane.Panels.Count == 0) {
                return;
            }
            if (Orientation == Orientation.Horizontal) {
                int x = left;
                for (int i = 0; i < Panels.Count; i++) {
                    if (Panels[i] is SeperatorLine line) {
                        RenderVerticalLine(x, top, top + height, line.SeperatorChar ?? '│', buffer);
                        x++;
                        continue;
                    }

                    Panels[i].Render(top, x, PanelSize(i, width), height, buffer);
                    x += PanelSize(i, width);
                }

                Floating?.Render(top, left, width, height, buffer);

                return;
            }

            int y = top;
            for (int i = 0; i < Panels.Count; i++) {
                if (Panels[i] is SeperatorLine line) {
                    RenderHorizontalLine(y, left, left + width, line.SeperatorChar ?? '─', buffer);
                    y++;
                    continue;
                }

                Panels[i].Render(y, left, width, PanelSize(i, height), buffer);
                y += PanelSize(i, height);
            }

            Floating?.Render(top, left, width, height, buffer);
        }
    }
}