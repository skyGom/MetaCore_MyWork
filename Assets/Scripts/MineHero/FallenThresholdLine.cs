using UnityEngine;

namespace Dasverse.Aleo
{
    public class FallenThresholdLine : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Crystal"))
            {
                CrystalSpawner.Instance.ReturnPool(collision.GetComponentInParent<Crystal>());
            }
        }
    }
}