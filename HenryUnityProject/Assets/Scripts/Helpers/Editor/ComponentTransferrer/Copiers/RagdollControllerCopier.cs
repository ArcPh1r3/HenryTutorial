using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoR2;
using UnityEditor;

public class RagdollControllerCopier : ComponentCopier<RagdollController> {

    protected override void PasteStoredComponent(GameObject selected) {
        
        RagdollController newController = GetOrAddPastedComponent<RagdollController>(selected);

        List<Transform> selectedChildren = selected.GetComponentsInChildren<Transform>(true).ToList();

        newController.bones = new Transform[storedComponent.bones.Length];

        for (int i = 0; i < newController.bones.Length; i++) {

            Transform newBone = newController.bones[i];
            Transform storedBone = storedComponent.bones[i];

            //if (storedBone == null) 
            //    continue;

            newBone = selectedChildren.Find(tran => tran.name == storedBone.name);

            newController.bones[i] = newBone;

            if (newBone == null) {
                pasteReport += $"\ncould not get bone for {storedBone.name}";
            }

            if (newBone != null && !newBone.GetComponent<Rigidbody>() && storedBone.GetComponent<Rigidbody>()) {

                RagdollBoneComponentsCopier componentCopier = new RagdollBoneComponentsCopier();
                componentCopier.TransferComponents(storedBone.gameObject, newBone.gameObject);

                pasteReport += $"\n found collider for {newBone.name}. Trasnferring ragdoll bone: {componentCopier.pasteReport}";
            }
        }
        
        //todod: componentstodisableonragdoll
    }
}

