using System.Text;

namespace MarcoZechner.Test;

public static class WriteTest{

    private static List<(string type, int count, int clrCount, int charactersScreen)> testResults = [];
    private static int counter = 0;

    public static async Task Run() {
        await RunTest("OneBlock", TestOneBlock);
        await RunTest("OneBlock2", TestOneBlock2);
        // await RunTest("OneBlockRnd", TestOneBlockRnd);
        // await RunTest("OneBlockFlipFlop", TestOneBlockFlipFlop);
        // await RunTest("OneLine", TestOneLine);
        // await RunTest("OneLineSet", TestOneLineSet);
        // await RunTest("SideBox", TestSideBox);
        // await RunTest("SideBoxSet", TestSideBoxSet);

        Console.Clear();
        Console.WriteLine("Test Results:");
        foreach ((string type, int count, int clrCount, int charactersScreen) in testResults) {
            Console.WriteLine($"{type}: {count} ({clrCount}) for {charactersScreen} characters on screen");
        }
    }

    private static async Task RunTest(string type, Action test) {
        counter = 0;
        Task timer = Task.Delay(10000);
        Task testTask = Task.Run(async () => {
            while (!timer.IsCompleted) {
                test();
                // await Task.Delay(1000/144);
            }
            });

        await Task.WhenAny(timer, testTask);
        int count = counter;

        await Task.WhenAll(timer, testTask);
        testTask.Dispose();
        timer.Dispose();

        timer = Task.Delay(10000);
        testTask = Task.Run(async () => {
            while (!timer.IsCompleted) {
                Console.Clear();
                test();
                // await Task.Delay(1000/144);
            }
            });

        await Task.WhenAny(timer, testTask);
        int clrCount = counter;

        await Task.WhenAll(timer, testTask);
        testTask.Dispose();
        timer.Dispose();

        int charactersScreen = Console.WindowWidth * (Console.WindowHeight - 2);

        testResults.Add((type, count, clrCount, charactersScreen));
    }

    public static string Repeat(this string value, int count)
    {
        return new StringBuilder().Insert(0, value, count).ToString();
    }

    private static void TestOneBlock()
    {
        counter++;
        Console.SetCursorPosition(0, 0);
        Console.WriteLine((new string('.', Console.WindowWidth) + "\n").Repeat(Console.WindowHeight-2));
    }

    private static void TestOneBlock2()
    {
        counter++;
        Console.SetCursorPosition(0, 0);
        Console.WriteLine((new string('Ä', Console.WindowWidth) + "\n").Repeat(Console.WindowHeight-2));
    }

    private static void TestOneBlockRnd()
    {
        counter++;
        char rndChar = (char)('A' + new Random().Next(0, 26));
        Console.SetCursorPosition(0, 0);
        Console.WriteLine((new string(rndChar, Console.WindowWidth) + "\n").Repeat(Console.WindowHeight-2));
    }

    private static bool flipFlop = false;
    private static void TestOneBlockFlipFlop()
    {
        counter++;
        char rndChar = flipFlop ? '`' : 'B';
        flipFlop = !flipFlop;
        Console.SetCursorPosition(0, 0);
        Console.WriteLine((new string(rndChar, Console.WindowWidth) + "\n").Repeat(Console.WindowHeight-2));
    }

    private static void TestOneLine()
    {
        counter++;
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < Console.WindowHeight-2; i++)
        {
            Console.WriteLine(new string('A', Console.WindowWidth));
        }
    }

    private static void TestOneLineSet()
    {
        counter++;
        for (int i = 0; i < Console.WindowHeight-2; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.WriteLine(new string('A', Console.WindowWidth));
        }
    }

    private static void TestSideBox()
    {
        counter++;
        Console.SetCursorPosition(0, 0);
        string line = "A".PadRight(Console.WindowWidth-1, '-') + 'B';
        for (int i = 1; i < Console.WindowHeight-2; i++)
        {
            Console.WriteLine(line);
        }
    }

    private static void TestSideBoxSet()
    {
        counter++;
        for (int i = 1; i < Console.WindowHeight-2; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write('A');
            Console.SetCursorPosition(Console.WindowWidth-1, i);
            Console.Write('B');
        }
    }
}