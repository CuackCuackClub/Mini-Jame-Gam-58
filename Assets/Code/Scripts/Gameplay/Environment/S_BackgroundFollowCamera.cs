using UnityEngine;

[DefaultExecutionOrder(200)]
public class S_BackgroundFollowCamera : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private SpriteRenderer skyRenderer;

    [SerializeField, Min(1f)]
    private float coverMargin = 1.08f;

    private void Awake()
    {
        Cache();
    }

    private void LateUpdate()
    {
        FollowAndCover();
    }

    private void Cache()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (skyRenderer == null)
        {
            skyRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void FollowAndCover()
    {
        Cache();
        if (targetCamera == null || skyRenderer == null || skyRenderer.sprite == null)
        {
            return;
        }

        Transform cameraTransform = targetCamera.transform;
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z
        );

        Bounds spriteBounds = skyRenderer.sprite.bounds;
        float spriteWidth = Mathf.Max(0.01f, spriteBounds.size.x);
        float spriteHeight = Mathf.Max(0.01f, spriteBounds.size.y);
        float cameraHeight = targetCamera.orthographicSize * 2f * coverMargin;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        float scale = Mathf.Max(cameraWidth / spriteWidth, cameraHeight / spriteHeight);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
