using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

/// <summary>
///     A <see cref="Panel" /> with uniform column and row sizes.
/// </summary>
public class UniformGrid : Panel
{
    private int _columns;

    private int _rows;

    public UniformGrid()
    {
        RowsProperty = Property(nameof(Rows), 0);
        ColumnsProperty = Property(nameof(Columns), 0);
        FirstColumnProperty = Property(nameof(FirstColumn), 0);
    }

    /// <summary>
    ///     Defines the <see cref="Rows" /> property.
    /// </summary>
    public ReactiveProperty<int> RowsProperty { get; }

    /// <summary>
    ///     Defines the <see cref="Columns" /> property.
    /// </summary>
    public ReactiveProperty<int> ColumnsProperty { get; }

    /// <summary>
    ///     Defines the <see cref="FirstColumn" /> property.
    /// </summary>
    public ReactiveProperty<int> FirstColumnProperty { get; }

    /// <summary>
    ///     Specifies the row count. If set to 0, row count will be calculated automatically.
    /// </summary>
    public int Rows
    {
        get => RowsProperty.Value;
        set => RowsProperty.Value = value;
    }

    /// <summary>
    ///     Specifies the column count. If set to 0, column count will be calculated automatically.
    /// </summary>
    public int Columns
    {
        get => ColumnsProperty.Value;
        set => ColumnsProperty.Value = value;
    }

    /// <summary>
    ///     Specifies, for the first row, the column where the items should start.
    /// </summary>
    public int FirstColumn
    {
        get => FirstColumnProperty.Value;
        set => FirstColumnProperty.Value = value;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateRowsAndColumns();

        var maxWidth = 0;
        var maxHeight = 0;

        var childAvailableSize = new Size(availableSize.Width / _columns, availableSize.Height / _rows);

        foreach (var child in Children)
        {
            child.Measure(childAvailableSize);

            if (child.DesiredSize.Width > maxWidth) maxWidth = child.DesiredSize.Width;

            if (child.DesiredSize.Height > maxHeight) maxHeight = child.DesiredSize.Height;
        }

        return new Size(maxWidth * _columns, maxHeight * _rows);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var x = FirstColumn;
        var y = 0;

        var width = finalSize.Width / _columns;
        var height = finalSize.Height / _rows;

        foreach (var child in Children)
        {
            if (!child.IsVisible) continue;

            child.Arrange(new Rect(x * width, y * height, width, height));

            x++;

            if (x >= _columns)
            {
                x = 0;
                y++;
            }
        }

        return finalSize;
    }

    private void UpdateRowsAndColumns()
    {
        _rows = Rows;
        _columns = Columns;

        if (FirstColumn >= Columns) FirstColumn = 0;

        var itemCount = FirstColumn;

        foreach (var child in Children)
            if (child.IsVisible)
                itemCount++;

        if (_rows == 0)
        {
            if (_columns == 0)
            {
                _rows = _columns = (int)Math.Ceiling(Math.Sqrt(itemCount));
            }
            else
            {
                _rows = Math.DivRem(itemCount, _columns, out var rem);

                if (rem != 0) _rows++;
            }
        }
        else if (_columns == 0)
        {
            _columns = Math.DivRem(itemCount, _rows, out var rem);

            if (rem != 0) _columns++;
        }
    }
}