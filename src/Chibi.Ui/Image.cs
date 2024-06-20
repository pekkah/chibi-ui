using System;
using System.Collections.Generic;
using Chibi.Ui.DataBinding;
using Meadow;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Peripherals.Displays;
using SimpleJpegDecoder;

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

        if (Source?.Buffer is null) return;

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

        if (!name.EndsWith(".jpg"))
            throw new InvalidOperationException("Only JPEG images are supported");

        using var stream = typeof(T).Assembly.GetManifestResourceStream(name);

        if (stream is null)
            throw ResourceNotFound(name);

        var decoder = new JpegDecoder();
        var jpg = decoder.DecodeJpeg(stream);

        var imageBuffer = new BufferRgb888(decoder.Width, decoder.Height, jpg);

        // convert to native buffer supported by the graphics device
        var deviceBuffer = graphicsDevice.CreateBuffer(decoder.Width, decoder.Height);
        deviceBuffer.WriteBuffer(0, 0, imageBuffer);

        var data = new ImageData(name, deviceBuffer);
        _imagesAsResources.Add(name, data);

        return data;

        static Exception ResourceNotFound(string name)
        {
            var resources = typeof(T).Assembly.GetManifestResourceNames();
            return new InvalidOperationException(
                $"Resource '{name}' not found. Available resources: {string.Join(", ", resources)}");
        }
    }
}

public record ImageData(string Path, IPixelBuffer Buffer);