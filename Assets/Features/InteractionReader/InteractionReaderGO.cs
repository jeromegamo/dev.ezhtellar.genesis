using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ezhtellar.Genesis
{
    public class InteractionReaderGO : MonoBehaviour, IInteractionReader
    {
        [SerializeField] InputActionAsset m_inputActionAsset;
        [SerializeField] private float m_rotationSensitivyFactor = 0.2f;

        InputActionMap m_inputActionMap;
        InputAction m_pointerAction;
        InputAction m_moveAction;
        float m_maxRaycastDistance = 50f;
        int m_groundLayerMaskIndex;
        Vector2 m_lastPointerPosition;
        InteractionResult m_lastInteractionResult;

        public event Action<Unit> WillAttack;
        public event Action<Vector3> WillMove;
        public event Action<Vector3> WillSetFormation;
        public event Action<Vector3> RotatingFormation;

        private void Awake()
        {
            m_inputActionMap = m_inputActionAsset.FindActionMap("TacticalMode");
            m_groundLayerMaskIndex = LayerMask.NameToLayer("Ground");
        }

        private void OnEnable()
        {
            m_inputActionMap.Enable();
        }

        private void Start()
        {
            m_pointerAction = m_inputActionMap.FindAction("Pointer");
            m_moveAction = m_inputActionMap.FindAction("Move");
        }

        private void OnDisable()
        {
            m_inputActionMap.Disable();
        }

        private Vector2 GetPointerPosition()
        {
            return m_pointerAction.ReadValue<Vector2>();
        }

        public void Update()
        {
            /* Translate if world interaction is against a navigable environment,
               interactable object, or attackable unit
             */
            if (m_moveAction.WasPressedThisFrame())
            {
                m_lastPointerPosition = GetPointerPosition();

                Ray cameraRay = Camera.main.ScreenPointToRay(m_lastPointerPosition);
                RaycastHit[] hits = Physics.RaycastAll(cameraRay, m_maxRaycastDistance);

                foreach (var hit in hits.TakeWhile(_ =>
                             m_lastInteractionResult.TargetUnit == null
                         ))
                {
                    if (m_lastInteractionResult.TargetUnit == null)
                    {
                        m_lastInteractionResult.TargetUnit = hit
                            .transform.gameObject.GetComponent<UnitController>()?.Unit;
                    }

                    if (!m_lastInteractionResult.PositionToMove.HasValue &&
                        hit.collider.gameObject.layer == m_groundLayerMaskIndex)
                    {
                        m_lastInteractionResult.PositionToMove = hit.point;
                    }
                }

                Debug.DrawRay(cameraRay.origin, cameraRay.direction * 50f, Color.red, 1f);

                switch (m_lastInteractionResult)
                {
                    case { TargetUnit: null, PositionToMove: { } position }:
                        OnWillSetFormation(position);
                        break;
                }

                ;
            }
            else if (m_moveAction.IsPressed() && !m_moveAction.WasPressedThisFrame() &&
                     m_lastInteractionResult.PositionToMove.HasValue)
            {
                Vector2 currentPointerPosition = GetPointerPosition();

                // Calculate the delta (change) since the last frame.
                Vector2 delta = currentPointerPosition - m_lastPointerPosition;

                // Rotate around Y based on horizontal pointer movement (delta.x).
                float rotationAmount = delta.x * m_rotationSensitivyFactor * Time.deltaTime;

                var formationRotation = new Vector3(0f, rotationAmount, 0f);

                OnRotatingFormation(formationRotation);

                // Update the last position for the next frame.
                m_lastPointerPosition = currentPointerPosition;
            }
            else if (m_moveAction.WasReleasedThisFrame())
            {
                switch (m_lastInteractionResult)
                {
                    case { TargetUnit: { } damageable }:
                        OnWillAttack(damageable);
                        break;
                    case { TargetUnit: null, PositionToMove: { } position }:
                        OnWillMove(position);
                        break;
                }

                ;

                m_lastInteractionResult.Reset();
            }
        }

        void OnWillSetFormation(Vector3 formationPosition)
        {
            WillSetFormation?.Invoke(formationPosition);
        }

        void OnWillAttack(Unit damageable)
        {
            WillAttack?.Invoke(damageable);
        }

        void OnWillMove(Vector2 position)
        {
            WillMove?.Invoke(position);
        }

        void OnRotatingFormation(Vector3 formationRotation)
        {
            RotatingFormation?.Invoke(formationRotation);
        }

        public static InteractionReaderGO Instantiate()
        {
            InteractionReaderGO interactionReader = Resources.Load<InteractionReaderGO>("InteractionReader");
            interactionReader.gameObject.SetActive(false);
            InteractionReaderGO go = Instantiate(interactionReader);
            go.gameObject.SetActive(true);
            return go;
        }


        struct InteractionResult
        {
            public Vector3? PositionToMove;
            public Unit TargetUnit;

            public void Reset()
            {
                PositionToMove = null;
                TargetUnit = null;
            }
        }
    }
}