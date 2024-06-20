using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chibi.Ui.Buffers;

[StructLayout(LayoutKind.Explicit)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct Rgb24(byte r, byte g, byte b) : IPixel<Rgb24>
{
    [FieldOffset(0)] public byte R = r;

    [FieldOffset(1)] public byte G = g;

    [FieldOffset(2)] public byte B = b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Rgb24 left, Rgb24 right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Rgb24 left, Rgb24 right)
    {
        return !left.Equals(right);
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Rgb24 other && Equals(other);
    }


    public readonly bool Equals(Rgb24 other)
    {
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, B, G);
    }


    public readonly override string ToString()
    {
        return $"Rgb24({R}, {G}, {B})";
    }
}