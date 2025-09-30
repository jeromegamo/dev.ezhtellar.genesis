using System;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class FormationSlot : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}