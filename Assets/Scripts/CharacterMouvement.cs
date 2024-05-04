using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMouvement : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float accelerationRun = 2;
    public float deccelerationRun = 2;
    public float maxRunSpeed = 10;
    public float stopDistance = .5f;
    public LayerMask obstacleLayerMask;

    [Header("Movement Infos")]
    [SerializeField] private Vector3 m_currentSpeed;

    private Vector2 m_movementInputValue;
    private Rigidbody m_rigidbody;
    private float m_previousSign;
    private float m_movementSign;
    private CharacterGeneral m_characterGeneral;

    public void Start()
    {
        m_characterGeneral = GetComponent<CharacterGeneral>();
    }

    public void AxisMovemetInnput(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            m_movementInputValue = Vector2.zero;

        }
        if (ctx.performed)
        {

            m_movementInputValue = ctx.ReadValue<Vector2>();
            m_movementSign = m_movementInputValue.x;
        }
    }

    public void UpdateMouvement()
    {
        if (m_movementInputValue.x == 0)
        {
            m_currentSpeed.x -= deccelerationRun * Time.deltaTime;

        }
        else
        {
            if (IsObstacleOnWay())
            {
                m_currentSpeed.x -= deccelerationRun * Time.deltaTime;
                m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0, maxRunSpeed);
                return;
            }
               
            m_currentSpeed.x += accelerationRun * Time.deltaTime;
        }


        m_currentSpeed.y = 0;
        m_currentSpeed.z = 0;

        m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0, maxRunSpeed);

        transform.position += m_movementSign * m_currentSpeed * Time.deltaTime;

    }

    public bool IsObstacleOnWay()
    {

        Ray rayObstacle = new Ray(transform.position, m_movementSign* m_currentSpeed.normalized);
        return Physics.Raycast(rayObstacle, stopDistance, obstacleLayerMask);
    }

    public void Update()
    {
      if(m_characterGeneral.IsOnGround())  UpdateMouvement();
    }

    public Vector3 GetMouvementDirection() { return m_currentSpeed; }



    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, m_movementSign * m_currentSpeed.normalized * stopDistance);
    }
}
