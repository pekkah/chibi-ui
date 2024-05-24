using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace Chibi.Ui;

public class Renderer(IGraphicsDevice graphicsDevice)
{
    public Rect DeviceBounds => new Rect(0, 0, graphicsDevice.Width, graphicsDevice.Height);

    public IGraphicsDevice GraphicsDevice => graphicsDevice;

    public void Render(UiElement element)
    {
        var context = graphicsDevice.CreateDrawingContext();

        //var stopwatch = Stopwatch.StartNew();

        RenderElement(element, context, element.Bounds);

        //Resolver.Log.Info($"Render: {stopwatch.Elapsed.TotalMilliseconds}ms");

        static void RenderElement(UiElement element, IDrawingContext context, Rect bounds)
        {
            element.Render(context, bounds);
            var count = element.GetChildCount();
            for (var i = 0; i < count; i++)
            {
                var child = element.GetChild(i);
                var childBounds = child.Bounds.Translate(new Point(bounds.X, bounds.Y));
                RenderElement(child, context, childBounds);
            }
        }
    }

    public HitTestResult? HitTest(UiElement element, TouchPoint point)
    {
        List<HitTestResult> hitElements = [];
        HitTestElement(element, point, element.Bounds, hitElements);

        return hitElements.LastOrDefault();

        static void HitTestElement(UiElement element, TouchPoint hitPoint, Rect parentBounds, List<HitTestResult> hitElements)
        {
            var elementBounds = element.Bounds.Translate(parentBounds.Position);

            if (!elementBounds.Contains(new Point(hitPoint.ScreenX, hitPoint.ScreenY)))
            {
                return;
            }

            var elementHitPoint = new HitTestResult()
            {
                Element = element, 
                HitPoint = hitPoint,
                LocalPoint = new Point(hitPoint.ScreenX - elementBounds.X, hitPoint.ScreenY - elementBounds.Y)
            };
    
            hitElements.Add(elementHitPoint);

            var count = element.GetChildCount();
            for (var i = 0; i < count; i++)
            {
                var child = element.GetChild(i);
                HitTestElement(child, hitPoint, elementBounds, hitElements);
            }
        }
    }

    public virtual void Show()
    {
        graphicsDevice.Show();
    }

    public virtual void Clear(Color clearColor)
    {
        graphicsDevice.Clear(clearColor);
    }
}

public class HitTestResult
{
    public UiElement Element { get; set; }

    public TouchPoint HitPoint { get; set; }

    public Point LocalPoint { get; set; }
}