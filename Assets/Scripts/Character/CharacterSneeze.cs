using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSneeze : MonoBehaviour
{
    [Header("Sneeze Parameters")]
    public float power = 10;
    public float decceleration = 10;
    public Vector3 currentForce;
    [Header("Sneeze Event")]
    public float sneezeTimer;
    [SerializeField] private float m_maxInclusive = 7.0f;
    [SerializeField] private float m_minInclusive = 1.0f;




    private float m_sneezeCounter;
    public UI_CharacterArrow m_characterArrow;
    public Rigidbody rigidbodyChara;

    private Vector3 currentDirection;
    private Vector3 inputDirection;
    private CharacterGeneral m_characterGeneral;
    private MeshRenderer m_meshRenderer;
    private Material m_characterMaterial;

    public bool IsSneezeInputPress;
    [Header("Info Sneeze")]
    [SerializeField] private bool m_isAllowRandomSneeze  = true;

    public void Start()
    {
        m_characterGeneral = GetComponent<CharacterGeneral>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_characterMaterial = m_meshRenderer.material;
        sneezeTimer = Random.Range(m_minInclusive, m_maxInclusive);
        rigidbodyChara = GetComponent<Rigidbody>();
        m_characterArrow = GetComponentInChildren<UI_CharacterArrow>();
        m_characterArrow.gameObject.SetActive(false);
    }

    #region  Input Functions

    public void InputSneezeButton(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            IsSneezeInputPress = true;
           
        }
        if(ctx.canceled)
        {
            if(IsSneezeInputPress)
            {
                m_characterArrow.gameObject.SetActive(false);
                CallSneeze();
            }
            IsSneezeInputPress = false;
        }

    }


    public void AxisSneezeDirection(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            currentDirection = ctx.ReadValue<Vector2>();
            currentDirection.y = Mathf.Clamp(currentDirection.y, -1, 0);
            inputDirection = currentDirection;
            currentDirection = -currentDirection;
        }

     
    }

    // Cheat Inputs 

    public void InputStopSneeze(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            m_isAllowRandomSneeze = !m_isAllowRandomSneeze;
        }
    }

    #endregion

    public void CallSneeze()
    {
        if (!m_characterGeneral.IsOnGround()) return;
        currentForce = currentDirection.normalized * power;
        rigidbodyChara.AddForce(currentForce, ForceMode.Impulse);
    }

    public void CallSneeze(Vector3 direction)
    {
        if (!m_characterGeneral.IsOnGround()) return;
        currentForce = direction.normalized * power;
        rigidbodyChara.AddForce(currentForce, ForceMode.Impulse);
    }

    public void Update()    
    {
        if (!m_characterGeneral.IsOnGround() && rigidbodyChara.velocity.y < 0)
            rigidbodyChara.velocity += Vector3.down * Time.deltaTime * decceleration;
            
        RandomSneeze();


        if (m_characterGeneral.IsOnGround() && IsSneezeInputPress)
        {
            m_characterArrow.gameObject.SetActive(true);
          if(m_characterArrow)  m_characterArrow.SetRotate(inputDirection, transform.position);
        }
        else
        {
            m_characterArrow.gameObject.SetActive(false);
        }
      

    }


    public void RandomSneeze()
    {
        if (!m_isAllowRandomSneeze) return;
        if (m_sneezeCounter > sneezeTimer)
        {
            currentDirection = Vector3.down;
            CallSneeze(-currentDirection);
            m_sneezeCounter = 0.0f;
            sneezeTimer = Random.Range(m_minInclusive, m_maxInclusive);
            m_characterMaterial.color = Color.white;
        }
        else
        {
            m_characterMaterial.color = Color.Lerp(Color.white, Color.red, m_sneezeCounter / sneezeTimer);
           m_sneezeCounter += Time.deltaTime;
        }
    }


}
