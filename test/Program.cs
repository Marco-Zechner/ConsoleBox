namespace MarcoZechner.Test;

using MarcoZechner.ConsoleBox;

public class Program
{
    private static DisplayPane editorPane = new();

    private static BoxPane boxPane = new(){
        Title = "Box Pane",
        TitleAlignment = 0.5f,
        Content = editorPane,
    };

    private static SplitPane dynamicPane = new(){
        Orientation = Orientation.Horizontal,
        Panels = [boxPane],
        RelativeSize = 0.8f
    };

    private static DisplayPane keybindsPane = new(){
        RelativeSize = 0.2f,
    };

    private static SplitPane mainScreen = new(){
        Orientation = Orientation.Vertical,
    };

    public static async Task Main()
    {
        mainScreen.Panels.Add(dynamicPane);
        mainScreen.AddSeperator('=');
        mainScreen.Panels.Add(keybindsPane);

        ConsoleManager.RootPanel.Panels.Add(mainScreen);

        Task.Run(() => AutoUpdateHints());

        HandleInput(editorPane);

        ConsoleManager.Stop();
    }

    private static void AutoUpdateHints() {
        while (ConsoleManager.IsRunning) {
            Thread.Sleep(20);
            if (menuMode != -1) {
                keybindsPane.Content = menuMode switch
                {
                    4 => "Enter: Unselect\nArrow keys: Move\nCtrl+Arrow keys: Resize",
                    _ => "Enter: Unselect\nNot implemented",
                };
                continue;
            }

            if (ConsoleManager.RootPanel.Floating != null &&  ConsoleManager.RootPanel.Floating.IsVisible)
            {
                keybindsPane.Content = "ESC: Close Menu\nArrow up/down: Navigate\nEnter: Select";
                continue;
            }

            keybindsPane.Content = "Ctrl+M: Open Menu\nCtrl+Q: Quit\nCtrl+Arrow Left/Right: Split/Unsplit Editor";
        }
    }

    private static void HandleInput(DisplayPane editorPane)
    {
        Console.WriteLine(ConsoleManager.IsRunning);
        while (ConsoleManager.IsRunning) {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                editorPane.Content += "\n";
                continue;
            }

            if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                if (key.Key == ConsoleKey.Q)
                {
                    ConsoleManager.Stop();
                    return;
                }

                if (key.Key == ConsoleKey.M)
                {
                    SplitPane? menuSplit = null;
                    DisplayPane? leftMenu = null;
                    DisplayPane? rightMenu = null;

                    if (ConsoleManager.RootPanel.Floating == null)
                    {
                        ConsoleManager.RootPanel.Floating = new FloatingPane();
                        menuSplit = new SplitPane
                        {
                            Orientation = Orientation.Horizontal
                        };

                        leftMenu = new DisplayPane
                        {
                            RelativeSize = 0.2f
                        };

                        rightMenu = new DisplayPane
                        {
                            RelativeSize = 0.8f
                        };

                        menuSplit.Panels.Add(leftMenu);
                        menuSplit.AddSeperator();
                        menuSplit.Panels.Add(rightMenu);

                        ConsoleManager.RootPanel.Floating.Pane.Panels.Add(menuSplit);
                    }
                    else {
                        menuSplit = ConsoleManager.RootPanel.Floating.Pane.Panels[0] as SplitPane;
                        leftMenu = menuSplit?.Panels[0] as DisplayPane;
                        rightMenu = menuSplit?.Panels[2] as DisplayPane;
                    }

                    ConsoleManager.RootPanel.Floating.IsVisible = true;

                    leftMenu.Content = "";
                    for (int i = 0; i < menuItems.Count; i++)
                    {
                        leftMenu.Content += (i == menuIndex ? '>' : ' ') + " " + menuItems[i].menuName + "\n";
                    }
                    rightMenu.Content = menuItems[menuIndex].menuContent;

                    MenuInput(leftMenu, rightMenu);

                    continue;
                }
           
                lock (ConsoleManager.RootPanel) {
                    if (key.Key == ConsoleKey.RightArrow) {
                        var split = mainScreen.Panels[0] as SplitPane;

                        split?.Panels.Add(new BoxPane() {
                            Title = $"Box Pane {split.Panels.Count}", 
                            Content = editorPane,
                            DoubleLines = split.Panels.Count % 2 == 0
                        });
                    }

                    if (key.Key == ConsoleKey.LeftArrow) {
                        var split = mainScreen.Panels[0] as SplitPane;

                        if (split?.Panels.Count > 1) {
                            split?.Panels.RemoveAt(split.Panels.Count - 1);
                        }
                    }
                }

                continue;
            }

