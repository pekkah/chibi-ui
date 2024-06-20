using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chibi.Ui.Buffers;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct Rgba32(byte r, byte g, byte b, byte a)
    : IPixel<Rgba32>
{
    public byte R = r;

    public byte G = g;

    public byte B = b;


    public byte A = a;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32(byte r, byte g, byte b) : this(r, g, b, byte.MaxValue)
    {
    }

    public uint Rgba
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => Unsafe.As<Rgba32, uint>(ref Unsafe.AsRef(in this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Unsafe.As<Rgba32, uint>(ref this) = value;
    }

    public Rgb24 Rgb
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => new(R, G, B);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            R = value.R;
            G = value.G;
            B = value.B;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Rgba32 left, Rgba32 right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Rgba32 left, Rgba32 right)
    {
        return !left.Equals(right);
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Rgba32 rgba32 && Equals(rgba32);
    }


    public readonly bool Equals(Rgba32 other)
    {
        return Rgba.Equals(other.Rgba);
    }

    public readonly override string ToString()
    {
        return $"Rgba32({R}, {G}, {B}, {A})";
    }

    public readonly override int GetHashCode()
    {
        return Rgba.GetHashCode();
    }
}