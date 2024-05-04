using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterArrow : MonoBehaviour
{
    [SerializeField] private float spriteDistance =1;
    private RectTransform m_rectTransform;
    public void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }
    public void SetRotate(Vector2 direction ,Vector3 pos)   
    {
        if (direction == Vector2.zero) return;

        m_rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        float angle = Vector3.SignedAngle(Vector3.up, direction.normalized, Vector3.forward);
        Vector2 position = pos;
        float z = m_rectTransform.position.z;
        m_rectTransform.position = position + direction.normalized * spriteDistance;
        m_rectTransform.position += new Vector3(0, 0, z);

        m_rectTransform.Rotate(new Vector3(0,0,angle),Space.World);
    }
}
