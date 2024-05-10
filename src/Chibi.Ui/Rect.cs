using System;
using System.Globalization;
using System.Numerics;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui;

/// <summary>
/// Defines a rectangle.
/// </summary>
public readonly struct Rect : IEquatable<Rect>
{
    /// <summary>
    /// The X position.
    /// </summary>
    private readonly int _x;

    /// <summary>
    /// The Y position.
    /// </summary>
    private readonly int _y;

    /// <summary>
    /// The width.
    /// </summary>
    private readonly int _width;

    /// <summary>
    /// The height.
    /// </summary>
    private readonly int _height;

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect"/> structure.
    /// </summary>
    /// <param name="x">The X position.</param>
    /// <param name="y">The Y position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public Rect(int x, int y, int width, int height)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect"/> structure.
    /// </summary>
    /// <param name="size">The size of the rectangle.</param>
    public Rect(Size size)
    {
        _x = 0;
        _y = 0;
        _width = size.Width;
        _height = size.Height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect"/> structure.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    public Rect(Point position, Size size)
    {
        _x = position.X;
        _y = position.Y;
        _width = size.Width;
        _height = size.Height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect"/> structure.
    /// </summary>
    /// <param name="topLeft">The top left position of the rectangle.</param>
    /// <param name="bottomRight">The bottom right position of the rectangle.</param>
    public Rect(Point topLeft, Point bottomRight)
    {
        _x = topLeft.X;
        _y = topLeft.Y;
        _width = bottomRight.X - topLeft.X;
        _height = bottomRight.Y - topLeft.Y;
    }

    /// <summary>
    /// Gets the X position.
    /// </summary>
    public int X => _x;

    /// <summary>
    /// Gets the Y position.
    /// </summary>
    public int Y => _y;

    /// <summary>
    /// Gets the width.
    /// </summary>
    public int Width => _width;

    /// <summary>
    /// Gets the height.
    /// </summary>
    public int Height => _height;

    /// <summary>
    /// Gets the position of the rectangle.
    /// </summary>
    public Point Position => new Point(_x, _y);

    /// <summary>
    /// Gets the size of the rectangle.
    /// </summary>
    public Size Size => new Size(_width, _height);

    /// <summary>
    /// Gets the right position of the rectangle.
    /// </summary>
    public int Right => _x + _width;

    /// <summary>
    /// Gets the bottom position of the rectangle.
    /// </summary>
    public int Bottom => _y + _height;

    /// <summary>
    /// Gets the left position.
    /// </summary>
    public int Left => _x;

    /// <summary>
    /// Gets the top position.
    /// </summary>
    public int Top => _y;

    /// <summary>
    /// Gets the top left point of the rectangle.
    /// </summary>
    public Point TopLeft => new Point(_x, _y);

    /// <summary>
    /// Gets the top right point of the rectangle.
    /// </summary>
    public Point TopRight => new Point(Right, _y);

    /// <summary>
    /// Gets the bottom left point of the rectangle.
    /// </summary>
    public Point BottomLeft => new Point(_x, Bottom);

    /// <summary>
    /// Gets the bottom right point of the rectangle.
    /// </summary>
    public Point BottomRight => new Point(Right, Bottom);

    /// <summary>
    /// Gets the center point of the rectangle.
    /// </summary>
    public Point Center => new Point(_x + _width / 2, _y + _height / 2);

    /// <summary>
    /// Checks for equality between two <see cref="Rect"/>s.
    /// </summary>
    /// <param name="left">The first rect.</param>
    /// <param name="right">The second rect.</param>
    /// <returns>True if the rects are equal; otherwise false.</returns>
    public static bool operator ==(Rect left, Rect right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks for inequality between two <see cref="Rect"/>s.
    /// </summary>
    /// <param name="left">The first rect.</param>
    /// <param name="right">The second rect.</param>
    /// <returns>True if the rects are unequal; otherwise false.</returns>
    public static bool operator !=(Rect left, Rect right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Multiplies a rectangle by a scaling Vector2.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="scale">The Vector2 scale.</param>
    /// <returns>The scaled rectangle.</returns>
    public static Rect operator *(Rect rect, Vector2 scale)
    {
        return new Rect(
            (int)(rect.X * scale.X),
            (int)(rect.Y * scale.Y),
            (int)(rect.Width * scale.X),
            (int)(rect.Height * scale.Y));
    }

    /// <summary>
    /// Multiplies a rectangle by a scale.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>The scaled rectangle.</returns>
    public static Rect operator *(Rect rect, double scale)
    {
        return new Rect(
            (int)(rect.X * scale),
            (int)(rect.Y * scale),
            (int)(rect.Width * scale),
            (int)(rect.Height * scale));
    }

    /// <summary>
    /// Divides a rectangle by a Vector2.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="scale">The Vector2 scale.</param>
    /// <returns>The scaled rectangle.</returns>
    public static Rect operator /(Rect rect, Vector2 scale)
    {
        return new Rect(
            (int)(rect.X / scale.X),
            (int)(rect.Y / scale.Y),
            (int)(rect.Width / scale.X),
            (int)(rect.Height / scale.Y));
    }

    /// <summary>
    /// Determines whether a point is in the bounds of the rectangle.
    /// </summary>
    /// <param name="p">The point.</param>
    /// <returns>true if the point is in the bounds of the rectangle; otherwise false.</returns>
    public bool Contains(Point p)
    {
        return p.X >= _x && p.X <= _x + _width &&
               p.Y >= _y && p.Y <= _y + _height;
    }

    /// <summary>
    /// Determines whether a point is in the bounds of the rectangle, exclusive of the
    /// rectangle's bottom/right edge.
    /// </summary>
    /// <param name="p">The point.</param>
    /// <returns>true if the point is in the bounds of the rectangle; otherwise false.</returns>    
    public bool ContainsExclusive(Point p)
    {
        return p.X >= _x && p.X < _x + _width &&
               p.Y >= _y && p.Y < _y + _height;
    }

    /// <summary>
    /// Determines whether the rectangle fully contains another rectangle.
    /// </summary>
    /// <param name="r">The rectangle.</param>
    /// <returns>true if the rectangle is fully contained; otherwise false.</returns>
    public bool Contains(Rect r)
    {
        return Contains(r.TopLeft) && Contains(r.BottomRight);
    }

    /// <summary>
    /// Centers another rectangle in this rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to center.</param>
    /// <returns>The centered rectangle.</returns>
    public Rect CenterRect(Rect rect)
    {
        return new Rect(
            _x + (_width - rect._width) / 2,
            _y + (_height - rect._height) / 2,
            rect._width,
            rect._height);
    }

    /// <summary>
    /// Inflates the rectangle.
    /// </summary>
    /// <param name="thickness">The thickness to be subtracted for each side of the rectangle.</param>
    /// <returns>The inflated rectangle.</returns>
    public Rect Inflate(int thickness)
    {
        return Inflate(new Thickness(thickness));
    }

    /// <summary>
    /// Inflates the rectangle.
    /// </summary>
    /// <param name="thickness">The thickness to be subtracted for each side of the rectangle.</param>
    /// <returns>The inflated rectangle.</returns>
    public Rect Inflate(Thickness thickness)
    {
        return new Rect(
            new Point(_x - thickness.Left, _y - thickness.Top),
            Size.Inflate(thickness));
    }

    /// <summary>
    /// Deflates the rectangle.
    /// </summary>
    /// <param name="thickness">The thickness to be subtracted for each side of the rectangle.</param>
    /// <returns>The deflated rectangle.</returns>
    public Rect Deflate(int thickness)
    {
        return Deflate(new Thickness(thickness));
    }

    /// <summary>
    /// Deflates the rectangle by a <see cref="Thickness"/>.
    /// </summary>
    /// <param name="thickness">The thickness to be subtracted for each side of the rectangle.</param>
    /// <returns>The deflated rectangle.</returns>
    public Rect Deflate(Thickness thickness)
    {
        return new Rect(
            new Point(_x + thickness.Left, _y + thickness.Top),
            Size.Deflate(thickness));
    }

    /// <summary>
    /// Returns a boolean indicating whether the rect is equal to the other given rect.
    /// </summary>
    /// <param name="other">The other rect to test equality against.</param>
    /// <returns>True if this rect is equal to other; False otherwise.</returns>
    public bool Equals(Rect other)
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return _x == other._x &&
               _y == other._y &&
               _width == other._width &&
               _height == other._height;
        // ReSharper enable CompareOfFloatsByEqualityOperator
    }

    /// <summary>
    /// Returns a boolean indicating whether the given object is equal to this rectangle.
    /// </summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if the object is equal to this rectangle; false otherwise.</returns>
    public override bool Equals(object? obj) => obj is Rect other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Width.GetHashCode();
            hash = hash * 23 + Height.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// Gets the intersection of two rectangles.
    /// </summary>
    /// <param name="rect">The other rectangle.</param>
    /// <returns>The intersection.</returns>
    public Rect Intersect(Rect rect)
    {
        var newLeft = rect.X > X ? rect.X : X;
        var newTop = rect.Y > Y ? rect.Y : Y;
        var newRight = rect.Right < Right ? rect.Right : Right;
        var newBottom = rect.Bottom < Bottom ? rect.Bottom : Bottom;

        if (newRight > newLeft && newBottom > newTop)
        {
            return new Rect(newLeft, newTop, newRight - newLeft, newBottom - newTop);
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Determines whether a rectangle intersects with this rectangle.
    /// </summary>
    /// <param name="rect">The other rectangle.</param>
    /// <returns>
    /// True if the specified rectangle intersects with this one; otherwise false.
    /// </returns>
    public bool Intersects(Rect rect)
    {
        return rect.X < Right && X < rect.Right && rect.Y < Bottom && Y < rect.Bottom;
    }

    /// <summary>
    /// Translates the rectangle by an offset.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>The translated rectangle.</returns>
    public Rect Translate(Vector2 offset)
    {
        return new Rect((int)(Position.X + offset.X), (int)(Position.Y + offset.Y), Size.Width, Size.Height);
    }

    /// <summary>
    /// Translates the rectangle by an offset.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>The translated rectangle.</returns>
    public Rect Translate(Point offset)
    {
        return new Rect(Position.X + offset.X, Position.Y + offset.Y, Size.Width, Size.Height);
    }

    /// <summary>
    /// Normalizes the rectangle so both the <see cref="Width"/> and <see 
    /// cref="Height"/> are positive, without changing the location of the rectangle
    /// </summary>
    /// <returns>Normalized Rect</returns>
    /// <remarks>
    /// Empty rect will be return when Rect contains invalid values. Like NaN.
    /// </remarks>
    public Rect Normalize()
    {
        Rect rect = this;

        if (double.IsNaN(rect.Right) || double.IsNaN(rect.Bottom) ||
           double.IsNaN(rect.X) || double.IsNaN(rect.Y) ||
           double.IsNaN(Height) || double.IsNaN(Width))
        {
            return default;
        }

        if (rect.Width < 0)
        {
            var x = X + Width;
            var width = X - x;

            rect = rect.WithX(x).WithWidth(width);
        }

        if (rect.Height < 0)
        {
            var y = Y + Height;
            var height = Y - y;

            rect = rect.WithY(y).WithHeight(height);
        }

        return rect;
    }

    /// <summary>
    /// Gets the union of two rectangles.
    /// </summary>
    /// <param name="rect">The other rectangle.</param>
    /// <returns>The union.</returns>
    public Rect Union(Rect rect)
    {
        if (Width == 0 && Height == 0)
        {
            return rect;
        }
        else if (rect.Width == 0 && rect.Height == 0)
        {
            return this;
        }
        else
        {
            var x1 = Math.Min(X, rect.X);
            var x2 = Math.Max(Right, rect.Right);
            var y1 = Math.Min(Y, rect.Y);
            var y2 = Math.Max(Bottom, rect.Bottom);

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }
    }

    internal static Rect? Union(Rect? left, Rect? right)
    {
        if (left == null)
            return right;
        if (right == null)
            return left;
        return left.Value.Union(right.Value);
    }

    /// <summary>
    /// Returns a new <see cref="Rect"/> with the specified X position.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <returns>The new <see cref="Rect"/>.</returns>
    public Rect WithX(int x)
    {
        return new Rect(x, _y, _width, _height);
    }

    /// <summary>
    /// Returns a new <see cref="Rect"/> with the specified Y position.
    /// </summary>
    /// <param name="y">The y position.</param>
    /// <returns>The new <see cref="Rect"/>.</returns>
    public Rect WithY(int y)
    {
        return new Rect(_x, y, _width, _height);
    }

    /// <summary>
    /// Returns a new <see cref="Rect"/> with the specified width.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <returns>The new <see cref="Rect"/>.</returns>
    public Rect WithWidth(int width)
    {
        return new Rect(_x, _y, width, _height);
    }

    /// <summary>
    /// Returns a new <see cref="Rect"/> with the specified height.
    /// </summary>
    /// <param name="height">The height.</param>
    /// <returns>The new <see cref="Rect"/>.</returns>
    public Rect WithHeight(int height)
    {
        return new Rect(_x, _y, _width, height);
    }

    /// <summary>
    /// Returns the string representation of the rectangle.
    /// </summary>
    /// <returns>The string representation of the rectangle.</returns>
    public override string ToString()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0}, {1}, {2}, {3}",
            _x,
            _y,
            _width,
            _height);
    }

    /// <summary>
    /// This method should be used internally to check for the rect emptiness
    /// Once we add support for WPF-like empty rects, there will be an actual implementation
    /// For now it's internal to keep some loud community members happy about the API being pretty 
    /// </summary>
    internal bool IsEmpty() => this == default;
}