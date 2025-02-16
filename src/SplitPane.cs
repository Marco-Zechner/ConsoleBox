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
            if (Panels[index].FixedSize > 0) {
                return Panels[index].FixedSize;
            }
            if (Panels.Count == 1) {
                return totalSize;
            }

            totalSize -= Panels.Sum(p => p.FixedSize);
            float totalRelativeSize = Panels.Where(p => p.FixedSize == 0).Sum(p => p.RelativeSize);
            List<int> sizes = [.. Panels.Select(p => (int)((p.FixedSize == 0 ? p.RelativeSize : 0) / totalRelativeSize * totalSize))];
            sizes[^1] += totalSize - sizes.Sum();
            return sizes[index];
        }

        public void AddSeperator(char seperatorChar = '+')
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
                    Panels[i].Render(top, x, PanelSize(i, width), height, buffer);
                    x += PanelSize(i, width);
                }

                Floating?.Render(top, left, width, height, buffer);

                return;
            }

            int y = top;
            for (int i = 0; i < Panels.Count; i++) {
                Panels[i].Render(y, left, width, PanelSize(i, height), buffer);
                y += PanelSize(i, height);
            }

            Floating?.Render(top, left, width, height, buffer);
        }
    }
}