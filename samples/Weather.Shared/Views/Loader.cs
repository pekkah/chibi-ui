using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Chibi.Ui.DataBinding;
using Chibi.Ui.Geometry;
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
                        /*new Circle()
                        {
                            Width = 18,
                            Filled = true,
                            Color = WeatherColorMapper.GetColorForWmoCode(condition),
                            Margin = new Thickness(2),
                        },*/
                        new Image()
                        {
                            Source = _assets.FromResource<Loader>("Chibi.Ui.Weather.Shared.Icons.sunny_color.jpg"),
                            Width = 18,
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
public static class WeatherColorMapper
{
    public static readonly Dictionary<int, Color> WmoCodeToColor = new Dictionary<int, Color>
    {
        { 0, new Color(255, 223, 0, 255) },       // Sunny: Yellow
        { 1, new Color(255, 223, 0, 255) },       // Mainly Sunny: Yellow
        { 2, new Color(135, 206, 250, 255) },     // Partly Cloudy: Light Blue
        { 3, new Color(192, 192, 192, 255) },     // Cloudy: Gray
        { 45, new Color(169, 169, 169, 255) },    // Foggy: Dark Gray
        { 48, new Color(169, 169, 169, 255) },    // Rime Fog: Dark Gray
        { 51, new Color(173, 216, 230, 255) },    // Light Drizzle: Light Blue
        { 53, new Color(100, 149, 237, 255) },    // Drizzle: Cornflower Blue
        { 55, new Color(30, 144, 255, 255) },     // Heavy Drizzle: Dodger Blue
        { 56, new Color(176, 224, 230, 255) },    // Light Freezing Drizzle: Powder Blue
        { 57, new Color(70, 130, 180, 255) },     // Freezing Drizzle: Steel Blue
        { 61, new Color(173, 216, 230, 255) },    // Light Rain: Light Blue
        { 63, new Color(100, 149, 237, 255) },    // Rain: Cornflower Blue
        { 65, new Color(30, 144, 255, 255) },     // Heavy Rain: Dodger Blue
        { 66, new Color(176, 224, 230, 255) },    // Light Freezing Rain: Powder Blue
        { 67, new Color(70, 130, 180, 255) },     // Freezing Rain: Steel Blue
        { 71, new Color(255, 250, 250, 255) },    // Light Snow: Snow
        { 73, new Color(240, 248, 255, 255) },    // Snow: Alice Blue
        { 75, new Color(220, 220, 220, 255) },    // Heavy Snow: Gainsboro
        { 77, new Color(245, 245, 245, 255) },    // Snow Grains: White Smoke
        { 80, new Color(173, 216, 230, 255) },    // Light Showers: Light Blue
        { 81, new Color(100, 149, 237, 255) },    // Showers: Cornflower Blue
        { 82, new Color(30, 144, 255, 255) },     // Heavy Showers: Dodger Blue
        { 85, new Color(255, 250, 250, 255) },    // Light Snow Showers: Snow
        { 86, new Color(240, 248, 255, 255) },    // Snow Showers: Alice Blue
        { 95, new Color(255, 0, 0, 255) },        // Thunderstorm: Red
        { 96, new Color(255, 69, 0, 255) },       // Light Thunderstorms With Hail: Orange Red
        { 99, new Color(139, 0, 0, 255) }         // Thunderstorm With Hail: Dark Red
    };

    public static Color GetColorForWmoCode(int wmoCode)
    {
        if (WmoCodeToColor.TryGetValue(wmoCode, out var code))
        {
            Resolver.Log.Info($"WMO code : {wmoCode} mapped to matching code {code}");
            return code;
        }

        // Find the closest WMO code
        int closestKey = WmoCodeToColor.Keys.OrderBy(key => Math.Abs(key - wmoCode)).First();
        Resolver.Log.Info($"WMO code : {wmoCode} mapped to closest matching code {closestKey}");
        return WmoCodeToColor[closestKey];
    }
}