using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Microsoft.Extensions.Primitives;
using Vector = Meadow.Foundation.Spatial.Vector;
#if !BUILDTASK
#endif

namespace Chibi.Ui;

public readonly struct Size : IEquatable<Size>
{
    /// <summary>
    /// A size representing infinity.
    /// </summary>
    public static readonly Size Infinity = new Size(
        int.MaxValue,
        int.MaxValue
        );

    /// <summary>
    /// The width.
    /// </summary>
    private readonly int _width;

    /// <summary>
    /// The height.
    /// </summary>
    private readonly int _height;

    /// <summary>
    /// Initializes a new instance of the <see cref="Size"/> structure.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Size(int width, int height)
    {
        _width = width;
        _height = height;
    }

    /// <summary>
    /// Gets the aspect ratio of the size.
    /// </summary>
    public int AspectRatio => _width / _height;

    /// <summary>
    /// Gets the width.
    /// </summary>
    public int Width => _width;

    /// <summary>
    /// Gets the height.
    /// </summary>
    public int Height => _height;

    /// <summary>
    /// Checks for equality between two <see cref="Size"/>s.
    /// </summary>
    /// <param name="left">The first size.</param>
    /// <param name="right">The second size.</param>
    /// <returns>True if the sizes are equal; otherwise false.</returns>
    public static bool operator ==(Size left, Size right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks for inequality between two <see cref="Size"/>s.
    /// </summary>
    /// <param name="left">The first size.</param>
    /// <param name="right">The second size.</param>
    /// <returns>True if the sizes are unequal; otherwise false.</returns>
    public static bool operator !=(Size left, Size right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static Size operator *(Size size, Vector2 scale)
    {
        return new Size(
            (int)(size._width * scale.X),
            (int)(size._height * scale.Y)
            );
    }

    /// <summary>
    /// Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static Size operator /(Size size, Vector2 scale)
    {
        return new Size(
            (int)(size._width / scale.X),
            (int)(size._height / scale.Y)
            );
    }

    /// <summary>
    /// Divides a size by another size to produce a scaling factor.
    /// </summary>
    /// <param name="left">The first size</param>
    /// <param name="right">The second size.</param>
    /// <returns>The scaled size.</returns>
    public static Vector2 operator /(Size left, Size right)
    {
        return new Vector2(
            left._width / right._width,
            left._height / right._height
            );
    }

    /// <summary>
    /// Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static Size operator *(Size size, int scale)
    {
        return new Size(size._width * scale, size._height * scale);
    }

    /// <summary>
    /// Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static Size operator /(Size size, int scale)
    {
        return new Size(size._width / scale, size._height / scale);
    }

    public static Size operator +(Size size, Size toAdd)
    {
        return new Size(size._width + toAdd._width, size._height + toAdd._height);
    }

    public static Size operator -(Size size, Size toSubtract)
    {
        return new Size(size._width - toSubtract._width, size._height - toSubtract._height);
    }

    /// <summary>
    /// Constrains the size.
    /// </summary>
    /// <param name="constraint">The size to constrain to.</param>
    /// <returns>The constrained size.</returns>
    public Size Constrain(Size constraint)
    {
        return new Size(
            Math.Min(_width, constraint._width),
            Math.Min(_height, constraint._height));
    }

    /// <summary>
    /// Deflates the size by a <see cref="Thickness"/>.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    /// <returns>The deflated size.</returns>
    /// <remarks>The deflated size cannot be less than 0.</remarks>
    public Size Deflate(Thickness thickness)
    {
        return new Size(
            Math.Max(0, _width - thickness.Left - thickness.Right),
            Math.Max(0, _height - thickness.Top - thickness.Bottom));
    }

    /// <summary>
    /// Returns a boolean indicating whether the size is equal to the other given size (bitwise).
    /// </summary>
    /// <param name="other">The other size to test equality against.</param>
    /// <returns>True if this size is equal to other; False otherwise.</returns>
    public bool Equals(Size other)
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return _width == other._width &&
               _height == other._height;
        // ReSharper enable CompareOfFloatsByEqualityOperator
    }

    /// <summary>
    /// Checks for equality between a size and an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// True if <paramref name="obj"/> is a size that equals the current size.
    /// </returns>
    public override bool Equals(object? obj) => obj is Size other && Equals(other);

    /// <summary>
    /// Returns a hash code for a <see cref="Size"/>.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Width.GetHashCode();
            hash = hash * 23 + Height.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// Inflates the size by a <see cref="Thickness"/>.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    /// <returns>The inflated size.</returns>
    public Size Inflate(Thickness thickness)
    {
        return new Size(
            _width + thickness.Left + thickness.Right,
            _height + thickness.Top + thickness.Bottom);
    }

    /// <summary>
    /// Returns a new <see cref="Size"/> with the same height and the specified width.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <returns>The new <see cref="Size"/>.</returns>
    public Size WithWidth(int width)
    {
        return new Size(width, _height);
    }

    /// <summary>
    /// Returns a new <see cref="Size"/> with the same width and the specified height.
    /// </summary>
    /// <param name="height">The height.</param>
    /// <returns>The new <see cref="Size"/>.</returns>
    public Size WithHeight(int height)
    {
        return new Size(_width, height);
    }

    /// <summary>
    /// Returns the string representation of the size.
    /// </summary>
    /// <returns>The string representation of the size.</returns>
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", _width, _height);
    }

    /// <summary>
    /// Deconstructs the size into its Width and Height values.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public void Deconstruct(out int width, out int height)
    {
        width = _width;
        height = _height;
    }
}