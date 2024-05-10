using System;

namespace Chibi.Ui;

public readonly struct GridLength
{
    public enum GridUnitType
    {
        Pixel,
        Percentage,
        Auto
    }

    public int Value { get; }
    public GridUnitType UnitType { get; }

    private GridLength(int value, GridUnitType unitType)
    {
        Value = value;
        UnitType = unitType;
    }

    public static GridLength Pixels(int pixels)
    {
        return new GridLength(pixels, GridUnitType.Pixel);
    }

    public static GridLength Percentage(int percentage)
    {
        return new GridLength(percentage, GridUnitType.Percentage);
    }

    public static GridLength Auto => new(0, GridUnitType.Auto);

    public int? ToPixels(int totalPixels)
    {
        return UnitType switch
        {
            GridUnitType.Pixel => Value,
            GridUnitType.Percentage => Value * totalPixels / 100,
            GridUnitType.Auto => null,
            _ => throw new InvalidOperationException("Invalid unit type.")
        };
    }

    public static GridLength operator +(GridLength a, GridLength b)
    {
        if (a.UnitType != b.UnitType)
            throw new InvalidOperationException("Cannot add GridLengths of different unit types.");

        return new GridLength(a.Value + b.Value, a.UnitType);
    }

    public static GridLength operator -(GridLength a, GridLength b)
    {
        if (a.UnitType != b.UnitType)
            throw new InvalidOperationException("Cannot subtract GridLengths of different unit types.");

        return new GridLength(a.Value - b.Value, a.UnitType);
    }

    public static bool TryParse(ReadOnlySpan<char> s, out GridLength result)
    {
        result = default;

        if (s.Length == 0)
            return false;

        if (s.EndsWith("px".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(s.Slice(0, s.Length - 2), out var pixels))
            {
                result = Pixels(pixels);
                return true;
            }
        }
        else if (s.EndsWith("%".AsSpan()))
        {
            if (int.TryParse(s.Slice(0, s.Length - 1), out var percentage))
            {
                result = Percentage(percentage);
                return true;
            }
        }
        else if (s.Equals("auto".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            result = Auto;
            return true;
        }

        return false;
    }

    public static bool TryParse(string s, out GridLength result)
    {
        return TryParse(s.AsSpan(), out result);
    }

    public static implicit operator GridLength(string s)
    {
        if (TryParse(s, out var result)) return result;

        throw new FormatException($"Invalid GridLength format {s}.");
    }
}
