using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiKnife : MonoBehaviour
{
    private Transform m_currentTarget;
    private WeaponIk m_weaponIk;

    private void Awake()
    {
        m_weaponIk = GetComponent<WeaponIk>();
    }

    public void DropKnife()
    {
        if (m_currentTarget)
        {
            m_currentTarget.transform.SetParent(null);
            m_currentTarget.gameObject.GetComponent<BoxCollider>().enabled = true;
            m_currentTarget.gameObject.AddComponent<Rigidbody>();
        }
    }

    public void ActivateKnife()
    {
        m_weaponIk.enabled = true;
    }

    public bool HasKnife()
    {
        return m_currentTarget != null;
    }

    public void SetTarget(Transform target)
    {
        m_weaponIk.SetTargetTransform(target);
        m_currentTarget = target;
    }
}
