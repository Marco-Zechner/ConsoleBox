namespace MarcoZechner.ConsoleBox;

public static class ConsoleManager{
    public static PanelBase RootPanel = new DisplayPane();

    static ConsoleManager(){
        Console.CursorVisible = false;
        Task.Run(RenderThread);
    }

    private static async Task RenderThread() {
        try {
            while (true) {
                if (RootPanel == null) {
                    throw new InvalidOperationException("Root panel must not be null.");
                }
                Console.Clear();
                lock (RootPanel) {
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
}