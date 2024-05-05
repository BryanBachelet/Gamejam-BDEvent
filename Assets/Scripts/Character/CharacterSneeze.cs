using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSneeze : MonoBehaviour
{
    [Header("Sneeze Parameters")]
    public float power = 10;
    public float decceleration = 10;
    public Vector3 currentForce;

    [Header("Sneeze Power")]
    public float maxPowerSneeze = 10;
    public float minPowerSneeze = 10;
    public float timeToReloadSneezeBar =1 ;
    public float multiplicatorInputPressForTime;

    private float m_currentSneezePowerTimer = 0.0f;
    public float m_currentSneezePower = 0.0f;
    

    [Header("Sneeze Event")]
    public float sneezeTimer;
    [SerializeField] private float m_maxInclusive = 7.0f;
    [SerializeField] private float m_minInclusive = 1.0f;
    public GameObject m_vfxSneezeLoading;


    private float m_sneezeCounter;
    public UI_CharacterArrow m_characterArrow;
    public Rigidbody rigidbodyChara;

    public Vector3 currentDirection;
    private Vector3 inputDirection;
    private CharacterGeneral m_characterGeneral;
    private MeshRenderer m_meshRenderer;
    private Material m_characterMaterial;

    public Image sneezeImage;
    public bool IsSneezeInputPress;
    [Header("Info Sneeze")]
    [SerializeField] private bool m_isAllowRandomSneeze  = true;

    public void Start()
    {
        InitComponents();

        sneezeTimer = Random.Range(m_minInclusive, m_maxInclusive);
        m_characterMaterial = m_meshRenderer.material;
        m_characterArrow.gameObject.SetActive(false);
    }


    public void InitComponents()
    {
        m_characterGeneral = GetComponent<CharacterGeneral>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        rigidbodyChara = GetComponent<Rigidbody>();
        m_characterArrow = GetComponentInChildren<UI_CharacterArrow>();

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
            currentDirection = currentDirection.normalized;
            float sign = Mathf.Sign(currentDirection.x);

            currentDirection.x = sign* Mathf.Clamp(Mathf.Abs(currentDirection.x),  .10f,  1.0f);
            
            currentDirection.y = Mathf.Clamp(currentDirection.y, -1, -0.15f);
            inputDirection = -currentDirection;
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
        currentForce = currentDirection.normalized * m_currentSneezePower;
        m_currentSneezePower = 0.0f;
        m_currentSneezePowerTimer = 0.0f;
        rigidbodyChara.AddForce(currentForce, ForceMode.Impulse);
    }

    public void CallSneeze(Vector3 direction)
    {
        if (!m_characterGeneral.IsOnGround()) return;
        currentForce = direction.normalized * m_currentSneezePower;
        rigidbodyChara.AddForce(currentForce, ForceMode.Impulse);
    }

    public void Update()    
    {
        if (!m_characterGeneral.IsOnGround() && rigidbodyChara.velocity.y < 0)
            rigidbodyChara.velocity += Vector3.down * Time.deltaTime * decceleration;
            
        RandomSneeze();
        SneezeUIFeedback();
        ReloadSneeze();

        sneezeImage.material.SetFloat("_Fill" , m_currentSneezePowerTimer / timeToReloadSneezeBar);
    }

    public void SneezeUIFeedback()
    {
        if (m_characterGeneral.IsOnGround() && IsSneezeInputPress)
        {
            m_characterArrow.gameObject.SetActive(true);
            if (m_characterArrow) m_characterArrow.SetRotate(inputDirection, transform.position);
        }
        else
        {
            m_characterArrow.gameObject.SetActive(false);
        }
    }

    public void ReloadSneeze()
    {
        if (m_currentSneezePowerTimer > timeToReloadSneezeBar)
        {
            currentDirection = new Vector3(Random.Range(-0.25f, .25f), Random.Range(-1.0f, 0.0f));
            CallSneeze(-currentDirection);
            m_currentSneezePower = 0.0f;
            m_currentSneezePowerTimer = 0.0f;
        }
        else
        {

            if (IsSneezeInputPress && m_characterGeneral.IsOnGround()) m_currentSneezePowerTimer += Time.deltaTime * multiplicatorInputPressForTime;
            else m_currentSneezePowerTimer += Time.deltaTime;
            m_currentSneezePower = Mathf.Lerp(minPowerSneeze,maxPowerSneeze, m_currentSneezePowerTimer / timeToReloadSneezeBar);
        }

    }

    public void RandomSneeze()
    {
        if (!m_isAllowRandomSneeze || !m_characterGeneral.IsOnGround())
        {
            m_vfxSneezeLoading.SetActive(false);
            return;
        }

        if (m_sneezeCounter > sneezeTimer)
        {
            currentDirection = new Vector3(Random.Range(-0.45f, .45f), Random.Range(-1.0f, 0.0f));
            CallSneeze(-currentDirection);
            m_sneezeCounter = 0.0f;
            sneezeTimer = Random.Range(m_minInclusive, m_maxInclusive);
            m_currentSneezePower = 0.0f;
            m_currentSneezePowerTimer = 0.0f;
            m_vfxSneezeLoading.SetActive(false);
        }
        else
        {
            if(m_sneezeCounter>sneezeTimer-2)
            {

                if(!m_vfxSneezeLoading.activeSelf)
                {
                    m_vfxSneezeLoading.SetActive(true);
                    m_vfxSneezeLoading.GetComponent<ParticleSystem>().Play();
                }
            
            }

           m_sneezeCounter += Time.deltaTime;
        }
    }


}
