using UnityEngine;

/// <summary>
/// ����� ����ٴϴ� ī�޶��� ������ �����ϴ� ��Ʈ�ѷ�
/// </summary>
public class DronCamController : MonoBehaviour
{
    #region Camera Settings
    [Header("Follow Settings")]
    [Tooltip("ī�޶�� ��� ������ �Ÿ�")]
    [SerializeField] private float distance = 2f;

    [Tooltip("ī�޶��� ����� ����")]
    [SerializeField] private float height = 3f;

    [Tooltip("���� ��ȭ�� �ε巯�� ����")]
    [Range(0.1f, 10f)]
    [SerializeField] private float heightDamping = 2.0f;

    [Tooltip("ȸ�� ��ȭ�� �ε巯�� ����")]
    [Range(0.1f, 10f)]
    [SerializeField] private float rotationDamping = 3.0f;
    #endregion

    #region References
    [Header("Target Reference")]
    [Tooltip("������ ��� ��ü")]
    [SerializeField] private Transform target;

    private Transform cameraTransform;
    #endregion

    #region Cached Values
    private Vector3 currentVelocity;
    private float currentRotationVelocity;
    private float currentHeightVelocity;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeCamera();
    }

    private void LateUpdate()
    {
        if (!ValidateTarget()) return;
        UpdateCameraPosition();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// ī�޶� �ʱ� ����
    /// </summary>
    private void InitializeCamera()
    {
        cameraTransform = transform;
        ValidateComponents();
    }

    /// <summary>
    /// �ʼ� ������Ʈ ����
    /// </summary>
    private void ValidateComponents()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to DronCamController!");
            enabled = false;
        }
    }
    #endregion

    #region Camera Movement
    /// <summary>
    /// Ÿ�� ��ȿ�� �˻�
    /// </summary>
    private bool ValidateTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target is missing!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// ī�޶� ��ġ�� ȸ�� ������Ʈ
    /// </summary>
    private void UpdateCameraPosition()
    {
        // ��ǥ ȸ������ ���� ���
        float targetRotationAngle = target.eulerAngles.y;
        float targetHeight = target.position.y + height;

        // ���� ī�޶��� ȸ������ ����
        float currentRotationAngle = cameraTransform.eulerAngles.y;
        float currentHeight = cameraTransform.position.y;

        // �ε巯�� ȸ���� ���� ��ȭ ����
        currentRotationAngle = Mathf.SmoothDampAngle(
            currentRotationAngle,
            targetRotationAngle,
            ref currentRotationVelocity,
            1 / rotationDamping
        );

        currentHeight = Mathf.SmoothDamp(
            currentHeight,
            targetHeight,
            ref currentHeightVelocity,
            1 / heightDamping
        );

        // ī�޶� ��ġ ������Ʈ
        UpdateCameraTransform(currentRotationAngle, currentHeight);
    }

    /// <summary>
    /// ���� ���� ������� ī�޶� Ʈ������ ������Ʈ
    /// </summary>
    private void UpdateCameraTransform(float rotationAngle, float heightValue)
    {
        // ȸ�� ���
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);

        // ��ġ ���
        Vector3 targetPosition = target.position;
        Vector3 newPosition = targetPosition - (rotation * Vector3.forward * distance);
        newPosition.y = heightValue;

        // Ʈ������ ����
        cameraTransform.position = newPosition;
        cameraTransform.LookAt(targetPosition);
    }
    #endregion
}