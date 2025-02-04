using System.Threading.Tasks;
using MarcoZechner.ConsoleBox;

namespace MarcoZechner.Test;

public class Program{

    private static int testIndex = 0;
    private static readonly (string name, Action run)[] tests = [
        ("Test1", Test1.Run),
        ("Test2", Test2.Run),
    ];

    private static readonly DisplayPane testSelection = new () {
        Content = "Loading Tests...",
        PanelName = "Test Selection"
    };

    private static readonly BoxPane testTitle = new () {
        Title = "Tests",
        Content = testSelection,
        PanelName = "Test Title"
    };

    private static readonly FloatingPane screenFloating = new(testTitle) {
        IsVisible = true,
        PanelName = "Screen Floating"
    };

    private static readonly SplitPane mainScreen = new() {
        Floating = screenFloating,
        PanelName = "Main Screen"
    };

    private static readonly PanelManager main = new() {
        RootPanel = mainScreen,
        HandleInputMethod = HandleInput,
        BeforeRender = BeforeRender
    };


    public static async Task Main(){
        // await Task.Run(WriteTest.Run);
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
            if (Test2.ConsoleManager.ExitReason != null) {
                Console.WriteLine(Test2.ConsoleManager.ExitReason);
                return;
            }

            main.Start();
        }
        if (key.Key == ConsoleKey.Escape)
            PanelManager.StopAll();
    }

    public static Task BeforeRender(PanelBase root, RenderBuffer current)
    {
        testSelection.Content = "";
        for (int i = 0; i < tests.Length; i++) {
            testSelection.Content += (i == testIndex ? "> " : "  ") + tests[i].name + "\n";
        }

        return Task.CompletedTask;
    }
}