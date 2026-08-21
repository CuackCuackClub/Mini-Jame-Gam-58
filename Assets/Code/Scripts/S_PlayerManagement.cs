using UnityEngine;
using UnityEngine.InputSystem;
public class S_PlayerManagement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    float horizontal;
    private Rigidbody2D rBody;
   
   private void Awake()
   {
    rBody = GetComponent<Rigidbody2D>();
  
   }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        rBody.linearVelocity = new Vector2(horizontal * speed, rBody.linearVelocity.y);

        if (horizontal < 0){ transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);}
        else if (horizontal > 0) { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);}

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rBody.linearVelocity.y) < 0.001f)
        {
            rBody.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    //private 
}
