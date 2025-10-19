using UnityEngine;

namespace Ezhtellar.Genesis
{
public class TacticalAttackGO : MonoBehaviour
{
   UnitsManager m_unitsManager;
   IInteractionReader m_interactionReader;
   
   public static TacticalAttackGO Instantiate(
      IInteractionReader interactionReader,
      UnitsManager unitsManager
   )
   {
      TacticalAttackGO prefab = Resources.Load<TacticalAttackGO>("TacticalAttack");
      prefab.gameObject.SetActive(false);
      TacticalAttackGO go = Instantiate(prefab);
      go.SetDependencies(interactionReader, unitsManager);
      go.gameObject.SetActive(true);
      return go;
   }

   public void SetDependencies(
      IInteractionReader interactionReader, 
      UnitsManager unitsManager)
   {
      m_interactionReader = interactionReader;
      m_unitsManager = unitsManager;
      m_interactionReader.WillAttack += InteractionReader_WillAttack;
   }

   private void InteractionReader_WillAttack(Unit target)
   {
      foreach (var unit in m_unitsManager.SelectedUnits)
      {
        unit.SetTarget(target); 
      }
   }

   // move selected units to the target
   // if the target is in range, play attack animation and
   // damage the damageable unit
}
}
