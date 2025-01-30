using MarcoZechner.ConsoleBox;

namespace MarcoZechner.Test;

public class Program{

    private static readonly PanelManager system1 = new(HandleInput);

    public static void Main(){    
        system1.Start();
    }

    public static void HandleInput(ConsoleKeyInfo key) {
        var pane = (DisplayPane)system1.RootPanel;
        
        if (char.IsLetterOrDigit(key.KeyChar) || char.IsWhiteSpace(key.KeyChar))
            pane.Content += key.KeyChar;

        if (key.Key == ConsoleKey.Enter)
            pane.Content += '\n';

        if (key.Key == ConsoleKey.Backspace)
        {
            if (pane.Content.Length> 0)
                pane.Content = pane.Content[..^1];
        }
    }
}