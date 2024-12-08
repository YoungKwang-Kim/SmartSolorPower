using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� ���� ��ο� ���¸� �����ϴ� ��Ʈ�ѷ� Ŭ����
/// </summary>
public class DroneController : MonoBehaviour
{
    #region Enums
    public enum DroneState
    {
        TakeOff,    // �̷�
        Flight,     // ����
        Return,     // ����
        Landing     // ����
    }
    #endregion

    #region Serialized Fields
    [Header("Drone Settings")]
    [SerializeField] private GameObject myDrone;
    [SerializeField] private float flightSpeed = 5f;
    [SerializeField] private float flightHeight = 10f;
    [SerializeField] private float readySpeed = 2.5f;

    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform waypointBase;

    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Canvas thermalCanvas;
    [SerializeField] private Canvas minimapCanvas;

    [Header("Camera")]
    [SerializeField] private DronCamController droneCamController;
    #endregion

    #region Private Fields
    private Animator propellerAnimator;
    private int currentWaypointIndex;
    private bool isDroneActive;
    private DroneState currentState;
    private const float DESTINATION_THRESHOLD = 1f;
    private const float LANDING_HEIGHT_THRESHOLD = 0.5f;
    private const float ROTATION_SPEED = 2f;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeDrone();
    }

    private void Update()
    {
        if (isDroneActive)
        {
            UpdateDroneState();
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// ��� �� ���� ������Ʈ �ʱ�ȭ
    /// </summary>
    private void InitializeDrone()
    {
        if (!ValidateComponents()) return;

        propellerAnimator = myDrone.GetComponent<Animator>();
        InitializeWaypoints();
        SetupUI();
    }

    /// <summary>
    /// �ʼ� ������Ʈ ��ȿ�� �˻�
    /// </summary>
    private bool ValidateComponents()
    {
        if (myDrone == null || waypointBase == null)
        {
            Debug.LogError("Required drone components are missing!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// ��������Ʈ �ʱ�ȭ
    /// </summary>
    private void InitializeWaypoints()
    {
        waypointBase.position = new Vector3(0, flightHeight, 0);

        foreach (Transform waypoint in waypoints)
        {
            waypoint.position = new Vector3(
                waypoint.position.x,
                flightHeight,
                waypoint.position.z
            );
        }
    }

    /// <summary>
    /// UI �ʱ�ȭ
    /// </summary>
    private void SetupUI()
    {
        currentState = DroneState.TakeOff;

        if (droneCamController != null)
            droneCamController.enabled = false;

        if (thermalCanvas != null)
            thermalCanvas.enabled = false;

        if (minimapCanvas != null)
            minimapCanvas.enabled = false;
    }
    #endregion

    #region Drone Control Methods
    /// <summary>
    /// ��� ���¿� ���� ���� ������Ʈ
    /// </summary>
    private void UpdateDroneState()
    {
        switch (currentState)
        {
            case DroneState.TakeOff:
                HandleTakeOff();
                break;
            case DroneState.Flight:
                HandleFlight();
                break;
            case DroneState.Return:
                HandleReturn();
                break;
            case DroneState.Landing:
                HandleLanding();
                break;
        }
    }

    /// <summary>
    /// �̷� ó��
    /// </summary>
    private void HandleTakeOff()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints set!");
            return;
        }

        StartPropellers();
        myDrone.transform.Translate(Vector3.up * readySpeed * Time.deltaTime);

        if (myDrone.transform.position.y > flightHeight)
        {
            currentState = DroneState.Flight;
        }
    }

    /// <summary>
    /// ���� ó��
    /// </summary>
    private void HandleFlight()
    {
        if (Vector3.Distance(waypoints[currentWaypointIndex].position, myDrone.transform.position) > DESTINATION_THRESHOLD)
        {
            MoveToTarget(waypoints[currentWaypointIndex].position);
        }
        else
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentState = DroneState.Return;
            }
        }
    }

    /// <summary>
    /// ���� ó��
    /// </summary>
    private void HandleReturn()
    {
        MoveToTarget(waypointBase.position);

        if (Vector3.Distance(waypointBase.position, myDrone.transform.position) < DESTINATION_THRESHOLD)
        {
            currentState = DroneState.Landing;
        }
    }

    /// <summary>
    /// ���� ó��
    /// </summary>
    private void HandleLanding()
    {
        myDrone.transform.Translate(Vector3.down * readySpeed * Time.deltaTime);

        if (myDrone.transform.position.y < LANDING_HEIGHT_THRESHOLD)
        {
            readySpeed = 0;
            StopPropellers();
        }
    }

    /// <summary>
    /// ��ǥ �������� ��� �̵�
    /// </summary>
    private void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - myDrone.transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        myDrone.transform.rotation = Quaternion.RotateTowards(
            myDrone.transform.rotation,
            targetRotation,
            ROTATION_SPEED
        );

        myDrone.transform.Translate(Vector3.forward * flightSpeed * Time.deltaTime);
    }
    #endregion

    #region Propeller Control
    /// <summary>
    /// �����緯 ����
    /// </summary>
    private void StartPropellers()
    {
        if (propellerAnimator == null)
        {
            Debug.LogWarning("Propeller animator not set!");
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            propellerAnimator.SetLayerWeight(i, 1);
        }
    }

    /// <summary>
    /// �����緯 ����
    /// </summary>
    private void StopPropellers()
    {
        if (propellerAnimator == null)
        {
            Debug.LogWarning("Propeller animator not set!");
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            propellerAnimator.SetLayerWeight(i, 0);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// ��� ���� ��ư Ŭ�� ó��
    /// </summary>
    public void OnStartButtonClick()
    {
        isDroneActive = true;

        if (droneCamController != null)
            droneCamController.enabled = true;

        if (thermalCanvas != null)
            thermalCanvas.enabled = true;

        if (minimapCanvas != null)
            minimapCanvas.enabled = true;
    }
    #endregion
}