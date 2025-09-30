using System;
using Ezhtellar.Genesis;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class SceneInstaller : MonoBehaviour, IInstaller
    {
        TacticalMoveController m_tacticalMoveController;
        UnitsManager m_unitsManager;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton((_) =>
            {
                var go = InteractionReaderGO.Instantiate();
                go.transform.SetParent(transform);
                return go;
            }, typeof(IInteractionReader));

            containerBuilder.AddSingleton((_) =>
            {
                var go = SelectionReaderGO.Instantiate();
                go.transform.SetParent(transform);
                return go;
            }, typeof(ISelectionReader));

        }

        private void Awake()
        {
            var container = gameObject.scene.GetSceneContainer();
            var selectionReader = container.Resolve<ISelectionReader>(); 
            var interactionReader = container.Resolve<IInteractionReader>();
            
            var playableUnits = FindObjectsByType<UnitGO>(FindObjectsSortMode.None);
            Debug.Log($"Found {playableUnits.Length} units");
            m_unitsManager = new UnitsManager(playableUnits);
            
            var  go = TacticalUnitSelectionGO.Instantiate(m_unitsManager, selectionReader);
            go.transform.SetParent(transform);
            
            DiamondFormationGO diamondFormationGo = DiamondFormationGO.Instantiate();
            diamondFormationGo.transform.SetParent(transform); 
                
            m_tacticalMoveController = new TacticalMoveController(m_unitsManager, interactionReader,  diamondFormationGo);
        }
    }
}