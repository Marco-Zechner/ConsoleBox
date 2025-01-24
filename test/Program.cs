namespace MarcoZechner.Test;

using MarcoZechner.ConsoleBox;

public class Program
{
    private static DisplayPane editorPane = new(){
        RelativeSize = 0.8f
    };

    private static DisplayPane keybindsPane = new(){
        RelativeSize = 0.2f
    };

    private static SplitPane mainScreen = new(){
        Orientation = Orientation.Vertical,
    };
    public static async Task Main()
    {
        mainScreen.Panels.Add(editorPane);
        mainScreen.AddSeperator();
        mainScreen.Panels.Add(keybindsPane);

        ConsoleManager.RootPanel.Panels.Add(mainScreen);

        HandleInput(editorPane);

        ConsoleManager.Stop();

        Console.WriteLine("Goodbye!");
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

            if (key.Key == ConsoleKey.Q && key.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                ConsoleManager.Stop();
                return;
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

    private static async Task ModifyText(DisplayPane leftPane)
    {
        string lorumIpsumText = """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed non risus. Suspendisse lectus tortor, dignissim sit amet, adipiscing nec, ultricies sed, dolor. Cras elementum ultrices diam. Maecenas ligula massa, varius a, semper congue, euismod non, mi. Proin porttitor, orci nec nonummy molestie, enim est eleifend mi, non fermentum diam nisl sit amet erat. Duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed d
            """;

        while (true)
        {
            var sections = lorumIpsumText.Split(' ', 2);
            var nextWord = sections.First();
            if (sections.Length == 1)
            {
                break;
            }
            lorumIpsumText = sections.Last();
            leftPane.Content += " " + nextWord;

            await Task.Delay(100);
        }
    }
}