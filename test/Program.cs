using MarcoZechner.ConsoleBox;

namespace MarcoZechner.Test;

public class Program{

    private static int testIndex = 0;
    private static readonly (string name, Action run)[] tests = [
        ("Test1", Test1.Run),
        ("Test2", Test1.Run),
        ("Test3", Test1.Run)
    ];

    private static readonly DisplayPane testSelection = new () {
        Content = "Loading Tests..."
    };

    private static readonly BoxPane testTitle = new () {
        Title = "Tests",
        Content = testSelection
    };

    private static readonly FloatingPane screenFloating = new(testTitle) {
        IsVisible = true
    };

    private static readonly SplitPane mainScreen = new() {
        Floating = screenFloating
    };

    private static readonly PanelManager main = new() {
        RootPanel = mainScreen,
        HandleInputMethod = HandleInput,
        BeforeRender = BeforeRender
    };


    public static void Main(){
        main.Start();
        Console.WriteLine("Stopped all Panels");
    }

    public static void HandleInput(ConsoleKeyInfo key) {
        if (key.Key ==  ConsoleKey.Tab)
            main.Debug_ColorUpdates = !main.Debug_ColorUpdates;

        if (key.Key == ConsoleKey.DownArrow) 
            testIndex = (testIndex + 1) % tests.Length;

        if (key.Key == ConsoleKey.UpArrow)
            testIndex = (testIndex - 1 + tests.Length) % tests.Length;
  
        if (key.Key == ConsoleKey.Enter) {
            main.Pause();
            tests[testIndex].run();
            main.Start();
        }
        if (key.Key == ConsoleKey.Escape)
            PanelManager.StopAll();
    }

    public static async Task BeforeRender(PanelBase root, RenderBuffer current) {
        testSelection.Content = "";
        for (int i = 0; i < tests.Length; i++) {
            testSelection.Content += (i == testIndex ? "> " : "  ") + tests[i].name + "\n";
        }
    }
}