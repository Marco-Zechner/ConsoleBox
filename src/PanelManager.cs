
namespace MarcoZechner.ConsoleBox;

public class PanelManager : PanelBase
{   
    // static 
    public static bool IsRendering => activePanelManager != null;
    private static PanelManager? activePanelManager;
    public static readonly Lock RenderLock = new();
    private static bool stopSignal = false;
    private static bool subscribedToExit = false;

    // instance
    public PanelBase RootPanel {get; set;} = new DisplayPane(){
        Content = "No Root Panel Set\n\t\tdfv\n\t\tdfbdf\n\t\tdfv\n\t\tdfbdf\n\t\tdfv\n\t\tdfbdf\n\t\tdfv\n\t\tdfbdf\n\t\tdfv\n\t\tdfbdf\n\t\tdfv\n\t\tdfbdf\n\t\tdfv\n\t\tdfbdf"
    };
    private readonly Action<ConsoleKeyInfo>? handleInputMethod;
    public string? ExitReason {get; private set;} = null;
    private bool Debug_ColorUpdates {get; set;} = true;

    public PanelManager() {
        handleInputMethod = null;
    }

    public PanelManager(Action<ConsoleKeyInfo> handleInputMethod) {
        this.handleInputMethod = handleInputMethod;
    }

    public override void Render(int top, int left, int width, int height, RenderBuffer buffer) 
    => RootPanel.Render(top, left, width, height, buffer);

    public void Start() {
        StartRenderThread(this, handleInputMethod);
        while (!IsRendering) {
            Task.Delay(20);
        }
    }

    public static void Stop() {
        stopSignal = true;
            activePanelManager = null;
        lock (RenderLock) {
        }
        Console.Clear();
    }

    public static void StartRenderThread(
        PanelManager panelManager,
        Action<ConsoleKeyInfo>? handleInput = null) 
    {
        if (activePanelManager != null) {
            throw new Exception("Can't render 2 PanelManagers at the same time.\nAnother PanelManager is already active and needs to be stopped first.\nDid you perhaps wanted to add this PanelManager as a Panel to the other one instead of Starting it?");
        }

        if (!subscribedToExit) {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Stop();
            subscribedToExit = true;
        }

        _ = Task.Run(() => RenderThread(panelManager));
        var inputMethod = handleInput ?? panelManager.handleInputMethod;
        if (inputMethod != null) {
            InputThread(inputMethod);
        }
    }

    private static async Task RenderThread(PanelManager panelManager) {
        activePanelManager = panelManager;
        RenderBuffer last = new(Console.WindowWidth, Console.WindowHeight); 
        RenderBuffer current = new(Console.WindowWidth, Console.WindowHeight);
        try {
            while (IsRendering) {
                RenderBuffer next = new(Console.WindowWidth, Console.WindowHeight, ' ');
                lock (RenderLock) {
                    panelManager.RootPanel.Render(0, 0, Console.WindowWidth, Console.WindowHeight, next);
                }
                Console.CursorVisible = false;
                if (panelManager.Debug_ColorUpdates && RenderBuffer.GetChanges(last, current, out RenderBuffer changes)) {
                    last = current;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    changes.Render();
                }
                if (RenderBuffer.GetChanges(current, next, out changes)) {
                    current = next;
                    if (panelManager.Debug_ColorUpdates)
                        Console.ForegroundColor = ConsoleColor.White;
                    changes.Render();
                }
                await Task.Delay(1000/ 60);
            }
        }
        catch(Exception e) {
            string exitReason = $"Exception:\n{e.Message}";
            if (e.InnerException != null) {
                exitReason += $"\nInner Exception:\n{e.InnerException.Message}";
            }
            activePanelManager.ExitReason = exitReason;
        }
        finally {
            Console.ForegroundColor = ConsoleColor.White;
            activePanelManager = null;
        }
    }

    private static void InputThread(Action<ConsoleKeyInfo>? handleInput) {
        while(!IsRendering) {
            Task.Delay(10);
        }
        while (IsRendering) {
            ConsoleKeyInfo key = Console.ReadKey(true);
            lock (RenderLock) {
                handleInput?.Invoke(key);
            }
            if (stopSignal) return;
        }
    }
}