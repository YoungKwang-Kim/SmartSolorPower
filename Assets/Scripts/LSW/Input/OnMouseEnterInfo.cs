using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseEnterInfo : MonoBehaviour
{
    public Canvas infoCanvas;

    private void Start()
    {
        StartCoroutine(InitialDelay());
    }

    private IEnumerator InitialDelay()
    {
        Time.timeScale = 0f;
        // ó�� 2�� ������ ����
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;

        // ī�޶� ��¦ �����̰ų� �ƹ� Ű�� ������ ĵ���� ��Ȱ��ȭ
        yield return StartCoroutine(WaitForCameraMovementOrAnyKey());
        SetCanvasAndChildrenActive(false);
    }

    private IEnumerator WaitForCameraMovementOrAnyKey()
    {
        Vector3 initialCameraPosition = Camera.main.transform.position;

        // ��� �ð� ���� ī�޶� �ణ �����̴� ���� üũ
        Vector3 targetCameraPosition = new Vector3(initialCameraPosition.x + 0.1f, initialCameraPosition.y, initialCameraPosition.z);
        float elapsedTime = 0f;

        while (elapsedTime < 0.1f)
        {
            Camera.main.transform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition, elapsedTime / 0.1f);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // �ƹ� Ű �Է��� ���� ������ ���
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
    }

    private void OnMouseEnter()
    {
        SetCanvasAndChildrenActive(true);
    }

    private void OnMouseExit()
    {
        SetCanvasAndChildrenActive(false);
    }

    private void SetCanvasAndChildrenActive(bool active)
    {
        infoCanvas.enabled = active;

        // �ڽ� ������Ʈ�鿡 ���� Ȱ��ȭ/��Ȱ��ȭ ó��
        foreach (Transform child in infoCanvas.transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}