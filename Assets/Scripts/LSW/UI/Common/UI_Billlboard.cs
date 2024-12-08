using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Billboard : MonoBehaviour
{
    public Transform target;
    [Tooltip("�������ϴ� ������ ũ�⸦ �Է��ϼ���.")]
    public float scaleAdjustable = 0.3f;
    private float scaleMultiplier = 0.01f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position) * scaleAdjustable;

        float newScale = distance * scaleMultiplier;

        transform.localScale = new Vector3(newScale, newScale, 0);

        transform.forward = target.forward;
    }
}