namespace Chibi.Ui.Tests;

public class UniformGridFacts
{
    public TestView TestView { get; } = new()
    {
        AllocatedSize = new Size(240, 320)
    };

    [Fact]
    public void Invalidate_when_Child_added()
    {
        /* Given */
        var sut = new UniformGrid();
        TestView.Content = sut;
        TestView.Load();

        Assert.True(sut.IsMeasureValid);
        Assert.True(sut.IsArrangeValid);

        /* When */
        sut.Children.Add(new TextBlock());

        /* Then */
        Assert.False(sut.IsMeasureValid);
        Assert.False(sut.IsArrangeValid);
    }

    [Fact]
    public void Invalidate_when_Children_replaced()
    {
        /* Given */
        var sut = new UniformGrid()
        {
            Children =
            [
                new TextBlock()
                {
                    Text = "initial"
                }
            ]
        };
        TestView.Content = sut;
        TestView.Load();
        TestView.LayoutManager.ExecuteInitialLayoutPass();

        Assert.True(sut.IsMeasureValid);
        Assert.True(sut.IsArrangeValid);

        /* When */
        sut.Children =
        [
            new TextBlock()
            {
                Text = "actual"
            }
        ];

        /* Then */
        Assert.False(sut.IsMeasureValid);
        Assert.False(sut.IsArrangeValid);
    }

    [Fact]
    public void Valid_after_LayoutPass_when_Children_replaced()
    {
        /* Given */
        var sut = new UniformGrid()
        {
            Children =
            [
                new TextBlock()
                {
                    Text = "initial"
                }
            ]
        };
        TestView.Content = sut;
        TestView.Load();
        TestView.LayoutManager.ExecuteInitialLayoutPass();

        Assert.True(sut.IsMeasureValid);
        Assert.True(sut.IsArrangeValid);

        /* When */
        sut.Children =
        [
            new TextBlock()
            {
                Text = "actual"
            }
        ];
        Assert.False(sut.IsMeasureValid);
        Assert.False(sut.IsArrangeValid);
        TestView.LayoutManager.ExecuteLayoutPass();

        /* Then */
        Assert.True(sut.IsMeasureValid);
        Assert.True(sut.IsArrangeValid);
    }

    [Fact]
    public async Task Invalidate_when_Children_added_with_task()
    {
        /* Given */
        var sut = new UniformGrid()
        {
            Children =
            [
                new TextBlock()
                {
                    Text = "initial"
                }
            ]
        };
        TestView.Content = sut;
        TestView.Load();
        TestView.LayoutManager.ExecuteInitialLayoutPass();

        Assert.True(sut.IsMeasureValid);
        Assert.True(sut.IsArrangeValid);

        /* When */
        await Task.Run(() =>
        {
            sut.Children =
            [
                new TextBlock()
                {
                    Text = "actual"
                }
            ];

            /* Then */
            Assert.False(sut.IsMeasureValid);
            Assert.False(sut.IsArrangeValid);
        });

    }

    [Fact]
    public void InvalidateRowsAndColumns_when_Children_replaced()
    {
        /* Given */
        var sut = new UniformGrid()
        {
            Children =
            [
                new TextBlock()
                {
                    Text = "initial"
                }
            ]
        };
        TestView.Content = sut;
        TestView.Load();
        TestView.LayoutManager.ExecuteInitialLayoutPass();


        /* When */
        sut.Children =
        [
            new TextBlock()
            {
                Text = "1"
            },
            new TextBlock()
            {
                Text = "2"
            }
        ];

        TestView.LayoutManager.ExecuteLayoutPass();

        /* Then */
        Assert.Equal(2, sut.ActualRows);
        Assert.Equal(2, sut.ActualColumns);
    }

    [Fact]
    public void InvalidateRows_When_Columns_set_when_Children_replaced()
    {
        /* Given */
        var sut = new UniformGrid()
        {
            Columns = 1,
            Children =
            [
                new TextBlock()
                {
                    Text = "initial"
                }
            ]
        };
        TestView.Content = sut;
        TestView.Load();
        TestView.LayoutManager.ExecuteInitialLayoutPass();


        /* When */
        sut.Children =
        [
            new TextBlock()
            {
                Text = "1"
            },
            new TextBlock()
            {
                Text = "2"
            }
        ];

        TestView.LayoutManager.ExecuteLayoutPass();

        /* Then */
        Assert.Equal(2, sut.ActualRows);
        Assert.Equal(1, sut.ActualColumns);
    }
}