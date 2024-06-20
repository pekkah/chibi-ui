using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chibi.Ui.Buffers;

public class Buffer<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Memory<byte> _memory;

    public Buffer(Memory<byte> memory, int width, int height)
    {
        _memory = memory;
        Width = width;
        Height = height;
    }

    public int Width { get; }

    public int Height { get; }

    public Span<byte> Span => _memory.Span;

    public void ProcessRows(PixelAccessorAction<TPixel> processPixels)
    {
        var pixelAccessor = new PixelAccessor<TPixel>(this);
        processPixels(pixelAccessor);
    }
}

public delegate void PixelAccessorAction<TPixel>(PixelAccessor<TPixel> pixelAccessor)
    where TPixel : unmanaged, IPixel<TPixel>;

public ref struct PixelAccessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Buffer<TPixel> _buffer;

    internal PixelAccessor(Buffer<TPixel> buffer)
    {
        _buffer = buffer;
    }

    public int Width => _buffer.Width;

    public int Height => _buffer.Height;

    public Span<TPixel> GetRowSpan(uint rowIndex)
    {
        ref var b0 = ref MemoryMarshal.GetReference(_buffer.Span);
        ref var e0 = ref Unsafe.As<byte, TPixel>(ref b0);
        e0 = ref Unsafe.Add(ref e0, (int)(rowIndex * Width));
        return MemoryMarshal.CreateSpan(ref e0, Width);
    }
}