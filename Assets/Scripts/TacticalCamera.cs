using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TacticalCamera : MonoBehaviour
{
    public enum State
    {
        Panning,
        Idle,
        Following
    }

    [SerializeField] private CinemachineCamera m_camera;
    [SerializeField] private CinemachineTargetGroup m_unitsGuide;
    [SerializeField] private GameObject m_panningGuide;
    [SerializeField] private float m_edgeThreshold = 8;
    [SerializeField] private float m_panSpeed = 5;
    [SerializeField] private bool m_showPanningGuideVisuals;
    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private PanningEdgeGuide m_panningEdgeGuide;

    private InputAction m_panningAction;
    private Vector3 m_panningDirection;

    private State m_state;
    private InputActionMap m_tacticalModeActionMap;

    private void Awake() => m_tacticalModeActionMap = m_inputActionAsset.FindActionMap("TacticalMode");

    private void Start()
    {
        m_panningAction = m_inputActionAsset.FindAction("Pointer");
        m_state = State.Idle;
        SetDebugVisuals();
    }

    private void Update()
    {
        StateSwitcher();
        UpdatePanningDirection();
    }

    private void OnEnable() => m_tacticalModeActionMap.Enable();

    private void OnDisable() => m_tacticalModeActionMap.Disable();

    private void OnValidate() => SetDebugVisuals();

    private void UpdatePanningDirection()
    {
        // Only process input if the game window has focus
        if (!Application.isFocused)
        {
            // The editor window (or another application) has focus.
            // Reset any panning state to avoid it starting when focus returns.
            m_state = State.Idle;
            return;
        }

        Vector2 pointerPosition = m_panningAction.ReadValue<Vector2>();
        m_panningDirection = GetPanningDirection(pointerPosition);

        if (m_panningDirection != Vector3.zero)
        {
            m_state = State.Panning;
        }
    }

    private void StateSwitcher()
    {
        switch (m_state)
        {
            case State.Panning:
                FollowPanningGuide();
                m_panningGuide.transform.position += m_panningDirection * (m_panSpeed * Time.deltaTime);
                break;
        }
    }

    private Vector3 GetPanningDirection(Vector3 pointerPosition)
    {
        Vector3 horizontal = Vector3.zero;
        Vector3 vertical = Vector3.zero;

        // Note: Panning is prevented when pointer is out of bounds of game screen
        bool isPanningLeft = pointerPosition.x >= 0f && pointerPosition.x <= m_edgeThreshold;
        bool isPanningRight = pointerPosition.x >= Screen.width - m_edgeThreshold && pointerPosition.x <= Screen.width;
        bool isPanningDown = pointerPosition.y >= 0f && pointerPosition.y <= m_edgeThreshold;
        bool isPanningUp = pointerPosition.y >= Screen.height - m_edgeThreshold && pointerPosition.y <= Screen.height;

        if (isPanningLeft) { horizontal = Vector3.left; }
        else if (isPanningRight) { horizontal = Vector3.right; }

        if (isPanningDown) { vertical = Vector3.back; }
        else if (isPanningUp) { vertical = Vector3.forward; }

        return horizontal + vertical;
    }

    private void FollowPanningGuide() => m_camera.Follow = m_panningGuide.transform;

    private void FollowUnitsGuide() => m_camera.Follow = m_unitsGuide.transform;

    private void SetDebugVisuals()
    {
        m_panningGuide.GetComponent<MeshRenderer>()?.gameObject.SetActive(m_showPanningGuideVisuals);
        m_panningEdgeGuide.gameObject.SetActive(m_showPanningGuideVisuals);
        m_panningEdgeGuide.SetThresholdSize(m_edgeThreshold);
    }
}