            if (key.Key == ConsoleKey.Backspace && editorPane.Content.Length > 0)
            {
                editorPane.Content = editorPane.Content[..^1];
                continue;
            }

            if (key.Modifiers == ConsoleModifiers.None || key.Modifiers == ConsoleModifiers.Shift && (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsWhiteSpace(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsSeparator(key.KeyChar)))
            {
                editorPane.Content += key.KeyChar;
                continue;
            }
        }
    }

    private static readonly List<(string menuName, string menuContent)> menuItems =
    [
        ("File", "New\nOpen\nSave\nSave As\nExit"),
        ("Edit", "Undo\nRedo\nCut\nCopy\nPaste"),
        ("View", "Zoom In\nZoom Out\nFull Screen"),
        ("Help", "About\nContact\nLicense"),
        ("Move", "Up\nDown\nLeft\nRight")
    ];


    private static int menuIndex = 0;
    private static int menuMode = -1;
    private static void MenuInput(DisplayPane leftMenu, DisplayPane rightMenu)
    {
        while (ConsoleManager.IsRunning && ConsoleManager.RootPanel.Floating != null)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                if (menuMode == -1)
                {
                    menuMode = menuIndex;
                    continue;
                } else 
                    menuMode = -1;
            }

            if (menuMode != -1) {
                switch (menuMode) {
                    case 4:
                        HandleMoveInput(key);
                        break;
                    default:
                        break;
                }
                continue;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                ConsoleManager.RootPanel.Floating.IsVisible = false;
                return;
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                menuIndex--;
                if (menuIndex < 0)
                {
                    menuIndex = 0;
                }
                leftMenu.Content = "";
                for (int i = 0; i < menuItems.Count; i++)
                {
                    leftMenu.Content += (i == menuIndex ? '>' : ' ') + " " + menuItems[i].menuName + "\n";
                }
                rightMenu.Content = menuItems[menuIndex].menuContent;
                continue;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                menuIndex++;
                if (menuIndex >= menuItems.Count)
                {
                    menuIndex = menuItems.Count - 1;
                }
                leftMenu.Content = "";
                for (int i = 0; i < menuItems.Count; i++)
                {
                    leftMenu.Content += (i == menuIndex ? '>' : ' ') + " " + menuItems[i].menuName + "\n";
                }
                rightMenu.Content = menuItems[menuIndex].menuContent;
                continue;
            }
        }
    }

    private static void HandleMoveInput(ConsoleKeyInfo key) {
        if (ConsoleManager.RootPanel.Floating == null)
        {
            return;
        }

        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            if (key.Key == ConsoleKey.UpArrow)
            {
                ConsoleManager.RootPanel.Floating.HeightPercent = Math.Max(0, ConsoleManager.RootPanel.Floating.HeightPercent - 0.01f);
                return;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                ConsoleManager.RootPanel.Floating.HeightPercent = Math.Min(1 - ConsoleManager.RootPanel.Floating.TopEdgePercent, ConsoleManager.RootPanel.Floating.HeightPercent + 0.01f);
                return;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                ConsoleManager.RootPanel.Floating.WidthPercent = Math.Max(0, ConsoleManager.RootPanel.Floating.WidthPercent - 0.01f);
                return;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                ConsoleManager.RootPanel.Floating.WidthPercent = Math.Min(1 - ConsoleManager.RootPanel.Floating.LeftEdgePercent, ConsoleManager.RootPanel.Floating.WidthPercent + 0.01f);
                return;
            }

            return;
        }

        if (key.Key == ConsoleKey.UpArrow)
        {
            ConsoleManager.RootPanel.Floating.TopEdgePercent = Math.Max(0, ConsoleManager.RootPanel.Floating.TopEdgePercent - 0.01f);
            return;
        }

        if (key.Key == ConsoleKey.DownArrow)
        {
            ConsoleManager.RootPanel.Floating.TopEdgePercent = Math.Min(1-ConsoleManager.RootPanel.Floating.HeightPercent, ConsoleManager.RootPanel.Floating.TopEdgePercent + 0.01f);
            return;
        }

        if (key.Key == ConsoleKey.LeftArrow)
        {
            ConsoleManager.RootPanel.Floating.LeftEdgePercent = Math.Max(0, ConsoleManager.RootPanel.Floating.LeftEdgePercent - 0.01f);
            return;
        }

        if (key.Key == ConsoleKey.RightArrow)
        {
            ConsoleManager.RootPanel.Floating.LeftEdgePercent = Math.Min(1-ConsoleManager.RootPanel.Floating.WidthPercent, ConsoleManager.RootPanel.Floating.LeftEdgePercent + 0.01f);
            return;
        }
    }
}