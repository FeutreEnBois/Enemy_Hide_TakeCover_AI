using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider Collider;
    public float FieldOfView = 90f;
    public LayerMask LineOfSightLayer;

    public delegate void GainSightEvent(Transform target);
    public GainSightEvent onGainSight;
    public delegate void LoseSightEvent(Transform target);
    public LoseSightEvent onLoseSight;

    private Coroutine CheckForLineOfSightCoroutine;

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckLineOfSight(other.transform))
        {
            CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onLoseSight?.Invoke(other.transform);
        if (CheckForLineOfSightCoroutine != null)
        {
            StopCoroutine(CheckForLineOfSightCoroutine);
        }
    }

    private bool CheckLineOfSight(Transform target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, direction);
        if(dotProduct >= Mathf.Cos(FieldOfView))
        {
            if(Physics.Raycast(transform.position, direction, out RaycastHit hit, Collider.radius, LineOfSightLayer))
            {
                onGainSight?.Invoke(target);
                return true;
            }
        }
        return false;
    }

    private IEnumerator CheckForLineOfSight(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (!CheckLineOfSight(target))
        {
            yield return wait;
        }
    }
}
