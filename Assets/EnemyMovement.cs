using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Player;
    public LayerMask HidableLayers;
    public EnemyLineOfSightChecker LineOfSightChecker;
    public NavMeshAgent Agent;
    [Range(-1f, 1f)]
    [Tooltip("Lower is a better hiding spot")]
    public float HideSensitivity = 0;

    public float maxDistance = 2f;
    [Range(1f, 10f)]
    public float MinDistanceBtwThePlayer = 5f;
    [Range(0, 5f)]
    public float MinObstacleHeight = 1.25f;
    [Range(0.01f, 1f)]
    public float UpdateFrequency = 0.25f;
    public float MultiplyNormalizedSampleDistance = 2f;

    private Coroutine MovementCoroutine;
    private Collider[] Colliders = new Collider[10]; // more is less performant, but more options;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        LineOfSightChecker.onGainSight += HandleGainSight;
        LineOfSightChecker.onLoseSight += HandleLoseSight;
    }

    private void HandleGainSight(Transform target)
    {
        if(MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);

        }

        Player = target;
        MovementCoroutine = StartCoroutine(Hide(target));
    }

    private void HandleLoseSight(Transform target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);

        }
        Player = null;
    }

    private IEnumerator Hide(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateFrequency);
        while (true)
        {
            for(int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }

            //check/store colliders (obstacles) touching or inside the sphere + inside HidableLayers
            int hits = Physics.OverlapSphereNonAlloc(Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

            int hitReduction = 0;
            for(int i = 0; i < hits; i++)
            {
                if(Vector3.Distance(Colliders[i].transform.position,target.position) < MinDistanceBtwThePlayer || Colliders[i].bounds.size.y < MinObstacleHeight)
                {
                    Colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            System.Array.Sort(Colliders, ColliderArraySortComparer);
            for (int i = 0; i < hits; i++)
            {
                //Finds the nearest point based on the NavMesh within a specified range.
                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, maxDistance, Agent.areaMask))
                {
                    //bool True if the nearest edge is found.
                    //Locate the closest NavMesh edge from a point on the NavMesh.
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, Agent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    // dot product = produit scalaire
                    // enter if the dot product is under 0 -> if the hit point do NOT face the player
                    if(Vector3.Dot(hit.normal,(target.position - hit.position).normalized) < HideSensitivity)
                    {
                        Agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // since previous spot wasn't facing "away" from the player, we'll try the other side of the object
                        if(NavMesh.SamplePosition(Colliders[i].transform.position - (target.position - hit.position).normalized * MultiplyNormalizedSampleDistance, out NavMeshHit hit2, maxDistance, Agent.areaMask))
                        {
                            if(!NavMesh.FindClosestEdge(hit2.position, out hit2, Agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if(Vector3.Dot(hit2.normal, (target.position - hit2.position).normalized) < HideSensitivity)
                            {
                                Agent.SetDestination(hit2.position);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
                }
            }
            yield return wait;
        }
    }

    public int ColliderArraySortComparer(Collider A, Collider B)
    {
        if(A == null && B != null)
        {
            return 1;
        }
        else if(A != null && B == null)
        {
            return -1;
        }
        else if(A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(Agent.transform.position,A.transform.position).CompareTo(Vector3.Distance(Agent.transform.position, B.transform.position));
        }
    }

}
