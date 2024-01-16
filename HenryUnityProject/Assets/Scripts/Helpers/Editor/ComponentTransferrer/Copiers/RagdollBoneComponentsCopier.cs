using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//used within ragdollcomponentcopier, not in main editorcopyandpastecomponents
public class RagdollBoneComponentsCopier : NotRetardedUnityComponentCopier {

    protected override void PasteStoredComponent(GameObject selected) {
        base.PasteStoredComponent(selected);

        for (int i = 0; i < storedComponents.Count; i++) {

            System.Type componentType = storedComponents[i].GetType();

            if(componentType == typeof(CharacterJoint)) {
                HandlePasteJoint((CharacterJoint)storedComponents[i], selected.GetComponent<CharacterJoint>());
            }

            //todo: working out rescaling colliders for fixing the stupid 100 armatures
            //if (componentType == typeof(Collider)) {
            //    HandlePasteCollider((Collider)storedComponents[i], selected.GetComponent<Collider>());
            //}
        }
    }

    private void HandlePasteJoint(CharacterJoint storedJoint, CharacterJoint newJoint) {

        if (storedJoint.connectedBody == null) {

            pasteReport += $"\noriginal joint {storedJoint} did not have a connectedBody";
            return;
        }

        newJoint.connectedBody = null;

        List<Transform> newChildren = newJoint.transform.root.GetComponentsInChildren<Transform>(true).ToList();

        //this will break hard if there's duplicates name anywhere on the rig
        //so don't do that
        Transform newConnectedBody = newChildren.Find(tran => tran.name == storedJoint.connectedBody.transform.name);

        if (newConnectedBody != null) {
            newJoint.connectedBody = newConnectedBody.GetComponent<Rigidbody>();

            if (newJoint.connectedBody == null) {
                pasteReport += $"\nfound connectedbody for {newJoint}, {newConnectedBody}, did not have a rigidbody";
            }
        } else {
            pasteReport += $"\ncould not find connectedbody for joint {newJoint}";
        }
    }

    private void HandlePasteCollider(Collider storedCollider, Collider newCollider) {

        if (storedCollider is SphereCollider) {

            SphereCollider storedSphereCollider = (SphereCollider)storedCollider;
            SphereCollider newSphereCollider = (SphereCollider)newCollider;
            //todo
            Vector3 scale = newCollider.transform.lossyScale;

            newSphereCollider.center = storedSphereCollider.center;
            newSphereCollider.radius = storedSphereCollider.radius;

            return;
        }
        //if (storedCollider is CapsuleCollider) {
        //    CapsuleCollider col = selected.GetComponent<CapsuleCollider>();

        //    col.center = (storedCollider as CapsuleCollider).center;
        //    col.radius = (storedCollider as CapsuleCollider).radius;
        //    col.height = (storedCollider as CapsuleCollider).height;
        //    col.direction = (storedCollider as CapsuleCollider).direction;
        //    return;
        //}
        //if (storedCollider is BoxCollider) {
        //    BoxCollider col = selected.GetComponent<BoxCollider>();

        //    col.center = (storedCollider as BoxCollider).center;
        //    col.size = (storedCollider as BoxCollider).size;
        //    return;
        //}
    }
}

