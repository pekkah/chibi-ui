namespace Chibi.Ui.Tests;

public class ViewBaseFacts
{
    public TestView Sut { get; } = new()
    {
        AllocatedSize = new Size(240, 320)
    };

    public ViewBaseFacts()
    {
        Sut.Load();
    }

    [Fact]
    public void Invalidate_when_Child_added()
    {
        /* Given */
        Assert.True(Sut.IsMeasureValid);
        Assert.True(Sut.IsArrangeValid);

        /* When */
        Sut.Content = new TextBlock();

        /* Then */
        Assert.False(Sut.IsMeasureValid);
        Assert.False(Sut.IsArrangeValid);
    }

    [Fact]
    public void Invalidate_when_Children_replaced()
    {
        /* Given */
        Sut.Content = new TextBlock();
        Sut.LayoutManager.ExecuteInitialLayoutPass();

        Assert.True(Sut.IsMeasureValid);
        Assert.True(Sut.IsArrangeValid);

        /* When */
        Sut.Content = new TextBlock()
        {
            Text = "actual"
        };

        /* Then */
        Assert.False(Sut.IsMeasureValid);
        Assert.False(Sut.IsArrangeValid);
    }

    [Fact]
    public void Valid_after_LayoutPass_when_Children_replaced()
    {
        /* Given */
        Sut.LayoutManager.ExecuteInitialLayoutPass();

        Assert.True(Sut.IsMeasureValid);
        Assert.True(Sut.IsArrangeValid);

        /* When */
        Sut.Content =
            new TextBlock()
            {
                Text = "actual"
            };

        Assert.False(Sut.IsMeasureValid);
        Assert.False(Sut.IsArrangeValid);
        Sut.LayoutManager.ExecuteLayoutPass();

        /* Then */
        Assert.True(Sut.IsMeasureValid);
        Assert.True(Sut.IsArrangeValid);
    }
}