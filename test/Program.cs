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
        Content = "Ctrl+M: Open Menu\nCtrl+Q: Quit"
    };

    private static SplitPane mainScreen = new(){
        Orientation = Orientation.Vertical,
    };

    public static async Task Main()
    {
        Console.WriteLine("Press Ctrl+M to open the menu.");

        mainScreen.Panels.Add(dynamicPane);
        mainScreen.AddSeperator('=');
        mainScreen.Panels.Add(keybindsPane);

        ConsoleManager.RootPanel.Panels.Add(mainScreen);

        HandleInput(editorPane);

        ConsoleManager.Stop();
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
                    if (ConsoleManager.RootPanel.Floating != null)
                    {
                        ConsoleManager.RootPanel.Floating = null;
                        continue;
                    }
                    ConsoleManager.RootPanel.Floating = new FloatingPane();
                    var menuSplit = new SplitPane
                    {
                        Orientation = Orientation.Horizontal
                    };

                    var leftMenu = new DisplayPane
                    {
                        RelativeSize = 0.2f
                    };

                    var rightMenu = new DisplayPane
                    {
                        RelativeSize = 0.8f
                    };

                    menuSplit.Panels.Add(leftMenu);
                    menuSplit.AddSeperator();
                    menuSplit.Panels.Add(rightMenu);

                    ConsoleManager.RootPanel.Floating.Pane.Panels.Add(menuSplit);

                    leftMenu.Content = "";
                    for (int i = 0; i < menuItems.Count; i++)
                    {
                        leftMenu.Content += "  " + menuItems[i].menuName + "\n";
                    }
                    rightMenu.Content = menuItems[0].menuContent;

                    MenuInput(leftMenu, rightMenu);

                    continue;
                }
           
                lock (ConsoleManager.RootPanel) {
                    if (key.Key == ConsoleKey.RightArrow) {
                        var split = mainScreen.Panels[0] as SplitPane;

                        split?.Panels.Add(new BoxPane() {
                            Title = $"Box Pane {split.Panels.Count}", 
                            Content = new DisplayPane(),
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

    private static void MenuInput(DisplayPane leftMenu, DisplayPane rightMenu)
    {
        int menuIndex = 0;
        int mode = -1;
        while (ConsoleManager.IsRunning && ConsoleManager.RootPanel.Floating != null)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                if (mode == -1)
                {
                    mode = menuIndex;
                    continue;
                } else 
                    mode = -1;
            }

            if (mode != -1) {
                switch (mode) {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        HandleMoveInput(key);
                        break;
                }
                continue;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                ConsoleManager.RootPanel.Floating = null;
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