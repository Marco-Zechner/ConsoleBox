namespace MarcoZechner.ConsoleBox;

public static class ConsoleManager{
    public static SplitPane RootPanel {get;} = new SplitPane();

    public static bool IsRunning { get; private set; } = false;

    private static Task? RenderTask { get; set; } = null;

    static ConsoleManager(){
        Console.CursorVisible = false;
        Console.Clear();
        RenderTask = Task.Run(RenderThread);
    }

    private static int frameCount = 0;
    private static async Task RenderThread() {
        IsRunning = true;
        RenderBuffer current = new(Console.WindowWidth, Console.WindowHeight);
        try {
            while (IsRunning)
            {
                if (RootPanel == null)
                {
                    throw new InvalidOperationException("Root panel must not be null.");
                }
                lock (RootPanel)
                {
                    RenderBuffer next = new(Console.WindowWidth, Console.WindowHeight);
                    RootPanel.Render(0, 0, Console.WindowWidth, Console.WindowHeight, next);
                    if (RenderBuffer.GetChanges(current, next, out RenderBuffer changes)) {
                        current = next;
                        // Console.ForegroundColor = colors[frameCount % colors.Length];
                        changes.Render();
                        frameCount++;
                    }
                }
                await Task.Delay(1000 / 60);
            }
        } 
        catch (Exception e) {
            Console.WriteLine(e);
            if (e.InnerException != null)
                Console.WriteLine(e.InnerException);
        }

        Console.ForegroundColor = ConsoleColor.White;
    }

    private static ConsoleColor[] colors = [
        // ConsoleColor.Black,
        ConsoleColor.DarkBlue,
        ConsoleColor.DarkGreen,
        ConsoleColor.DarkCyan,
        ConsoleColor.DarkRed,
        ConsoleColor.DarkMagenta,
        ConsoleColor.DarkYellow,
        ConsoleColor.Gray,
        ConsoleColor.DarkGray,
        ConsoleColor.Blue,
        ConsoleColor.Green,
        ConsoleColor.Cyan,
        ConsoleColor.Red,
        ConsoleColor.Magenta,
        ConsoleColor.Yellow,
        ConsoleColor.White
    ];

    public static void Stop() {
        IsRunning = false;
    }

    public static void Start() {
        if (RenderTask != null) {
            IsRunning = false;
            RenderTask.Wait();
        }
        RenderTask = Task.Run(RenderThread);
    }
}