using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Chibi.Ui.DataBinding;
using Chibi.Ui.Views;
using Chibi.Ui.Weather.Shared.OpenMeteo;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Weather.Shared.Views;

public class MainView : WeatherViewBase
{
    public MainView(IRenderingDetails details)
    {
        Hours = new ReactiveProperty<ObservableCollection<UiElement>>("Hours", []);
        Days = new ReactiveProperty<ObservableCollection<UiElement>>("Days", []);
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
                            Margin = new Thickness(3, 5, 5, 3),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Text = "Chibi.Ui Weather".ToUpperInvariant(),
                            Font = new Font12x16(),
                            Color = Theme.ScreenTitle
                        },
                        new TextBlock
                        {
                            Margin = new Thickness(0, 5, 3, 3),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            TextProperty =
                            {
                                details.Fps.Select(i => $"FPS:{i}")
                            },
                            Color = Color.DarkRed,
                            Font = new Font6x8()
                        }
                    ]
                },
                new TextBlock()
                {
                    Height = 24,
                    Padding = new Thickness(2),
                    Background = new RectangleBrush()
                    {
                        Color = Theme.SectionBackground,
                        Filled = true
                    },
                    Color = Color.White,
                    Font = new Font12x16(),
                    Text = "Next 6 Hours".ToUpperInvariant(),
                },
                new UniformGrid
                {
                    Background = new RectangleBrush()
                    {
                        Color = Theme.SectionBackground,
                        Filled = true
                    },
                    Columns = 3,
                    Rows = 2,
                    ChildrenProperty =
                    {
                        Hours
                    }
                },
                new UniformGrid()
                {
                    Background = new RectangleBrush()
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

    public ReactiveProperty<ObservableCollection<UiElement>> Hours { get; }

    public ReactiveProperty<ObservableCollection<UiElement>> Days { get; }

    public override void Load()
    {
        using var httpClient = new HttpClient();

        // need to allow async in Load and Unload?
        using var response = httpClient.GetAsync(
            "https://api.open-meteo.com/v1/forecast?latitude=60.1695&longitude=24.9354&current=temperature_2m,relative_humidity_2m&hourly=temperature_2m&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=auto",
            HttpCompletionOption.ResponseHeadersRead)
            .Result;

        var forecastString = response.Content.ReadAsStringAsync().Result;
        var forecast = Utf8Json.JsonSerializer.Deserialize<MeteoResponse>(forecastString);

        var hours = new List<UiElement>();
        var start = forecast.hourly.time
            .Select(DateTime.Parse)
            .ToList()
            .FindIndex(i => i.Hour == DateTime.Now.Hour);

        for(int i = start; i < start + 6 ; i++)
        {
            var hour = DateTime.Parse(forecast.hourly.time[i]).Hour;
            var temp = forecast.hourly.temperature_2m[i];
            hours.Add(new HeaderContentControl
            {
                Height = 50,
                HeaderFontColor = Theme.SectionTitle,
                ValueFontColor = Color.White,
                ValueFont = new Font12x16(),
                Header = $"{hour}:00",
                Value = $"{temp:F1}{forecast.hourly_units.temperature_2m}",
                Background = new RectangleBrush()
                {
                    Color = Theme.SectionBackground.WithBrightness(0.4f),
                    Filled = false,
                    CornerRadius = 2
                }
            });
        }

        Hours.Value = new ObservableCollection<UiElement>(hours);


        var days = new List<UiElement>();

        for (int i = 0; i < 7; i++)
        {
            var day = DateTime.Parse(forecast.daily.time[i]).DayOfWeek;
            var max = forecast.daily.temperature_2m_max[i];
            var min = forecast.daily.temperature_2m_min[i];
            days.Add(new StackPanel()
            {
                Margin = new Thickness(2),
                Height = 24,
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                [
                    new TextBlock()
                    {
                        Width = 115,
                        Text = day.ToString(),
                        Font = new Font12x16(),
                        Color = Color.White,
                    },
                    new TextBlock()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Height = 24,
                        Text = $"{max:F1}-{min:F1}{forecast.daily_units.temperature_2m_min}",
                        Font = new Font8x12(),
                        Color = Color.White
                    }
                ]
            });
        }    

        Days.Value = new ObservableCollection<UiElement>(days);
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


