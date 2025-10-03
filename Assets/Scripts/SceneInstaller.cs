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
        TacticalAttackGO m_tacticalAttack;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton((_) =>
            {
                var playableUnits = FindObjectsByType<UnitGO>(FindObjectsSortMode.None);
                Debug.Log($"Found {playableUnits.Length} units");
                return new UnitsManager(playableUnits);
            });
            
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
            var unitsManager = container.Resolve<UnitsManager>();
            
            
            var  go = TacticalUnitSelectionGO.Instantiate(unitsManager, selectionReader);
            go.transform.SetParent(transform);
            
            DiamondFormationGO diamondFormationGo = DiamondFormationGO.Instantiate();
            diamondFormationGo.transform.SetParent(transform); 
                
            m_tacticalMoveController = new TacticalMoveController(unitsManager, interactionReader,  diamondFormationGo);

            m_tacticalAttack = TacticalAttackGO.Instantiate(interactionReader, unitsManager);
            m_tacticalAttack.transform.SetParent(transform);
        }
    }
}