using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderButton : MonoBehaviour
{
    public Vector3 hoverScale = new Vector3(1.7f, 1.7f, 1.7f);
    public Vector3 hoverPosition = new Vector3(142.07f, -8.2041f, 96.441f);
    public float lerpSpeed = 2f;
    public bool nextScene = false;

    private Vector3 initialScale;
    private Vector3 initialPosition;

    // ���� ���� ���� �ڷ�ƾ�� �����ϱ� ���� ���� ���� �߰�
    private Coroutine lerpCoroutine;

    private void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    private void OnMouseEnter()
    {
        Debug.Log("���콺 ȣ��");
        // ������ �ڷ�ƾ�� �ִٸ� ����
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        // ũ�⸦ Ű��� �ڷ�ƾ ����
        lerpCoroutine = StartCoroutine(LerpScale(hoverScale, hoverPosition));
    }

    private void OnMouseExit()
    {
        // ������ �ڷ�ƾ�� �ִٸ� ����
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        // ũ�⸦ ���̴� �ڷ�ƾ ����
        lerpCoroutine = StartCoroutine(LerpScale(initialScale, initialPosition));
    }

    private IEnumerator LerpScale(Vector3 targetScale, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;
        Vector3 startingPosition = transform.localPosition;

        while (elapsedTime < lerpSpeed)
        {
            transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / lerpSpeed);
            transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / lerpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure the target scale is reached
        transform.localPosition = targetPosition;
    }

    private void OnMouseDown()
    {
        if(nextScene == true)
        {
            SceneManager.LoadScene(2);
        }
    }
}