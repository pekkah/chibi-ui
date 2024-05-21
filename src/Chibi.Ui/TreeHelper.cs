using System.Collections.Generic;

namespace Chibi.Ui;

public class TreeHelper
{
    public static void Visit(UiElement root, System.Action<UiElement?, UiElement> visitor)
    {
        VisitCore(null, root, visitor);

        static void VisitCore(UiElement? parent, UiElement child, System.Action<UiElement?, UiElement> visitor)
        {
            visitor(parent, child);
            for (var i = 0; i < child.GetChildCount(); i++)
            {
                VisitCore(child, child.GetChild(i), visitor);
            }
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
