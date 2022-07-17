using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotation = Vector3.up;

    private void Update()
    {
        this.transform.localRotation *= Quaternion.Euler(_rotation * Time.deltaTime);
    }
}
