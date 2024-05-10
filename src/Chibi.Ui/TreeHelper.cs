using System.Collections.Generic;

namespace Chibi.Ui;

public class TreeHelper
{
    public static void Visit(UiElement root, System.Action<UiElement> visitor)
    {
        visitor(root);
        var count = root.GetChildCount();
        for (var i = 0; i < count; i++)
        {
            var child = root.GetChild(i);
            Visit(child, visitor);
        }
    }

    public static UiElement? FindReverse(UiElement element, System.Func<UiElement, bool> predicate)
    {
        var parent = element;
        while (parent != null)
        {
            if (predicate(parent))
                return parent;

            parent = parent.ParentElement;
        }

        return null;
    }
}

public record class TreeRoot(UiElement Root, IReadOnlyDictionary<string, UiElement> NamedElements);
