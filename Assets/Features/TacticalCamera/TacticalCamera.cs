using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Ezhtellar.AI;
using State = Ezhtellar.AI.State;
using StateMachine = Ezhtellar.AI.StateMachine;

namespace Ezhtellar.Genesis
{
    public class TacticalCamera : MonoBehaviour
    {

        [SerializeField] private CinemachineCamera m_camera;
        [SerializeField] private CinemachineTargetGroup m_unitsGuide;
        [SerializeField] private GameObject m_panningGuide;
        [SerializeField] private float m_edgeThreshold = 8;
        [SerializeField] private float m_panSpeed = 5;
        [SerializeField] private bool m_showPanningGuideVisuals;
        [SerializeField] private InputActionAsset m_inputActionAsset;
        [SerializeField] private PanningEdgeGuide m_panningEdgeGuide;

        private InputAction m_panningAction;

        private InputActionMap m_tacticalModeActionMap;
        private StateMachine m_cameraStateMachine;

        private void Awake() => m_tacticalModeActionMap = m_inputActionAsset.FindActionMap("TacticalMode");
        
        private void Start()
        {
            m_panningAction = m_inputActionAsset.FindAction("Pointer");
            SetDebugVisuals();
            m_cameraStateMachine.Start();
        }

        private void Update()
        {
            m_cameraStateMachine.Update();
        }

        private void OnEnable()
        {
            BuildStateMachine();            
            m_tacticalModeActionMap.Enable();
        }

        private void OnDisable()
        {
            m_cameraStateMachine.Stop();
            m_tacticalModeActionMap.Disable();
        }

        private void OnValidate() => SetDebugVisuals();

        private Vector3 GetPanningDirection(Vector3 pointerPosition)
        {
            Vector3 horizontal = Vector3.zero;
            Vector3 vertical = Vector3.zero;

            // Note: Panning is prevented when pointer is out of bounds of game screen
            bool isPanningLeft = pointerPosition.x >= 0f && pointerPosition.x <= m_edgeThreshold;
            bool isPanningRight = pointerPosition.x >= Screen.width - m_edgeThreshold &&
                                  pointerPosition.x <= Screen.width;
            bool isPanningDown = pointerPosition.y >= 0f && pointerPosition.y <= m_edgeThreshold;
            bool isPanningUp = pointerPosition.y >= Screen.height - m_edgeThreshold &&
                               pointerPosition.y <= Screen.height;

            if (isPanningLeft)
            {
                horizontal = Vector3.left;
            }
            else if (isPanningRight)
            {
                horizontal = Vector3.right;
            }

            if (isPanningDown)
            {
                vertical = Vector3.back;
            }
            else if (isPanningUp)
            {
                vertical = Vector3.forward;
            }

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

        private void BuildStateMachine()
        {
            Vector3 m_panningDirection = Vector3.zero;
            
            var camera = new State.Builder()
                .WithName("TacticalCamera")
                .WithOnUpdate(() =>
                {
                    Vector3 pointerPosition = m_panningAction.ReadValue<Vector2>();                    
                    m_panningDirection = GetPanningDirection(pointerPosition);                    
                })
                .Build();

            m_cameraStateMachine = StateMachine.FromState(camera);
            
            var idle = new State.Builder()
                .WithName("Idle")
                .WithOnUpdate(() =>
                {
                })
                .Build();
            
            
            var panning = new State.Builder()
                .WithName("Panning")
                .WithOnUpdate(() =>
                {
                    if (!Application.isFocused) { return; }
                    FollowPanningGuide();
                    m_panningGuide.transform.position += m_panningDirection * (m_panSpeed * Time.deltaTime);                    
                })
                .Build();
            
            // The editor window (or another application) has focus.
            // Reset any panning state to avoid it starting when focus returns.
            panning.AddTransition(new Transition(idle, () => m_panningDirection == Vector3.zero));
            
            idle.AddTransition(new Transition(panning, () => m_panningDirection != Vector3.zero));
            
            var following = new State.Builder()
                .WithName("Following")
                .Build();
            
            m_cameraStateMachine.AddState(idle, isInitial: true);
            m_cameraStateMachine.AddState(panning);
            m_cameraStateMachine.AddState(following);
        }
    }
}