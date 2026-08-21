using UnityEngine;
using UnityEngine.InputSystem;
public class S_PlayerManagement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
   private Rigidbody2D rBody;
   
   private void Awake()
   {
    rBody = GetComponent<Rigidbody2D>();
   }

    private void Update()
    {
        rBody.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, rBody.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rBody.linearVelocity.y) < 0.001f)
        {
            rBody.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    //private 
}
