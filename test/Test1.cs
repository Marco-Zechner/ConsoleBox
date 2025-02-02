using MarcoZechner.ConsoleBox;

namespace MarcoZechner.Test;

public class Test1{

    private static readonly BoxPane activePane = new() {
        Title = "Active Pane",
        Content = selectedPane,
        PanelName = "Active Pane"
    };

    private static readonly BoxPane inactive = new() {
        Title = "Inactive Pane",
        Content = selectedPane == left ? right : left,
        PanelName = "Inactive Pane"
    };

    private static readonly DisplayPane left = new() {
        Content = "Left",
        PanelName = "Left Pane"
    };

    private static readonly DisplayPane right = new() {
        Content = "Right",
        PanelName = "Right Pane"
    };

    private static readonly PanelManager subManager = new() {
        RootPanel = new SplitPane() {
            Panels = [activePane, inactive],
            Orientation = Orientation.Horizontal,
        },
        HandleInputMethod = HandleInput,
        BeforeRender = BeforeRender,
    };

    private static DisplayPane selectedPane = left;

    public static void Run(){
        subManager.Start();
    }

    public static void HandleInput(ConsoleKeyInfo key) {
        if (key.Key ==  ConsoleKey.Tab)
        {
            subManager.Debug_ColorUpdates = !subManager.Debug_ColorUpdates;
        }

        else if (key.Key == ConsoleKey.Escape)
        {
            subManager.Stop();
        }

        else if (key.Key == ConsoleKey.LeftArrow)
        {
            selectedPane = left;
        }

        else if (key.Key == ConsoleKey.RightArrow)
        {
            selectedPane = right;
        }

        else if (key.Key == ConsoleKey.Backspace)
        {
            if (selectedPane.Content.Length > 0)
            {
                selectedPane.Content = selectedPane.Content[..^1];
            }
        }
        else {
            selectedPane.Content += key.KeyChar;
        }

    }

    public static Task BeforeRender(PanelBase root, RenderBuffer current)
    {
        activePane.Content = selectedPane;
        inactive.Content = selectedPane == left ? right : left;

        if (selectedPane == right) {
            subManager.RootPanel = new SplitPane() {
                Panels = [inactive, activePane],
                Orientation = Orientation.Horizontal,
            };
        }else {
            subManager.RootPanel = new SplitPane() {
                Panels = [activePane, inactive],
                Orientation = Orientation.Horizontal,
            };
        }

        return Task.CompletedTask;
    }
}