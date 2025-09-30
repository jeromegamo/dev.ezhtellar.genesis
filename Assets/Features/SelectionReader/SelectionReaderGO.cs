using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ezhtellar.Genesis
{
/*
    Note: Should readers only responsibility is to explicitly
    express the intention of the common inputs like started, performed,
    cancelled, WasPressed, IsPressing, WasReleased? Or should it be
    tightly coupled to logic that involves gameobjects where the inputs
    are being used against ie raycasted.
 */
    public class SelectionReaderGO : MonoBehaviour, ISelectionReader
    {
        [SerializeField] InputActionAsset m_inputActionAsset;

        InputAction m_pointerAction;
        InputActionMap m_tacticalModeActionMap;
        InputAction m_unitSelectAction;

        public event Action<Vector2> WillSelect;
        public event Action<Vector2> DraggingSelection;
        public event Action<Vector2> DidSelect;

        private void Awake()
        {
            m_tacticalModeActionMap = m_inputActionAsset.FindActionMap("TacticalMode");
        }

        private void OnEnable() => m_tacticalModeActionMap.Enable();

        private void OnDisable() => m_tacticalModeActionMap.Disable();

        void Start()
        {
            m_pointerAction = m_inputActionAsset.FindAction("Pointer");
            m_unitSelectAction = m_inputActionAsset.FindAction("UnitSelect");
        }

        // Update is called once per frame
        void Update()
        {
            if (m_unitSelectAction.WasPressedThisFrame())
            {
                OnWillSelect(GetPointerPosition());
            }
            else if (m_unitSelectAction.IsPressed() && !m_unitSelectAction.WasPressedThisFrame())
            {
                OnSelecting(GetPointerPosition());
            }
            else if (m_unitSelectAction.WasReleasedThisFrame())
            {
                OnDidSelect(GetPointerPosition());
            }
        }

        private void OnSelecting(Vector2 pointerPosition)
        {
            DraggingSelection?.Invoke(pointerPosition);
        }

        private void OnDidSelect(Vector2 pointerPosition)
        {
            DidSelect?.Invoke(pointerPosition);
        }

        private void OnWillSelect(Vector2 pointerPosition)
        {
            WillSelect?.Invoke(pointerPosition);
        }

        Vector2 GetPointerPosition() => m_pointerAction.ReadValue<Vector2>();

        public static SelectionReaderGO Instantiate()
        {
            SelectionReaderGO prefab = Resources.Load<SelectionReaderGO>("SelectionReader");
            prefab.gameObject.SetActive(false);
            SelectionReaderGO go = Instantiate(prefab);
            go.gameObject.SetActive(true);
            return go;
        }
    }
}