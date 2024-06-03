using System;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

/// <summary>
///     A <see cref="Panel" /> with uniform column and row sizes.
/// </summary>
public class UniformGrid : Panel
{
    public UniformGrid()
    {
        RowsProperty = Property(nameof(Rows), 0);
        ColumnsProperty = Property(nameof(Columns), 0);
        FirstColumnProperty = Property(nameof(FirstColumn), 0);
        AffectsMeasure<int>(RowsProperty);
        AffectsMeasure<int>(ColumnsProperty);
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

    public int ActualRows { get; private set; }

    public int ActualColumns { get; private set; }

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateRowsAndColumns();

        var maxWidth = 0;
        var maxHeight = 0;
        var rows = ActualRows == 0 ? 1 : ActualRows;
        var columns = ActualColumns == 0 ? 1 : ActualColumns;
        var childAvailableSize = new Size(availableSize.Width / columns, availableSize.Height / rows);

        foreach (var child in Children)
        {
            child.Measure(childAvailableSize);

            if (child.DesiredSize.Width > maxWidth) maxWidth = child.DesiredSize.Width;

            if (child.DesiredSize.Height > maxHeight) maxHeight = child.DesiredSize.Height;
        }

        return new Size(maxWidth * columns, maxHeight * rows);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var x = FirstColumn;
        var y = 0;

        var rows = ActualRows == 0 ? 1 : ActualRows;
        var columns = ActualColumns == 0 ? 1 : ActualColumns;
        var width = finalSize.Width / columns;
        var height = finalSize.Height / rows;

        foreach (var child in Children)
        {
            if (!child.IsVisible) continue;

            child.Arrange(new Rect(x * width, y * height, width, height));

            x++;

            if (x >= Columns)
            {
                x = 0;
                y++;
            }
        }

        return finalSize;
    }

    private void UpdateRowsAndColumns()
    {
        var _rows = Rows;
        var _columns = Columns;

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

        ActualRows = _rows;
        ActualColumns = _columns;
    }
}