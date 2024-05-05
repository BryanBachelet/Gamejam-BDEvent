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

    [Header("Air Control")]
    public float accelerationAirControl = 2;
    public float maxSpeedAirControl = 2;
    

    [Header("Movement Infos")]
    [SerializeField] private Vector3 m_currentSpeed;

    private Vector2 m_movementInputValue;
    private Rigidbody m_rigidbody;
    private float m_previousSign;
    private float m_movementSign;
    private CharacterGeneral m_characterGeneral;
    private CharacterSneeze m_characterSneeze;


    private Vector3 prevPos;
    public GameObject meshChara;
    public Animator m_meshAnimator;

    public void Start()
    {
        m_characterGeneral = GetComponent<CharacterGeneral>();
        m_characterSneeze = GetComponent<CharacterSneeze>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    #region Input Functons
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
    
    #endregion



    public void UpdateAirMovement(float acceleration, float maxSpeed,float ratio)
    {
        if (m_movementInputValue.x == 0  || m_characterSneeze.IsSneezeInputPress)
        {
            m_currentSpeed.x -= deccelerationRun * Time.deltaTime;

        }
        else
        {
            if (IsObstacleOnWay())
            {
                m_currentSpeed.x -= deccelerationRun * Time.deltaTime;
                m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0, maxSpeed);
                return;
            }
               
            m_currentSpeed.x += acceleration * Time.deltaTime;
        }


        m_currentSpeed.y = 0;
        m_currentSpeed.z = 0;

        m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0, maxSpeed);

        m_rigidbody.AddForce(m_movementSign * m_currentSpeed * ratio, ForceMode.Force);
     //    transform.position += m_movementSign * m_currentSpeed *ratio * Time.deltaTime;
        //Vector3 speed = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
        //speed = Vector3.ClampMagnitude(speed, maxSpeed);
        //m_rigidbody.velocity = new Vector3(speed.x, m_rigidbody.velocity.y, speed.z);

    }

    public void UpdateMouvement(float acceleration, float maxSpeed)
    {
        if (m_movementInputValue.x == 0 || m_characterSneeze.IsSneezeInputPress)
        {
            m_currentSpeed.x -= deccelerationRun * Time.deltaTime;
            m_meshAnimator.SetTrigger("Idle");
            m_meshAnimator.ResetTrigger("Running");
            m_meshAnimator.ResetTrigger("OnAir");
        }
        else
        {
            if (IsObstacleOnWay())
            {
                m_currentSpeed.x -= deccelerationRun * Time.deltaTime;
                m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0, maxSpeed);
                return;
            }

            m_currentSpeed.x += acceleration * Time.deltaTime;
            m_meshAnimator.SetTrigger("Running");
            m_meshAnimator.ResetTrigger("Idle");
            m_meshAnimator.ResetTrigger("OnAir");
        }


        m_currentSpeed.y = 0;
        m_currentSpeed.z = 0;

        m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0, maxSpeed);


        transform.position += m_movementSign * m_currentSpeed * Time.deltaTime;

        
        

    }

    public bool IsObstacleOnWay()
    {
        Ray rayObstacle = new Ray(transform.position, m_movementSign* m_currentSpeed.normalized);
        return Physics.Raycast(rayObstacle, stopDistance, obstacleLayerMask);
    }

    public void Update()
    {
      if(m_characterGeneral.IsOnGround()) 
            UpdateMouvement(accelerationRun,maxRunSpeed);

        if (m_rigidbody.velocity.y < m_characterSneeze.maxPowerSneeze && !m_characterGeneral.IsOnGround())
        {
            float ratio = 1 - (m_rigidbody.velocity.magnitude / m_characterSneeze.maxPowerSneeze);
            if (m_rigidbody.velocity.y < 0) 
                ratio = 1;
            UpdateAirMovement(accelerationAirControl, maxSpeedAirControl, ratio);
        }

      
    }


    public void FixedUpdate()
    {
        if (m_characterGeneral.IsOnGround()  && !m_characterSneeze.beforSneeze|| m_rigidbody.velocity.y < 2 && !m_characterSneeze.beforSneeze )
        {
            float signFace = Mathf.Sign(transform.position.x - prevPos.x);
            float angle = signFace == -1 ? -90 : 110;
            meshChara.transform.rotation = Quaternion.Euler(0, angle, 0);
            prevPos = transform.position;
        }
        else if(m_characterSneeze.beforSneeze)
        {
            float signFace = Mathf.Sign(m_characterSneeze.currentDirection.x);
            float angle = signFace == -1 ? -90 : 110;
            meshChara.transform.rotation = Quaternion.Euler(0, angle, 0);

        }



    }



    public Vector3 GetMouvementSpeed() { return m_currentSpeed; }
    public float GetMovementSign() { return m_currentSpeed.x == 0 ? 0:m_movementSign; }
   
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, m_movementSign * m_currentSpeed.normalized * stopDistance);
    }
}
