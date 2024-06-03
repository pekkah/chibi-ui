using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chibi.Ui.DataBinding;
using Chibi.Ui.Weather.Shared.OpenMeteo;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Weather.Shared.Views;

public class Loader
{
    private readonly AssetManager _assets;
    private readonly LightweightSubject<ObservableCollection<UiElement>> _days = new();
    private readonly LightweightSubject<string?> _error = new();
    private readonly LightweightSubject<ObservableCollection<UiElement>> _hours = new();

    private readonly LightweightSubject<bool> _loading = new();

    public Loader(AssetManager assets)
    {
        _assets = assets;
        _loading.Next(true);
        _error.Next(null);
    }

    public IObservable<ObservableCollection<UiElement>> Hours => _hours;

    public IObservable<ObservableCollection<UiElement>> Days => _days;

    public IObservable<bool> Loading => _loading;
    public IObservable<string?> Error => _error;

    public async Task Load()
    {
        _loading.Next(true);
        _error.Next(null);

        try
        {
            Resolver.Log.Info("Loading weather data");
            var stopwatch = Stopwatch.StartNew();

            var httpClient = new HttpClient();
            // need to allow async in Load and Unload?
            var response = await httpClient.GetAsync(
                "https://api.open-meteo.com/v1/forecast?latitude=60.1695&longitude=24.9354&current=temperature_2m,relative_humidity_2m&hourly=temperature_2m&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=auto",
                HttpCompletionOption.ResponseContentRead);

            Resolver.Log.Info($"Got response: {stopwatch.Elapsed.TotalSeconds}s");

            stopwatch.Restart();
            Resolver.Log.Info(
                $"Before Weather data parsed ({GC.GetTotalMemory(false)})b");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var forecast = MeteoResponse.FromJsonMicro(bytes);
            response.Dispose();
            httpClient.Dispose();
            GC.Collect();
            Resolver.Log.Info(
                $"After Weather data parsed: {stopwatch.Elapsed.TotalSeconds}s ({GC.GetTotalMemory(false)})b");

            stopwatch.Restart();
            var hours = new ObservableCollection<UiElement>();
            var start = forecast.hourly.time
                .Select(DateTime.Parse)
                .ToList()
                .FindIndex(i => i.Hour == DateTime.Now.Hour);

            for (var i = start; i < start + 6; i++)
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
                    Background = new RectangleBrush
                    {
                        Color = Theme.SectionBackground.WithBrightness(0.4f),
                        Filled = false,
                        CornerRadius = 2
                    }
                });
            }

            _hours.Next(hours);


            var days = new ObservableCollection<UiElement>();

            for (var i = 0; i < 7; i++)
            {
                var day = DateTime.Parse(forecast.daily.time[i]).DayOfWeek;
                var max = forecast.daily.temperature_2m_max[i];
                var min = forecast.daily.temperature_2m_min[i];
                var condition = forecast.daily.weather_code[i];
                days.Add(new StackPanel
                {
                    Margin = new Thickness(2),
                    Height = 24,
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Children =
                    [
                        new TextBlock
                        {
                            Width = 115,
                            Text = day.ToString(),
                            Font = new Font12x16(),
                            Color = Color.White
                        },
                        new Image()
                        {
                            Source = _assets.FromResource<Loader>($"Chibi.Ui.Weather.Shared.Icons.sun.bmp"),
                            TransparencyColor = new Color(0,0,0,0)
                        },
                        new TextBlock
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

            _days.Next(days);
            _loading.Next(false);

            GC.Collect();
            Resolver.Log.Info(
                $"After load: {stopwatch.Elapsed.TotalSeconds}s ({GC.GetTotalMemory(false)})b");
        }
        catch (Exception e)
        {
            Resolver.Log.Error(e);
            _error.Next(e.Message);
        }
        finally
        {
            _loading.Next(false);
        }
    }
}