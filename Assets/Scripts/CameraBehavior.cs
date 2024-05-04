using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
   
    [Header("Character Parameters")]
    public Transform target;

    public Vector2 offsetCameraInMovement;
    public Vector2 baseOffset;
    public float distanceToTarget;
    public float triggerDistanceBox = 3;
    public float lerpSpeedCameraMovement = 3;
    [SerializeField] private float minLerpValueCameraOnMove = 0.1f;

    private float cameraMovementTime = 1;
    [SerializeField] private float timer;

    private CharacterMouvement m_characterMouvement;
    private CharacterSneeze m_characterSneeze;
    private CharacterGeneral m_characterGeneral;

    [Header("Infos Camera")]
    [SerializeField] private bool m_activeDebug;

    public void Start()
    {
        m_characterMouvement = target.GetComponent<CharacterMouvement>();
        m_characterSneeze = target.GetComponent<CharacterSneeze>();
        m_characterGeneral = target.GetComponent<CharacterGeneral>();
    }

    // Update is called once per frame
    void Update()
    {
            Vector3 position = new Vector3();
        float sign = 0;

        Vector2 offsetValue = offsetCameraInMovement;
        if (m_characterGeneral.IsOnGround())
        {

            sign = m_characterMouvement.GetMovementSign();
            offsetValue *= sign;
        }   
        else
        {
            sign = Mathf.Sign(m_characterSneeze.rigidbodyChara.velocity.x);
            offsetValue *= m_characterSneeze.rigidbodyChara.velocity.normalized;
        }

            Vector2 positionTarget = target.position;
            positionTarget +=  offsetValue + baseOffset;
            Vector2 positionCamera = transform.position;

            if (sign != 0)
            {
                if (timer / cameraMovementTime < minLerpValueCameraOnMove)
                {
                    timer += Time.deltaTime;
                    timer = Mathf.Clamp(timer, 0, minLerpValueCameraOnMove);
                }

            }
            else
            {
                if (Vector2.Distance(positionCamera, positionTarget) > .5f)
                {
                    if (timer < cameraMovementTime)
                    {
                        timer += Time.deltaTime;
                    }
                }
                else
                {
                    timer = 0;
                }

            }




            Vector2 pos2D = Vector2.Lerp(positionCamera, positionTarget, timer / cameraMovementTime);

            position = pos2D;
            position.z = -distanceToTarget;
            SetCameraPosition(position);

        }

    public bool IsOutsideCenter()
    {
        Vector2 positionTarget = target.position ;
        Vector2 positionCamera = transform.position;

        float resultDistance = Vector2.Distance(positionCamera, positionTarget);
        Debug.Log("Result distance " + resultDistance.ToString());
        return resultDistance > (triggerDistanceBox/2);
    }

    public void SetCameraPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void OnDrawGizmosSelected()
    {
        if (!m_activeDebug) return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position + Vector3.forward*distanceToTarget, new Vector3(triggerDistanceBox, triggerDistanceBox, 1));
    }
}
