using System;
using Ezhtellar.Genesis;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class SceneInstaller : MonoBehaviour, IInstaller
    {
        TacticalMoveReader m_tacticalMoveReader;
        TacticalAttackGO m_tacticalAttack;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton((_) => new UnitsManager());
            
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
                
            m_tacticalMoveReader = new TacticalMoveReader(unitsManager, interactionReader,  diamondFormationGo);

            m_tacticalAttack = TacticalAttackGO.Instantiate(interactionReader, unitsManager);
            m_tacticalAttack.transform.SetParent(transform);
        }
    }
}