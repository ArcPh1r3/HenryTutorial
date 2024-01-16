using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnhandledComponentsReporter : ComponentCopier<Component> {

    public override bool hasComponent => storedComponents != null || storedComponents?.Count == 0;

    public override string copyReport { get => ""; }

    public List<Component> storedComponents;

    public override void StoreComponent(GameObject copiedObject) {
        storedComponents = copiedObject.GetComponents<Component>().ToList();
    }

    public override void PasteComponent(GameObject selected) {
        base.PasteComponent(selected);

        storedComponents = null;
    }

    protected override void PasteStoredComponent(GameObject selected) {

        List<Type> extratypes = new List<Type>();

        for (int i = 0; i < storedComponents.Count; i++) {

            Type componentType = storedComponents[i].GetType();
            if (!selected.GetComponent(componentType)) {
                extratypes.Add(componentType);
            }
        }

        string typesLog = "";
        for (int i = 0; i < extratypes.Count; i++) {
            typesLog += $"{extratypes[i]}, ";
        }

        if (!string.IsNullOrEmpty(typesLog)) {
            pasteReport += $"\nfollowing components from original object were not transferred: {typesLog}";
        }
    }
}