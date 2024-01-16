using System;
using UnityEditor;
using UnityEngine;

public abstract class ComponentCopierBase { /*uh*/

    public abstract bool hasComponent { get; }

    public virtual string copyReport { get; set; }
    public virtual string pasteReport { get; set; }

    public abstract void StoreComponent(GameObject copiedObject);

    public abstract void PasteComponent(GameObject selected);

    public void TransferComponents(GameObject from, GameObject to) {
        StoreComponent(from);
        PasteComponent(to);
    }
}

public abstract class ComponentCopier<T> : ComponentCopierBase where T : Component{

    public override bool hasComponent => storedComponent != null;

    public T storedComponent;

    public override void StoreComponent(GameObject copiedObject) {

        copyReport = "";
        storedComponent = copiedObject.GetComponent<T>();
        copyReport = $"{storedComponent?.GetType().ToString()}, ";
    }
    public override void PasteComponent(GameObject selected) {

        if (!hasComponent)
            return;

        pasteReport = "";
        Undo.RegisterCompleteObjectUndo(selected, "paste copied components");

        PasteStoredComponent(selected);

        storedComponent = null;
    }

    protected abstract void PasteStoredComponent(GameObject selected);

    protected T GetOrAddPastedComponent<C>(GameObject selected) where C : Component {

        T newComponent = selected.GetComponent<T>();
        if (newComponent == null) {
            newComponent = selected.AddComponent<T>();
        } else {
            pasteReport += $"\nalready had {typeof(T)}";
        }
        Undo.RegisterCreatedObjectUndo(newComponent, "paste copied components");

        return newComponent;
    }
}
