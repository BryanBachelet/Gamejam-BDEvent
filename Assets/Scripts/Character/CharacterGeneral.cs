using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGeneral : MonoBehaviour
{

    [SerializeField] private float distanceGroundTest = 1.0f;
    private const float offsetGroundTest = 0.5f;
    public LayerMask groundLayer;
    private bool isGrounded = false;



    public bool IsOnGround()
    {
        return isGrounded;
    }
    internal bool _IsOnGround()
    {
        bool isGrounded = Physics.Raycast(transform.position + Vector3.right * offsetGroundTest, Vector3.down, distanceGroundTest, groundLayer);
        bool isGrounded2 = Physics.Raycast(transform.position + Vector3.right * -offsetGroundTest, Vector3.down, distanceGroundTest, groundLayer);
        return isGrounded || isGrounded2; 

    }

    public void Update()
    {
        isGrounded = _IsOnGround();
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + Vector3.right * offsetGroundTest, Vector3.down * distanceGroundTest);
        Gizmos.DrawRay(transform.position + Vector3.right * -offsetGroundTest, Vector3.down * distanceGroundTest);

    }
}
