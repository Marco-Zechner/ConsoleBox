namespace MarcoZechner.Test;

using MarcoZechner.ConsoleBox;

public class Program
{
    public static async Task Main()
    {
        SplitPane splitPane = new()
        {
            Orientation = Orientation.Horizontal,
        };

        SplitPane splitPane2 = new()
        {
            Orientation = Orientation.Vertical,
        };

        SplitPane splitPane3 = new()
        {
            RelativeSize = 80f,
            Orientation = Orientation.Vertical,
        };

        var leftPane = new DisplayPane
        {
            RelativeSize = 60f,
            Content = "Hello, World!            ."
        };

        var middelPane = new DisplayPane
        {
            RelativeSize = 20f,
            Content = "This is a test."
        };

        splitPane3.AddPanel(leftPane);
        splitPane3.AddPanel(middelPane);

        splitPane.AddPanel(leftPane);
        splitPane.AddPanel(middelPane);
        splitPane.AddPanel(splitPane3);

        splitPane2.AddPanel(middelPane);
        splitPane2.AddPanel(leftPane);
        splitPane2.AddPanel(leftPane);

        ConsoleManager.RootPanel = splitPane;

        string lorumIpsumText ="""
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed non risus. Suspendisse lectus tortor, dignissim sit amet, adipiscing nec, ultricies sed, dolor. Cras elementum ultrices diam. Maecenas ligula massa, varius a, semper congue, euismod non, mi. Proin porttitor, orci nec nonummy molestie, enim est eleifend mi, non fermentum diam nisl sit amet erat. Duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed duis semper. Duis arcu massa, scelerisque vitae, consequat in, pretium a, enim. Pellentesque congue. Ut in risus volutpat libero pharetra tempor. Cras vestibulum bibendum augue. Praesent egestas leo in pede. Praesent blandit odio eu enim. Pellentesque sed d
            """;

        lorumIpsumText = string.Concat(Enumerable.Repeat(lorumIpsumText, 10));

        int counter = 0;
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

            if (counter >= 20) {
                ConsoleManager.RootPanel = ConsoleManager.RootPanel == splitPane ? splitPane2 : splitPane;
                counter = 0;
            }

            counter++;

            await Task.Delay(100);
        }

        Console.ReadLine();
    }
}