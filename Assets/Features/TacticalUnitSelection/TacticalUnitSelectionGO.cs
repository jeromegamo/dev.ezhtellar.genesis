using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Ezhtellar.Genesis
{
    public class TacticalUnitSelectionGO : MonoBehaviour
    {
        [SerializeField] private LayerMask m_unitLayerMask;
        [SerializeField] private RectTransform m_selectionBox;

        private UnitsManager m_unitsManager;
        private Vector2 m_selectionBoxEndPosition = Vector2.zero;
        private Vector2 m_selectionBoxStartPosition = Vector2.zero;
        ISelectionReader m_selectionReader;

        public static TacticalUnitSelectionGO Instantiate(UnitsManager unitsManager, ISelectionReader selectionReader)
        {
            var resource = Resources.Load<TacticalUnitSelectionGO>("TacticalUnitSelection");
            resource.gameObject.SetActive(false);
            TacticalUnitSelectionGO go = Instantiate(resource);
            go.SetDependencies(unitsManager, selectionReader);
            go.gameObject.SetActive(true);
            return go;
        }

        private void SetDependencies(UnitsManager unitsManager, ISelectionReader selectionReader)
        {
            m_unitsManager = unitsManager;
            m_selectionReader = selectionReader;
            m_selectionReader.WillSelect += SelectionReader_WillSelect;
            m_selectionReader.DraggingSelection += SelectionReader_DraggingSelection;
            m_selectionReader.DidSelect += SelectionReader_DidSelect;
        }

        private void SelectionReader_DidSelect(Vector2 pointerPosition)
        {
            // clear all selection
            m_unitsManager.DeselectAllUnits();

            SelectSingleUnit(pointerPosition);

            bool unitGotSelected = m_unitsManager.SelectedUnits.Count() == 1;

            if (unitGotSelected)
            {
                // FollowCameraGroup();
                return;
            }

            SelectMultipleUnits(pointerPosition);

            // FollowCameraGroup();


            // reset ui to default
            ResetSelectionBox();
        }

        private void SelectionReader_DraggingSelection(Vector2 pointerPosition)
        {
            ResizeSelectionBox(pointerPosition);
        }

        private void SelectionReader_WillSelect(Vector2 pointerPosition)
        {
            StartSelectionBox(pointerPosition);
        }

        private void StartSelectionBox(Vector2 pointerPosition)
        {
            m_selectionBox.gameObject.SetActive(true);
            m_selectionBoxStartPosition = pointerPosition;
        }

        private void SelectSingleUnit(Vector2 pointerPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, m_unitLayerMask)
                && hit.collider.TryGetComponent(out UnitGO unit))
            {
                unit.Select();
            }
        }

        private void SelectMultipleUnits(Vector2 pointerPosition)
        {
            ResizeSelectionBox(pointerPosition);

            Bounds bounds = new(m_selectionBox.anchoredPosition, m_selectionBox.sizeDelta);

            foreach (IUnit unit in m_unitsManager.PlayableUnits)
            {
                Vector2 point = Camera.main.WorldToScreenPoint(unit.Position);
                if (bounds.Contains(point))
                {
                    unit.Select();
                }
            }
        }

        private void ResizeSelectionBox(Vector2 pointerPosition)
        {
            m_selectionBoxEndPosition = pointerPosition;

            float width = m_selectionBoxEndPosition.x - m_selectionBoxStartPosition.x;
            float height = m_selectionBoxEndPosition.y - m_selectionBoxStartPosition.y;

            m_selectionBox.position = m_selectionBoxStartPosition;

            m_selectionBox.anchoredPosition = m_selectionBoxStartPosition + new Vector2(
                width / 2,
                height / 2
            );

            m_selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        }

        private void ResetSelectionBox()
        {
            m_selectionBox.gameObject.SetActive(false);
            m_selectionBoxStartPosition = Vector2.zero;
            m_selectionBoxEndPosition = Vector2.zero;
            m_selectionBox.sizeDelta = Vector2.zero;
            m_selectionBox.anchoredPosition = Vector2.zero;
            m_selectionBox.position = Vector2.zero;
        }
    }
}