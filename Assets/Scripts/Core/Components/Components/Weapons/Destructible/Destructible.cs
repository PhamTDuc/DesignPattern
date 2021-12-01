using System;
using UnityEngine;

public class Destructible : MonoBehaviour, IDestructible
{
    [SerializeField] int m_maxHealth;
    [SerializeField] int m_currentHealth;
    [SerializeField] GameObject m_impact;
    [SerializeField] GameObject m_destroyedVersion;
    [SerializeField] bool m_destroyParent;
    [SerializeField] bool m_canDestroy;

    IDestructible m_rootParent;

    public int MaxHealth => m_maxHealth;
    public int CurrentHealth => m_currentHealth;

    void Awake()
    {
        IDestructible rootParent = transform.root.GetComponent<IDestructible>();
        m_rootParent = (rootParent != (IDestructible)this) ? rootParent : null;
        Heal(m_maxHealth);
        m_rootParent?.Heal(m_maxHealth);
    }
    public void Heal(int value)
    {
        m_currentHealth += value;
    }
    public void GetDamage(int damage, RaycastHit hit)
    {
        m_currentHealth -= damage;
        if (m_impact != null)
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Instantiate(m_impact, hit.point, rot);
        }

        m_rootParent?.GetDamage(damage, hit);

        if (m_currentHealth <= 0 && m_canDestroy) // * OnDied()
        {
            OnDied();
        }
    }

    protected virtual void OnDied()
    {
        if (m_destroyedVersion != null)
        {
            Instantiate(m_destroyedVersion, transform.position, transform.rotation);
        }

        if (m_destroyParent)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
