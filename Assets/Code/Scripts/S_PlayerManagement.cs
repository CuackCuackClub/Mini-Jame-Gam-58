using UnityEngine;
using System.Collections;
using UnnityEngine.InputSystem;
public class S_PlayerManagement : MonoBehaviour
{
   private Rigidbody2D rBody;
   
   private void Awake()
   {
    rBody = GetComponent<Rigidbody2D>();
   }

    private void Start()
    {
        
    }

    private void Update()
    {
        rBody.linearVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
