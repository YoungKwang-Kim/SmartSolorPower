using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Material[] originalMaterials;
    private Material[] highlightedMaterials;
    public float scaleIncrease = 1f;
    public float smoothness = 7f; // ���� ������ �ε巯�� ���
    public Material highlightMaterial; // �̹� �����Ǿ� �ִ� "HighLightShader_1" ���׸���

    void Start()
    {
        // �ʱ� ũ�� ����
        originalScale = transform.localScale;
        // ��ǥ ũ�� ���
        targetScale = originalScale;

        // �ʱ� ���׸��� �迭 ����
        originalMaterials = GetComponent<Renderer>().materials;

        // ���̶���Ʈ ���׸��� �߰�
        highlightedMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            highlightedMaterials[i] = originalMaterials[i];
        }

        // ���� "HighLightShader_1" ���׸��� �߰�
        highlightedMaterials[originalMaterials.Length] = highlightMaterial;
    }

    void Update()
    {
        // ������ ����Ͽ� �ε巯�� ũ�� ��ȭ ����
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, smoothness * Time.deltaTime);
    }

    void OnMouseEnter()
    {
        // ���콺 ���� �� ��ǥ ũ�� �� ���׸��� ����
        targetScale = originalScale + new Vector3(0, scaleIncrease, 0);
        GetComponent<Renderer>().materials = highlightedMaterials;
    }

    void OnMouseExit()
    {
        // ���콺 ������ �ʱ� ũ��� ���� ���׸���� ����
        targetScale = originalScale;
        GetComponent<Renderer>().materials = originalMaterials;
    }
}