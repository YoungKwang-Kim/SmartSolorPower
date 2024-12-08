using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressToNextScene : MonoBehaviour
{
    private Material[] originalMaterials;
    private Material[] pressMaterials;

    public Material pressMaterial;

    private void Start()
    {
        // �ʱ� ���׸��� �迭 ����
        originalMaterials = GetComponent<Renderer>().materials;

        // ���̶���Ʈ ���׸��� �߰�
        pressMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            pressMaterials[i] = originalMaterials[i];
        }

        // ���� "HighLightShader_1" ���׸��� �߰�
        pressMaterials[originalMaterials.Length] = pressMaterial;
    }

    private void OnMouseDown()
    {
        // ���׸��� ����
        GetComponent<Renderer>().materials = pressMaterials;
    }
    private void OnMouseUp()
    {
        // ���� ������ �̵�
        SceneManager.LoadScene(1);
    }
}