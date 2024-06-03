using System;
using System.Collections.ObjectModel;
using Chibi.Ui.DataBinding;
using Chibi.Ui.Views;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Weather.Shared.Views;

public class MainView : WeatherViewBase
{
    private readonly Loader _loader;

    public MainView(IRenderingDetails details, AssetManager assets)
    {
        _loader = new Loader(assets);
        TimeUntilNextUpdate = Property(nameof(TimeUntilNextUpdate), TimeSpan.FromSeconds(0));
        Content = new DockPanel
        {
            LastChildFill = true,
            Children =
            [
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Attach =
                    [
                        DockPanel.SetDock(Dock.Top)
                    ],
                    Children =
                    [
                        new TextBlock
                        {
                            Margin = new Thickness(3, 2, 5, 3),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Text = "Chibi.Ui Weather".ToUpperInvariant(),
                            Font = new Font12x16(),
                            Color = Theme.ScreenTitle
                        },
                        new TextBlock
                        {
                            Margin = new Thickness(0, 2, 3, 3),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            TextProperty =
                            {
                                details.Fps.Select(i => $"{i}")
                            },
                            Color = Color.DarkRed,
                            Font = new Font6x8()
                        }
                    ]
                },
                new UniformGrid()
                {
                    Columns = 2,
                    Rows = 1,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = new RectangleBrush
                    {
                        Color = Theme.SectionBackground,
                        Filled = true
                    },
                    Children =
                    [
                        new TextBlock
                        {
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            VerticalContentAlignment = VerticalAlignment.Top,
                            Height = 20,
                            Padding = new Thickness(2),
                            Color = Color.White,
                            Font = new Font12x16(),
                            TextProperty =
                            {
                                Loading.Select(i => i ? "Loading..." : "Next 6h".ToUpperInvariant())
                                    .CombineLatest(
                                        Error,
                                        (loading, error) => !string.IsNullOrEmpty(error) ? error : loading
                                    )
                            }
                        },
                        new TextBlock
                        {
                            HorizontalContentAlignment = HorizontalAlignment.Right,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 30, 0),
                            Height = 20,
                            Width = 40,
                            Padding = new Thickness(2),
                            Color = Color.White,
                            Font = new Font12x16(),
                            TextProperty =
                            {
                                TimeUntilNextUpdate.Select(i => $"{(int)i.TotalSeconds}s")
                            }
                        }
                    ]
                },
                new UniformGrid
                {
                    Background = new RectangleBrush
                    {
                        Color = Theme.SectionBackground,
                        Filled = true
                    },
                    Columns = 3,
                    ChildrenProperty =
                    {
                        Hours
                    }
                },
                new UniformGrid
                {
                    Background = new RectangleBrush
                    {
                        Color = Color.FromHex("#0089c4"),
                        Filled = true
                    },
                    Columns = 1,
                    Rows = 7,
                    ChildrenProperty =
                    {
                        Days
                    }
                }
            ]
        };
    }

    private DateTime _lastUpdate;

    private void Update()
    {
        var now = DateTime.Now;
        var elapsed = now - _lastUpdate;
        TimeUntilNextUpdate.Value = TimeUntilNextUpdate.Value.Subtract(elapsed);

        if (TimeUntilNextUpdate.Value.TotalSeconds <= 0)
        {
            Resolver.Log.Info("Updateding...");
            _ = _loader.Load();
            TimeUntilNextUpdate.Value = TimeSpan.FromMinutes(5);
        }

        _lastUpdate = now;
    }

    public override void Render(Renderer renderer)
    {
        Update();
        base.Render(renderer);
    }

    public ReactiveProperty<TimeSpan> TimeUntilNextUpdate { get; }

    public IObservable<ObservableCollection<UiElement>> Hours => _loader.Hours;

    public IObservable<ObservableCollection<UiElement>> Days => _loader.Days;

    public IObservable<bool> Loading => _loader.Loading;

    public IObservable<string?> Error => _loader.Error;

    public override void Loaded()
    {
        base.Loaded();
    }

    public override void OnLeft()
    {
        FocusManager.Previous();
    }

    public override void OnRight()
    {
        FocusManager.Next();
    }

    public override void OnEnter()
    {
        var clickable = FocusManager.Current as IClickable;
        clickable?.Click(null);
    }
}