using System;
using UnityEngine;

namespace Common
{
    public class DetectionRadius: MonoBehaviour
    {
        [SerializeField] private float radius = 5f;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}