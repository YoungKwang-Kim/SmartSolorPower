using UnityEngine;

/// <summary>
/// 드론을 따라다니는 카메라의 동작을 제어하는 컨트롤러
/// </summary>
public class DronCamController : MonoBehaviour
{
    #region Camera Settings
    [Header("Follow Settings")]
    [Tooltip("카메라와 드론 사이의 거리")]
    [SerializeField] private float distance = 2f;

    [Tooltip("카메라의 상대적 높이")]
    [SerializeField] private float height = 3f;

    [Tooltip("높이 변화의 부드러움 정도")]
    [Range(0.1f, 10f)]
    [SerializeField] private float heightDamping = 2.0f;

    [Tooltip("회전 변화의 부드러움 정도")]
    [Range(0.1f, 10f)]
    [SerializeField] private float rotationDamping = 3.0f;
    #endregion

    #region References
    [Header("Target Reference")]
    [Tooltip("추적할 드론 객체")]
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
    /// 카메라 초기 설정
    /// </summary>
    private void InitializeCamera()
    {
        cameraTransform = transform;
        ValidateComponents();
    }

    /// <summary>
    /// 필수 컴포넌트 검증
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
    /// 타겟 유효성 검사
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
    /// 카메라 위치와 회전 업데이트
    /// </summary>
    private void UpdateCameraPosition()
    {
        // 목표 회전각과 높이 계산
        float targetRotationAngle = target.eulerAngles.y;
        float targetHeight = target.position.y + height;

        // 현재 카메라의 회전각과 높이
        float currentRotationAngle = cameraTransform.eulerAngles.y;
        float currentHeight = cameraTransform.position.y;

        // 부드러운 회전과 높이 변화 적용
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

        // 카메라 위치 업데이트
        UpdateCameraTransform(currentRotationAngle, currentHeight);
    }

    /// <summary>
    /// 계산된 값을 기반으로 카메라 트랜스폼 업데이트
    /// </summary>
    private void UpdateCameraTransform(float rotationAngle, float heightValue)
    {
        // 회전 계산
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);

        // 위치 계산
        Vector3 targetPosition = target.position;
        Vector3 newPosition = targetPosition - (rotation * Vector3.forward * distance);
        newPosition.y = heightValue;

        // 트랜스폼 적용
        cameraTransform.position = newPosition;
        cameraTransform.LookAt(targetPosition);
    }
    #endregion
}