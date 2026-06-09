using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class BabyItemResolver
{
    public static bool TryGetKind(IXRSelectInteractable interactable, out BabyItemKind kind)
    {
        kind = default;
        if (interactable == null)
            return false;

        var component = interactable.transform.GetComponentInParent<BabyItemTypeComponent>();
        if (component != null)
        {
            kind = component.Kind;
            return true;
        }

        return TryGetKindFromName(interactable.transform, out kind);
    }

    static bool TryGetKindFromName(Transform transform, out BabyItemKind kind)
    {
        kind = default;
        var current = transform;
        while (current != null)
        {
            switch (current.name)
            {
                case "Baby Bottle":
                    kind = BabyItemKind.Bottle;
                    return true;
                case "Pacifiler":
                case "pacifierR18.c4d":
                    kind = BabyItemKind.Pacifier;
                    return true;
                case "BabyLunaClothDiaper":
                    kind = BabyItemKind.Diaper;
                    return true;
            }

            current = current.parent;
        }

        return false;
    }

    public static bool IsCleanDiaper(IXRSelectInteractable interactable)
    {
        var diaper = interactable.transform.GetComponentInParent<DiaperItem>();
        return diaper == null || diaper.IsClean;
    }
}
