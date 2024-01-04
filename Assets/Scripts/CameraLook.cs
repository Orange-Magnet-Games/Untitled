using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    private PlayerController player;
    private Vector2 input;
    [SerializeField, Range(0, 1)] private float sensitivity = 1.0f;
    private void Start()
    {
        player = Manager.instance.player;
        player.input.Camera.Look.performed += ctx => OnLook(ctx, true);
        player.input.Camera.Look.canceled += ctx => OnLook(ctx, false);

    }

    private void OnLook(InputAction.CallbackContext ctx, bool performed)
    {
        input = performed ? ctx.ReadValue<Vector2>() : Vector2.zero;
    }
    void Update()
    {
        player.transform.eulerAngles += new Vector3(0, input.x * sensitivity, 0);
        var eulerAngles = transform.eulerAngles;
        
        eulerAngles -= new Vector3(input.y * sensitivity, 0, 0);
        eulerAngles = new Vector3(ClampAngle(eulerAngles.x, -90, 90), eulerAngles.y, eulerAngles.z);
        
        transform.eulerAngles = eulerAngles;
    }

    private static float ClampAngle(float angle, float min, float max) {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}
