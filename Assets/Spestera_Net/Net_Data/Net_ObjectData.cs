using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_ObjectData : MonoBehaviour
{
    [SerializeField] private int _objectId;
    [SerializeField] private float _colliderSize;
    public int GetObjectId()
    {
        return _objectId;
    }

    public float GetColliderSize()
    {
        return _colliderSize;
    }
}
