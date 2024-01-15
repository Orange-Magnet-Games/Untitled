using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spin : MonoBehaviour {
    [SerializeField] private Vector3 spin;
    [SerializeField] private float speed;
    private void FixedUpdate() {
        transform.Rotate(spin, speed, Space.Self);
    }
}
