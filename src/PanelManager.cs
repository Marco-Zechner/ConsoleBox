namespace MarcoZechner.ConsoleBox;

public class PanelManager
{   
    // static 
    private static PanelManager? activePanelManager;
    private static List<PanelManager> waitingManagers = new();
    public static readonly Lock RenderLock = new();
    private static bool stopSignal = false;
    private static bool subscribedToExit = false;

    // instance
    public bool IsRendering => activePanelManager == this;
    public PanelBase RootPanel {get; set;} = new DisplayPane(){
        Content = "No Root Panel Set"
    };
    public Action<ConsoleKeyInfo>? HandleInputMethod {get; set;}
    public Func<PanelBase, RenderBuffer, Task>? BeforeRender {get; set;} = null;
    public string? ExitReason {get; private set;} = null;
    private bool debug_ColorUpdates = false;
    public bool Debug_ColorUpdates {get; set;} = false;
    private bool instanceStopSignal = false;
    private ThreadState renderThread = ThreadState.Inactive;
    private RenderBuffer last = new(Console.WindowWidth, Console.WindowHeight); 
    private RenderBuffer current = new(Console.WindowWidth, Console.WindowHeight);


    private enum ThreadState {
        Inactive,
        Running,
        Waiting,
        Finished,
        Error,
    }

    public PanelManager() {
        HandleInputMethod = null;
    }
    
    public void Start() {
        stopSignal = false;
        instanceStopSignal = false;

        if (activePanelManager != null) {
            throw new Exception("Can't render 2 PanelManagers at the same time.\nAnother PanelManager is already active and needs to be stopped first.\nDid you perhaps wanted to add this PanelManager as a Panel to the other one instead of Starting it?");
        }

        if (renderThread == ThreadState.Waiting) {
            if (waitingManagers.Contains(this)) {
                waitingManagers.Remove(this);
            }
            activePanelManager = this;
            last = new(Console.WindowWidth, Console.WindowHeight);
            current = new(Console.WindowWidth, Console.WindowHeight);
            return;
        }

        renderThread = ThreadState.Inactive;

        if (!subscribedToExit) {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Stop();
            subscribedToExit = true;
        }

        activePanelManager = this;
        _ = Task.Run(() => RenderThread());
        InputThread();
        while (renderThread != ThreadState.Finished && renderThread != ThreadState.Inactive && renderThread != ThreadState.Error) {
            Task.Delay(1000/60);
        }
    }

    public void Pause() {
        activePanelManager = null;
        while (renderThread != ThreadState.Waiting) {
            Task.Delay(1000/60);
        }
    }

    public void Stop() {
        instanceStopSignal = true;
    }

    public static void StopAll() {
        stopSignal = true;
        activePanelManager = null;
        Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
        Console.WriteLine();
    }

    private async Task RenderThread() {
        renderThread = ThreadState.Running;
        last = new(Console.WindowWidth, Console.WindowHeight);
        current = new(Console.WindowWidth, Console.WindowHeight);
        try {
            while (instanceStopSignal == false && stopSignal == false) {
                if (!IsRendering) {
                    renderThread = ThreadState.Waiting;
                    if (waitingManagers.Contains(this) == false) {
                        waitingManagers.Add(this);
                    }
                    await Task.Delay(1000/60);
                    continue;
                } else {
                    renderThread = ThreadState.Running;
                }

                if (BeforeRender != null)
                    await BeforeRender.Invoke(RootPanel, current);

                RenderBuffer next = new(Console.WindowWidth, Console.WindowHeight, ' ');
                lock (RenderLock) {
                    RootPanel.Render(0, 0, Console.WindowWidth, Console.WindowHeight, next);
                }
                Console.CursorVisible = false;
                bool renderedLastTime = RenderBuffer.GetChanges(last, current, out RenderBuffer changes);
                if (debug_ColorUpdates != Debug_ColorUpdates && Debug_ColorUpdates) {
                    debug_ColorUpdates = Debug_ColorUpdates;
                    renderedLastTime = true;
                    changes = next;
                }
                if (Debug_ColorUpdates && renderedLastTime) {
                    last = current;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    changes.Render();
                }

                bool renderThisTime = RenderBuffer.GetChanges(current, next, out changes);
                if (debug_ColorUpdates != Debug_ColorUpdates && !Debug_ColorUpdates) {
                    debug_ColorUpdates = Debug_ColorUpdates;
                    renderThisTime = true;
                    changes = next;
                }
                if (renderThisTime) {
                    current = next;
                    Console.ForegroundColor = ConsoleColor.White;
                    changes.Render();
                }
                await Task.Delay(1000/ 60);
            }
        }
        catch(Exception e) {
            string exitReason = $"Exception:\n{e.Message}\n{e.StackTrace}";
            if (e.InnerException != null) {
                exitReason += $"\nInner Exception:\n{e.InnerException.Message}\n{e.InnerException.StackTrace}";
            }
            ExitReason = exitReason;
            renderThread = ThreadState.Error;
        }
        finally {
            Console.ForegroundColor = ConsoleColor.White;
            if (activePanelManager == this) {
                activePanelManager = null;
            }
            if (renderThread != ThreadState.Error)
            renderThread = ThreadState.Finished;
        }
    }

    private void InputThread() {
        while (stopSignal == false && instanceStopSignal == false && renderThread != ThreadState.Error && renderThread != ThreadState.Finished && HandleInputMethod != null) {
            if (!IsRendering) {
                Task.Delay(1000/60);
                continue;
            }
            if (Console.KeyAvailable == false) {
                Task.Delay(1000/60);
                continue;
            }
            ConsoleKeyInfo key = Console.ReadKey(true);
            HandleInputMethod?.Invoke(key);
        };
    }
}