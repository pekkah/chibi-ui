using System;
using System.Collections.Generic;
using Chibi.Ui.Views;

namespace Chibi.Ui;

public class LayoutManager(ViewBase owner)
{
    private const int MaxPasses = 10;
    private readonly UiElement _owner = owner as UiElement ?? throw new ArgumentNullException(nameof(owner));
    private readonly Queue<UiElement> _toArrange = new();
    private readonly List<UiElement> _toArrangeAfterMeasure = new();
    private readonly Queue<UiElement> _toMeasure = new();
    private bool _queued;
    private bool _running;
    private int _totalPassCount;

    public virtual event EventHandler? LayoutUpdated;

    public virtual void InvalidateMeasure(UiElement control)
    {
        if (control is null)
            throw new ArgumentNullException(nameof(control));

        if (control is not ILayoutRoot && control.RootElement is null)
        {
#if DEBUG
            throw new InvalidOperationException(
                "LayoutManager.InvalidateMeasure called on a control that is detached from the visual tree.");
#else
                return;
#endif
        }

        _toMeasure.Enqueue(control);
        QueueLayoutPass();
    }

    /// <inheritdoc />
    public virtual void InvalidateArrange(UiElement control)
    {
        if (control is null)
            throw new ArgumentNullException(nameof(control));

        if (control.RootElement is null && control is not ILayoutRoot)
        {
#if DEBUG
            throw new InvalidOperationException(
                "LayoutManager.InvalidateArrange called on a control that is detached from the visual tree.");
#else
                return;
#endif
        }

        _toArrange.Enqueue(control);
        QueueLayoutPass();
    }

    public void ExecuteQueuedLayoutPass()
    {
        if (!_queued) return;

        ExecuteLayoutPass();
    }

    /// <inheritdoc />
    public virtual void ExecuteLayoutPass()
    {
    
        if (!_running)
            try
            {
                _running = true;
                ++_totalPassCount;

                for (var pass = 0; pass < MaxPasses; ++pass)
                {
                    InnerLayoutPass();
                }
            }
            finally
            {
                _running = false;
            }

        _queued = false;
        LayoutUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public virtual void ExecuteInitialLayoutPass()
    {
        try
        {
            _running = true;
            Measure(_owner);
            Arrange(_owner);
        }
        finally
        {
            _running = false;
        }

        // Running the initial layout pass may have caused some control to be invalidated
        // so run a full layout pass now (this usually due to scrollbars; its not known
        // whether they will need to be shown until the layout pass has run and if the
        // first guess was incorrect the layout will need to be updated).
        ExecuteLayoutPass();
    }


    private void InnerLayoutPass()
    {
        for (var pass = 0; pass < MaxPasses; ++pass)
        {
            ExecuteMeasurePass();
            ExecuteArrangePass();

            if (_toMeasure.Count == 0) break;
        }
    }

    private void ExecuteMeasurePass()
    {
        while (_toMeasure.Count > 0)
        {
            var control = _toMeasure.Dequeue();

            if (!control.IsMeasureValid) Measure(control);

            _toArrange.Enqueue(control);
        }
    }

    private void ExecuteArrangePass()
    {
        while (_toArrange.Count > 0)
        {
            var control = _toArrange.Dequeue();

            if (!control.IsArrangeValid)
                if (Arrange(control) == ArrangeResult.AncestorMeasureInvalid)
                    _toArrangeAfterMeasure.Add(control);
        }

        foreach (var i in _toArrangeAfterMeasure)
            InvalidateArrange(i);
        _toArrangeAfterMeasure.Clear();
    }

    private bool Measure(UiElement control)
    {
        if (!control.IsVisible || control.RootElement is null)
            return false;

        // Controls closest to the visual root need to be arranged first. We don't try to store
        // ordered invalidation lists, instead we traverse the tree upwards, measuring the
        // controls closest to the root first. This has been shown by benchmarks to be the
        // fastest and most memory-efficient algorithm.
        if (control.ParentElement is { } parent)
            if (!Measure(parent))
                return false;

        // If the control being measured has IsMeasureValid == true here then its measure was
        // handed by an ancestor and can be ignored. The measure may have also caused the
        // control to be removed.
        if (!control.IsMeasureValid)
        {
            if (control is IEmbeddedLayoutRoot embeddedRoot)
                control.Measure(embeddedRoot.AllocatedSize);
            else if (control is ILayoutRoot root)
                control.Measure(Size.Infinity);
            else if (control.PreviousMeasure.HasValue) control.Measure(control.PreviousMeasure.Value);
        }

        return true;
    }

    private ArrangeResult Arrange(UiElement control)
    {
        if (!control.IsVisible || control.RootElement is null)
            return ArrangeResult.NotVisible;

        if (control.ParentElement is { } parent)
            if (Arrange(parent) is var parentResult && parentResult != ArrangeResult.Arranged)
                return parentResult;

        if (!control.IsMeasureValid)
            return ArrangeResult.AncestorMeasureInvalid;

        if (!control.IsArrangeValid)
        {
            if (control is IEmbeddedLayoutRoot embeddedRoot)
                control.Arrange(new Rect(embeddedRoot.AllocatedSize));
            else if (control is ILayoutRoot root)
                control.Arrange(new Rect(control.DesiredSize));
            else if (control.PreviousArrange != null)
                // Has been observed that PreviousArrange sometimes is null, probably a bug somewhere else.
                // Condition observed: control.VisualParent is Scrollbar, control is Border.
                control.Arrange(control.PreviousArrange.Value);
        }

        return ArrangeResult.Arranged;
    }

    private void QueueLayoutPass()
    {
        if (!_queued && !_running) _queued = true;
    }

    private enum ArrangeResult
    {
        Arranged,
        NotVisible,
        AncestorMeasureInvalid
    }
}