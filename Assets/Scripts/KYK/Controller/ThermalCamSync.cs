using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalCamSync : MonoBehaviour
{
    private Transform mainCameraTransform;
    public Transform droneCamera;

    private void Start()
    {
        // ���� ī�޶��� Transform�� ã���ϴ�.
        mainCameraTransform = droneCamera.transform;

        // ���� ���� ī�޶� ���ٸ� ��� ǥ���մϴ�.
        if (mainCameraTransform == null)
        {
            Debug.LogWarning("Main Camera not found!");
        }
    }

    private void Update()
    {
        // ���� ī�޶��� position�� rotation ���� ���� ī�޶� �����մϴ�.
        if (mainCameraTransform != null)
        {
            transform.position = mainCameraTransform.position;
            transform.rotation = Quaternion.Euler(30, mainCameraTransform.rotation.y, 0);
        }
    }
}