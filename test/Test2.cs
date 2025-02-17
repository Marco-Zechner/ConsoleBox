namespace MarcoZechner.Test;

using MarcoZechner.ConsoleBox;

public class Test2
{
    private static DisplayPane editorPane = new() {
        PanelName = "Editor Pane"
    };

    private static BoxPane boxPane = new(){
        Title = "Box Pane",
        TitleAlignment = 0.5f,
        Content = editorPane,
        PanelName = "Box Pane"
    };

    private static SplitPane dynamicPane = new(){
        Orientation = Orientation.Horizontal,
        Panels = [boxPane],
        RelativeSize = 0.8f,
        PanelName = "Dynamic Pane"
    };

    private static DisplayPane keybindsPane = new(){
        RelativeSize = 0.2f,
        PanelName = "Keybinds Pane",
    };

    private static SplitPane mainScreen = new(){
        Orientation = Orientation.Vertical,
        PanelName = "Main Screen"
    };

    public static readonly PanelManager ConsoleManager = new(){
        RootPanel = mainScreen,
        HandleInputMethod = HandleInput,
        BeforeRender = AutoUpdateHints
    };

    public static void Run()
    {
        mainScreen.Panels.Clear();

        mainScreen.Panels.Add(dynamicPane);
        mainScreen.AddSeperator('=');
        mainScreen.Panels.Add(keybindsPane);

        //TODO: fix this
        // ((SplitPane)ConsoleManager.RootPanel).Panels.Add(mainScreen); //this breaks everything... why?
        //oh ... because mainScreen is already the root panel... so circular reference ... endless loop, I need to throw a exception for this...

        ConsoleManager.Start();
    }

    public static Task AutoUpdateHints(PanelBase root, RenderBuffer current) {
        if (menuMode != -1) {
            keybindsPane.Content = menuMode switch
            {
                4 => "Enter: Unselect\nArrow keys: Move\nCtrl+Arrow keys: Resize",
                _ => "Enter: Unselect\nNot implemented",
            };
            return Task.CompletedTask;
        }

        var Floating = ((SplitPane)ConsoleManager.RootPanel).Floating;

        if (Floating != null && Floating.IsVisible)
        {
            keybindsPane.Content = "ESC: Close Menu\nArrow up/down: Navigate\nEnter: Select";
            return Task.CompletedTask;
        }

        keybindsPane.Content = "Ctrl+M: Open Menu\nCtrl+Q: Quit\nCtrl+Arrow Left/Right: Split/Unsplit Editor";

        return Task.CompletedTask;
    }

    private static void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Tab) {
            ConsoleManager.Debug_ColorUpdates = !ConsoleManager.Debug_ColorUpdates;
            return;
        }

        if (key.Key == ConsoleKey.Enter)
        {
            editorPane.Content += "\n";
            return;
        }

        if (key.Key == ConsoleKey.F1) {
            keybindsPane.IsVisible = !keybindsPane.IsVisible;
            mainScreen.Panels[1].IsVisible = keybindsPane.IsVisible; //seperator
            return;
        }

        var Floating = ((SplitPane)ConsoleManager.RootPanel).Floating;

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

                if (Floating == null)
                {
                    Floating = new FloatingPane() {
                        PanelName = "Menu Floating",
                    };
                    menuSplit = Floating.Pane;
                    menuSplit.Orientation = Orientation.Horizontal;

                    leftMenu = new DisplayPane
                    {
                        RelativeSize = 0.2f,
                        PanelName = "Menu Left"
                    };

                    rightMenu = new DisplayPane
                    {
                        RelativeSize = 0.8f,
                        PanelName = "Menu Right"
                    };

                    menuSplit.Panels.Add(leftMenu);
                    menuSplit.AddSeperator();
                    menuSplit.Panels.Add(rightMenu);
                }
                else {
                    menuSplit = Floating.Pane.Panels[0] as SplitPane;
                    leftMenu = menuSplit?.Panels[0] as DisplayPane;
                    rightMenu = menuSplit?.Panels[1] as DisplayPane;
                }

                Floating.IsVisible = true;

                leftMenu.Content = "";
                for (int i = 0; i < menuItems.Count; i++)
                {
                    leftMenu.Content += (i == menuIndex ? '>' : ' ') + " " + menuItems[i].menuName + "\n";
                }
                rightMenu.Content = menuItems[menuIndex].menuContent;

                ConsoleManager.HandleInputMethod = MenuInput;

                ((SplitPane)ConsoleManager.RootPanel).Floating = Floating;
                return;
            }
        
            lock (PanelManager.RenderLock) {
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

            return;
        }

        if (key.Key == ConsoleKey.Backspace && editorPane.Content.Length > 0)
        {
            editorPane.Content = editorPane.Content[..^1];
            return;
        }

        if (key.Modifiers == ConsoleModifiers.None || key.Modifiers == ConsoleModifiers.Shift && (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsWhiteSpace(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsSeparator(key.KeyChar)))
        {
            editorPane.Content += key.KeyChar;
            return;
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
    private static void MenuInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Enter)
        {
            if (menuMode == -1)
            {
                menuMode = menuIndex;
                return;
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
            return;
        }

        var Floating = ((SplitPane)ConsoleManager.RootPanel).Floating;

        if (key.Key == ConsoleKey.Escape)
        {
            Floating.IsVisible = false;
            ConsoleManager.HandleInputMethod = HandleInput;
            return;
        }

        var leftMenu = (DisplayPane)Floating.Pane.Panels[0];
        var rightMenu = (DisplayPane)Floating.Pane.Panels[2];

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
            return;
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
            return;
        }
    }

    private static void HandleMoveInput(ConsoleKeyInfo key) {
        var Floating = ((SplitPane)ConsoleManager.RootPanel).Floating;

        if (Floating == null)
        {
            return;
        }

        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            if (key.Key == ConsoleKey.UpArrow)
            {
                Floating.HeightPercent = Math.Max(0, Floating.HeightPercent - 0.01f);
                return;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                Floating.HeightPercent = Math.Min(1 - Floating.TopEdgePercent, Floating.HeightPercent + 0.01f);
                return;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                Floating.WidthPercent = Math.Max(0, Floating.WidthPercent - 0.01f);
                return;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                Floating.WidthPercent = Math.Min(1 - Floating.LeftEdgePercent, Floating.WidthPercent + 0.01f);
                return;
            }

            return;
        }

        if (key.Key == ConsoleKey.UpArrow)
        {
            Floating.TopEdgePercent = Math.Max(0, Floating.TopEdgePercent - 0.01f);
            return;
        }

        if (key.Key == ConsoleKey.DownArrow)
        {
            Floating.TopEdgePercent = Math.Min(1-Floating.HeightPercent, Floating.TopEdgePercent + 0.01f);
            return;
        }

        if (key.Key == ConsoleKey.LeftArrow)
        {
            Floating.LeftEdgePercent = Math.Max(0, Floating.LeftEdgePercent - 0.01f);
            return;
        }

        if (key.Key == ConsoleKey.RightArrow)
        {
            Floating.LeftEdgePercent = Math.Min(1-Floating.WidthPercent, Floating.LeftEdgePercent + 0.01f);
            return;
        }
    }
}