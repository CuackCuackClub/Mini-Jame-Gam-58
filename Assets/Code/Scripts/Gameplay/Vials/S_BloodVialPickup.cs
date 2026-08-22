using UnityEngine;

public class S_BloodVialPickup : MonoBehaviour
{
    private bool collected;

    private void OnEnable()
    {
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other == null)
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

        collected = true;
        gameObject.SetActive(false);
    }
}
