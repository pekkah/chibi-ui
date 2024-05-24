using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chibi.Ui.DataBinding;
using Meadow;

namespace Chibi.Ui.Views;

public interface IRenderingControl
{
    void Pause();
    void Resume();
}

public abstract class ViewManagerBase<TViewBase> : ObservableObject, IRenderingControl, INavigationController, IRenderingDetails where TViewBase: IViewController
{
    private readonly Color _clearColor;
    private bool _paused;

    protected ViewManagerBase(IGraphicsDevice graphicsDevice, int maxFps, Color clearColor)
    {
        _clearColor = clearColor;
        Fps = Property(nameof(Fps), 0);
        MaxFps = maxFps;
        Renderer = new Renderer(graphicsDevice);
    }

    protected Stack<TViewBase> ViewStack { get; set; } = new();

    public TViewBase CurrentView => ViewStack.Peek();

    public Renderer Renderer { get; }

    public int MaxFps { get; }

    public ReactiveProperty<int> Fps { get; }

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }

    public void Navigate<T>() where T: IViewController
    {
        Resolver.Log.Info($"Navigating to {typeof(T).Name}");
        Pause();
        if (Resolver.Services.GetOrCreate<T>() is not TViewBase view)
            throw new InvalidOperationException($"View {typeof(T).Name} does not implement {typeof(TViewBase).Name}");

        if (ViewStack.TryPop(out var previousView))
        {
            previousView.Unload();
        }

        view.AllocatedSize = Renderer.DeviceBounds.Size;
        view.Load();
        ViewStack.Push(view);
        view.Loaded();
        Resume();
    }

    public void Draw()
    {
        Renderer.Clear(_clearColor);
        CurrentView.Render(Renderer);
        Renderer.Show();
        if (_paused)
        {
            while (_paused)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }
    }

    public void Start(CancellationToken cancellationToken)
    {
        var ticksPerSecond = Stopwatch.Frequency;
        var ticksPerFrame = ticksPerSecond / MaxFps;
        var ticksPerMillisecond = ticksPerSecond / 1000;

        _ = Task.Factory.StartNew(async () =>
        {
            var nextFrameTicks = Stopwatch.GetTimestamp();
            long frameCount = 0;
            var lastFpsUpdateTicks = nextFrameTicks;

            while (!cancellationToken.IsCancellationRequested)
            {
                Draw();
                nextFrameTicks += ticksPerFrame;

                frameCount++;
                if (Stopwatch.GetTimestamp() - lastFpsUpdateTicks >= ticksPerSecond)
                {
                    Fps.Value = (int)frameCount;
                    frameCount = 0;
                    lastFpsUpdateTicks = Stopwatch.GetTimestamp();
                    GC.Collect();
                }

                var delayTicks = nextFrameTicks - Stopwatch.GetTimestamp();
                if (delayTicks > 0)
                {
                    var delayMilliseconds = (int)(delayTicks / ticksPerMillisecond);
                    await Task.Delay(delayMilliseconds, CancellationToken.None);
                }
            }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously);
    }
}