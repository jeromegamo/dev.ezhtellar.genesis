using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class UnitFacingDirectionGizmo: MonoBehaviour
    {
        public void OnDrawGizmos()
        {
            var start = new Vector3(
                x: transform.position.x,
                y: transform.position.y + 1,
                z: transform.position.z
            );

            var forwardVector = transform.position + transform.forward;
            var end = new Vector3(
                x: forwardVector.x,
                y: forwardVector.y + 1,
                z: forwardVector.z
            );
       
            Debug.DrawLine(start, end, Color.red);
        }
    }
}