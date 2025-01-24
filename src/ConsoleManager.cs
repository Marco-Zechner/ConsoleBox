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

    private static async Task RenderThread() {
        IsRunning = true;
        try {
            while (IsRunning)
            {
                if (RootPanel == null)
                {
                    throw new InvalidOperationException("Root panel must not be null.");
                }
                lock (RootPanel)
                {
                    RootPanel.Render(0, 0, Console.WindowWidth, Console.WindowHeight);
                }
                await Task.Delay(1000 / 60);
            }
        } 
        catch (Exception e) {
            Console.WriteLine(e);
            if (e.InnerException != null)
                Console.WriteLine(e.InnerException);
        }
    }

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