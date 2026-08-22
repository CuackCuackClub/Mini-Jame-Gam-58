using UnityEngine;

public class S_BloodVialPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        S_BloodVialInventory inventory = other.GetComponentInParent<S_BloodVialInventory>();
        if (inventory == null)
        {
            return;
        }

        if (!inventory.AddVial())
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
