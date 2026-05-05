using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSocket : MonoBehaviour
{
    public MeshSockets.SocketId socketId;
    public HumanBodyBones bone;

    public Vector3 offset;
    public Vector3 rotation;

    public Transform attachPoint;

    void Start()
    {
        Animator animator = GetComponentInParent<Animator>();
        if (attachPoint == null)
        {
            attachPoint = new GameObject("socket" + socketId).transform;
            attachPoint.SetParent(animator.GetBoneTransform(bone));
            attachPoint.localPosition = offset;
            attachPoint.localRotation = Quaternion.Euler(rotation);
        }
    }

    public void Attach(Transform objectTransform) {
        objectTransform.SetParent(attachPoint, false);
    }
}
