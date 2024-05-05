using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    [Header("Character Parameters")]
    public Transform target;

    public Vector2 offsetCameraInMovement;
    public float mindistanceToTarget;
    public float maxDistanceTarget =40;
    public float triggerDistanceBox = 3;
    public float maxShakeDuration = 1;

    [Range(0, 1)] public float ratioSpeedDezoom;
    public float airDezoomBonusDistance = 10;
    [Range(0, 1)] public float ratioSpeedZoom;
    public float airZoomBonusDistance = 10;


    [SerializeField] float cameraMovementTime = 1;
    [SerializeField] private float timer;

    private CharacterMouvement m_characterMouvement;
    private CharacterSneeze m_characterSneeze;
    private CharacterGeneral m_characterGeneral;
    private Render.Camera.CameraShake m_cameraShake;
    private Rigidbody m_rigidbody;

    [Header("Infos Camera")]
    [SerializeField] private bool m_activeDebug;

    public void Start()
    {
        m_characterMouvement = target.GetComponent<CharacterMouvement>();
        m_characterSneeze = target.GetComponent<CharacterSneeze>();
        m_characterGeneral = target.GetComponent<CharacterGeneral>();
        m_characterSneeze.m_sneezeEvent += ShakeCam;
        m_rigidbody = GetComponent<Rigidbody>();
        m_cameraShake = GetComponent<Render.Camera.CameraShake>();
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    public Vector2 ComputeOffset(float ratioOffset)
    {
        float sign = 0;

        Vector2 offsetValue = offsetCameraInMovement;
        if (m_characterGeneral.IsOnGround() || !m_characterSneeze.beforSneeze)
        {

            sign = m_characterMouvement.GetMovementSign();
            Vector3 dir = new Vector3(sign, 0, 0);
            offsetValue = dir * offsetCameraInMovement.x * sign;
        }
        else
        {
            sign = Mathf.Sign(m_characterSneeze.rigidbodyChara.velocity.x);
            offsetValue = m_characterSneeze.rigidbodyChara.velocity.normalized;
            offsetValue *= offsetCameraInMovement.x;
        }

        return offsetValue * ratioOffset;
    }

    public float ComputeRatioOffset(bool up)
    {

        if (!up) timer -= Time.fixedDeltaTime;
        else timer += Time.fixedDeltaTime;

        timer = Mathf.Clamp(timer, 0, cameraMovementTime);

        return timer / cameraMovementTime;
    }

    public void ShakeCam(float ratio)
    {

        m_cameraShake.LaunchShake(maxShakeDuration * ratio);
    }

    public void UpdateCameraMouvement(Vector2 offsetValue)
    {
        Vector3 position = new Vector3();

        Vector2 positionTarget = target.position;
        positionTarget += offsetValue;
        Vector2 positionCamera = transform.position;


        Vector2 pos2D = Vector2.Lerp(positionCamera, positionTarget, Time.fixedDeltaTime * 2);

        float ratio = (m_characterSneeze.m_currentSneezePower - m_characterSneeze.minPowerSneeze) / (m_characterSneeze.maxPowerSneeze - m_characterSneeze.minPowerSneeze);
        float depthDistance = -Mathf.Lerp(mindistanceToTarget, maxDistanceTarget,ratio );
        position = pos2D;
        float finalDepth = m_characterGeneral.IsOnGround() ? Mathf.Lerp(transform.position.z, depthDistance, 0.1f) :  Mathf.Lerp(transform.position.z, -maxDistanceTarget-airDezoomBonusDistance, ratioSpeedDezoom);
       
        if(m_characterSneeze.beforSneeze) finalDepth = Mathf.Lerp(transform.position.z, -mindistanceToTarget + airZoomBonusDistance, m_characterSneeze.countdownBeforeSneeze/m_characterSneeze.timeBeforSneeze);
        position.z = finalDepth;

        SetCameraPosition(position + m_cameraShake.GetEffectPos());
        SetCameraRotation();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float ratioOffset = 0;
        Vector2 offsetValue = Vector2.zero;

        if (!IsOutsideCenter()) ratioOffset = ComputeRatioOffset(false);
        else ratioOffset = ComputeRatioOffset(true);

        offsetValue = ComputeOffset(ratioOffset);
        UpdateCameraMouvement(offsetValue);
    }


    public bool IsOutsideCenter()
    {
        Vector2 positionTarget = target.position;
        Vector2 positionCamera = transform.position;

        float resultDistance = Vector2.Distance(positionCamera, positionTarget);
        return resultDistance > (triggerDistanceBox);
    }

    public void SetCameraPosition(Vector3 position)
    {
        m_rigidbody.MovePosition(position);
    }

    public void SetCameraRotation()
    {
        Quaternion rot =  Quaternion.Euler(m_cameraShake.GetEffectRot());
        m_rigidbody.MoveRotation(rot);
    }

    public void OnDrawGizmosSelected()
    {
        if (!m_activeDebug) return;

        Gizmos.color = Color.green;

        Vector2 positionTarget = target.position;
        Vector2 positionCamera = transform.position;
        Gizmos.DrawLine(positionTarget, positionCamera);

        Vector3 finalPos = positionCamera;

        Gizmos.DrawWireCube(finalPos, new Vector3(triggerDistanceBox * 2, triggerDistanceBox * 2, 1));
    }
}
