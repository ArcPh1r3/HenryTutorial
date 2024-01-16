using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using System;

public class EditorAddRagdoll {

    [MenuItem("CONTEXT/RagdollController/Make bones on the bones")]
    public static void createRagdoll() {

        RagdollController ragdollController = Selection.activeGameObject.GetComponent<RagdollController>();

        for (int i = 0; i < ragdollController.bones.Length; i++) {
            SetupBone(ragdollController.bones[i]);
        }
    }

    private static void SetupBone(Transform bone) {

        Undo.RecordObject(bone, "getting boned");

        CreateCollider(bone);
        CreateRigidBody(bone);
        CreateJoint(bone);
    }

    private static void CreateCollider(Transform bone) {

        if (bone.GetComponent<Collider>()) {
            Debug.Log($"{bone.name} already has collider. aborting", bone);
            return;
        }

        CapsuleCollider collider = Undo.AddComponent<CapsuleCollider>(bone.gameObject);
        Undo.RegisterCreatedObjectUndo(collider, "getting boned");

        collider.radius = 0.1f;

        if (bone.childCount < 1) {
            collider.height = 0.2f;
            return;
        }

        Transform child = bone.GetChild(0);
        collider.height = child.localPosition.y * 0.9f;
        collider.center = new Vector3(0, child.localPosition.y * 0.5f);
    }
    
    private static void CreateRigidBody(Transform bone) {

        if (bone.GetComponent<Rigidbody>()) {
            Debug.Log($"{bone.name} already has Rigidbody. aborting", bone);
            return;
        }

        Rigidbody rig = Undo.AddComponent<Rigidbody>(bone.gameObject);
        Undo.RegisterCreatedObjectUndo(rig, "getting boned");

        rig.isKinematic = true;
    }

    private static void CreateJoint(Transform bone) {

        if (bone.GetComponent<CharacterJoint>()) {
            Debug.Log($"{bone.name} already has CharacterJoint. aborting", bone);
            return;
        }
        CharacterJoint joint = Undo.AddComponent<CharacterJoint>(bone.gameObject);
        Undo.RegisterCreatedObjectUndo(joint, "getting boned");

        joint.connectedBody = joint.transform.parent.GetComponentInParent<Rigidbody>();

        if(joint.connectedBody == null) {
            Debug.Log($"Didn't find connectedbody for {joint}.\nAdd your connectedbody, or remove the CharacterJoint component if it is a root bone.", joint);
        }
    }
}
