using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//handles unity components such as colliders rigidbodies and joints
//you know just 3 random examples
public class NotRetardedUnityComponentCopier : ComponentCopier<Component> {

    public override bool hasComponent => storedComponents != null || storedComponents?.Count == 0;

    public List<Component> storedComponents;

    public override void StoreComponent(GameObject copiedObject) {
        copyReport = "";
        storedComponents = copiedObject.GetComponents<Component>().ToList();

        storedComponents.RemoveAll(comp => comp is MonoBehaviour || comp is Transform);

        for (int i = 0; i < storedComponents.Count; i++) {
            copyReport += $"{storedComponents[i].GetType().ToString()}, ";
        }
    }

    public override void PasteComponent(GameObject selected) {
        base.PasteComponent(selected);

        storedComponents = null;
    }

    protected override void PasteStoredComponent(GameObject selected) {

        for (int i = 0; i < storedComponents.Count; i++) {
            UnityEditorInternal.ComponentUtility.CopyComponent(storedComponents[i]);

            System.Type componentType = storedComponents[i].GetType();

            Component newComponent = selected.GetComponent(componentType);
            if (newComponent == null) {

                newComponent = selected.AddComponent(componentType);
                Undo.RegisterCreatedObjectUndo(newComponent, "paste copied components");

                pasteReport += $"\nadded {componentType}";
            } else {

            //update values of existing component. doesn't support multiple of same component.
                pasteReport += $"\nalready had {componentType}";
            }
            UnityEditorInternal.ComponentUtility.PasteComponentValues(newComponent);
        }
    }
}

