using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChildLocatorCopier : ComponentCopier<ChildLocator> {

    protected override void PasteStoredComponent(GameObject selected) {
        
        #region comment out this region if you're in thunderkit
        //in order to use this power, you must
        //  find your ror2 Assembly-Csharp.dll you're referencing in editor
        //  open it up in dnspy,
        //      expose the private variable ChildLocator.transformPairs to be public,
        //             the private struct ChildLocator.NameTransformPair,
        //             the private variables in the struct,
        //             
        //if that sounds retarded, that's because it is. would love help devising a better way
        //if you have no idea what the fuck I'm talking about ping Thetimesweeper#5727

        ChildLocator newLocator = GetOrAddPastedComponent<ChildLocator>(selected);

        List<Transform> newChildren = selected.GetComponentsInChildren<Transform>(true).ToList();

        newLocator.transformPairs = new ChildLocator.NameTransformPair[storedComponent.transformPairs.Length];

        for (int i = 0; i < newLocator.transformPairs.Length; i++) {

            ChildLocator.NameTransformPair newPair = newLocator.transformPairs[i];
            ChildLocator.NameTransformPair storedPair = storedComponent.transformPairs[i];

            newPair.name = storedPair.name;

            if (storedPair.transform != null) {
                //check all children for name that matches old transform
                newPair.transform = newChildren.Find(tran => tran.name == storedPair.transform.name);
            }

            if (newPair.transform == null) {
                pasteReport += $"\ncould not find child transform for {newPair.name}";
            }

            newLocator.transformPairs[i] = newPair;
        }
        #endregion
    
    }
}

