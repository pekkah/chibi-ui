using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Chibi.Ui.DataBinding;
using Meadow;
using Meadow.Peripherals.Displays;

namespace Chibi.Ui;

public class Image : UiElement
{
    public Image()
    {
        SourceProperty = Property(nameof(Source), default(ImageData));
        TransparencyColorProperty = Property(nameof(TransparencyColor), default(Color?));
        SourceProperty.Subscribe(ev =>
        {
            Width = ev.Value?.Buffer?.Width ?? 0;
            Height = ev.Value?.Buffer?.Height ?? 0;
            InvalidateMeasure();
        });
        AffectsRender<Color?>(TransparencyColorProperty);
    }

    public ReactiveProperty<Color?> TransparencyColorProperty { get; }

    public Color? TransparencyColor
    {
        get => TransparencyColorProperty.Value;
        set => TransparencyColorProperty.Value = value;
    }

    public ReactiveProperty<ImageData?> SourceProperty { get; }

    public ImageData? Source
    {
        get => SourceProperty.Value;
        set => SourceProperty.Value = value;
    }

    public override void Render(IDrawingContext context, Rect bounds)
    {
        base.Render(context, bounds);

        if (Source?.Buffer is null)
        {
            return;
        }

        //todo: something weird with the image colors here
        context.DrawBuffer(bounds.Position, Source.Buffer, TransparencyColor);
    }
}

public class AssetManager(IGraphicsDevice graphicsDevice)
{
    private readonly Dictionary<string, ImageData> _imagesAsResources = new();

    public ImageData FromResource<T>(string name)
    {
        if (_imagesAsResources.TryGetValue(name, out var imageData)) return imageData;

        using var stream = typeof(T).Assembly.GetManifestResourceStream(name);
        var image = Meadow.Foundation.Graphics.Image.LoadFromStream(stream);
        var deviceBuffer = graphicsDevice.CreateBuffer(image.Width, image.Height);
        deviceBuffer.WriteBuffer(0, 0,
            image.DisplayBuffer ?? throw new InvalidOperationException("Image pixel buffer is null"));

        var data = new ImageData(name, deviceBuffer);
        _imagesAsResources.Add(name, data);

        var pixel0 = deviceBuffer.GetPixel(0, 0);
        return data;
    }
}

public record ImageData(string Path, IPixelBuffer Buffer);