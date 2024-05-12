using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chibi.Ui.DataBinding;
using Chibi.Ui.Views;
using Chibi.Weather.Shared.Views;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Weather.Shared.Views;

public class MainView : WeatherViewBase
{
    public MainView(IRenderingDetails details)
    {
        Hours = new ReactiveProperty<ObservableCollection<UiElement>>("Hours", []);
        Root = new DockPanel
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
                            Margin = new Thickness(3, 5, 30, 3),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Text = "Chibi.Ui - Weather",
                            Font = new Font8x12()
                        },
                        new TextBlock
                        {
                            Margin = new Thickness(3, 5, 3, 3),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            TextProperty =
                            {
                                details.Fps.Select(i => $"FPS: {i}")
                            },
                            Color = Color.DarkRed,
                            Font = new Font6x8()
                        }
                    ]
                },
                new UniformGrid
                {
                    Columns = 3,
                    ChildrenProperty =
                    {
                        Hours
                    }
                }
            ]
        };
    }

    public ReactiveProperty<ObservableCollection<UiElement>> Hours { get; }

    public override void Load()
    {
        var hours = new ObservableCollection<UiElement>();
        foreach (var hour in Enumerable.Range(DateTime.Now.Hour, 6))
        {
            hours.Add(new HeaderContentControl
            {
                Header = $"{hour}:00",
                Value = "20°C"
            });
        }

        Hours.Value = new ObservableCollection<UiElement>(hours);
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
        clickable?.Click();
    }
}