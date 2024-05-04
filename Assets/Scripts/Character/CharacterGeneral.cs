using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGeneral : MonoBehaviour
{
    public LayerMask groundLayer;
    private bool isGrounded = false;

    public bool IsOnGround(float distance)
    {
        return Physics.Raycast(transform.position, Vector3.down, distance, groundLayer);

    }

    public bool IsOnGround()
    {
        return isGrounded;
    }
    private bool InternIsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

    }
    public bool IsOnGround(out RaycastHit hit )
    {
        return Physics.Raycast(transform.position, Vector3.down,out hit, 1.1f, groundLayer);

    }

    public void Update()
    {
        isGrounded = InternIsOnGround();
    }
}
