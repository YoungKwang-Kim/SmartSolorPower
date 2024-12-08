using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public Canvas exitCanvas;
    private void Start()
    {
        exitCanvas.enabled = false;
    }
    public void GoToHome()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToPrevious()
    {
        // ���� ���� �ε��� ��������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���� ���� �ε��� ���
        int previousSceneIndex = currentSceneIndex - 1;

        // �ּ� �ε��� 0���� ����
        previousSceneIndex = Mathf.Clamp(previousSceneIndex, 0, SceneManager.sceneCountInBuildSettings - 1);

        // ���� ������ �̵�
        SceneManager.LoadScene(previousSceneIndex);
    }

    public void GoToNext()
    {
        // ���� ���� �ε��� ��������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���� ���� �ε��� ���
        int nextSceneIndex = currentSceneIndex + 1;

        // �ּ� �ε��� 0���� ����
        nextSceneIndex = Mathf.Clamp(nextSceneIndex, 0, 2);

        // ���� ������ �̵�
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void Exit()
    {
        exitCanvas.enabled = true;
        Time.timeScale = 0f;
    }

    public void Yes()
    {
        Application.Quit();
    }

    public void No()
    {
        exitCanvas.enabled = false;
        Time.timeScale = 1f;
    }
}