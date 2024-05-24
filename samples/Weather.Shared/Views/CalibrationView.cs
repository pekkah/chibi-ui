using System.Collections.Generic;
using Chibi.Ui.Views;
using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace Chibi.Ui.Weather.Shared.Views;

public class CalibrationView : WeatherViewBase
{
    public CalibrationView(ICalibratableTouchscreen touchscreen, INavigationController navigation, IRenderingControl renderingControl)
    {
        Content = new DockPanel()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            LastChildFill = true,
            Background = new RectangleBrush()
            {
                Color = Color.Black,
                Filled = true
            },
            Children =
            {
                new CalibrationPanel(touchscreen, navigation, renderingControl)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                }
            }
        };
    }
}

public class CalibrationPanel: UiElement, IClickable
{
    private readonly ICalibratableTouchscreen _touchscreen;
    private readonly INavigationController _navigation;
    private readonly IRenderingControl _renderingControl;

    private readonly List<CalibrationPoint> _calibrationPoints = new();
    private Rect _point1;
    private Rect _point2;

    public CalibrationPanel(ICalibratableTouchscreen touchscreen, INavigationController navigation, IRenderingControl renderingControl)
    {
        _touchscreen = touchscreen;
        _navigation = navigation;
        _renderingControl = renderingControl;
    }

    public override void Render(IDrawingContext context, Rect bounds)
    {
        var px1 = bounds.X + 20;
        var py1 = bounds.Y + 20;
        
        var px2 = bounds.X + bounds.Width - 20;
        var py2 = bounds.Y + bounds.Height - 20;

        _point1 = new Rect(px1 - 1, py1 - 1, 3, 3);
        _point2 = new Rect(px2 - 1, py2 - 1, 3, 3);
        context.DrawText(new Point(bounds.X+bounds.Width / 2-50, bounds.Y+bounds.Height / 2-5), "Calibrate", new Font8x8(), Color.White);
        context.DrawRectangle(_point1, Color.White, true);
        context.DrawRectangle(_point2, Color.White, true);
        _renderingControl.Pause();
    }

    public void Click(HitTestResult? point)
    {
        if (point is null)
            return;

        var calPoint = _calibrationPoints.Count == 0 ? _point1 : _point2;

        var calDataPoint =
            new CalibrationPoint(point.HitPoint.RawX, calPoint.X + 1, point.HitPoint.RawY, calPoint.Y + 1);
        _calibrationPoints.Add(calDataPoint);
        Resolver.Log.Info($"Calibration point set to Raw: {calDataPoint.RawX},{calDataPoint.RawY} Screen: {calDataPoint.ScreenX},{calDataPoint.ScreenY}");

        if (_calibrationPoints.Count == 2)
        {
            Resolver.Log.Info("Calibration complete");
            _touchscreen.SetCalibrationData(_calibrationPoints);
            _renderingControl.Resume();
            _navigation.Navigate<MainView>();
        }
    }
}