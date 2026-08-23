using UnityEngine;

public class S_HitImpact : MonoBehaviour
{
    [SerializeField, Min(0.05f)]
    private float lifetime = 0.6f;

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }

    public static void Spawn(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
        {
            return;
        }

        Instantiate(prefab, position, Quaternion.identity);
    }
}
