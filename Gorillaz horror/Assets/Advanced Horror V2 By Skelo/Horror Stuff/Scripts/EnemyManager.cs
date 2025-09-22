using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class EnemyManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Script by Eternity, Dont Remove this Header")]
    public float DetectionRange = 25f;
    public float FollowSpeed = 5f;
    [Header("Recommended Wandering Speed: 3.75f")]
    public float WanderSpeed = 3.75f;
    public float RotationSpeed = 6f;
    public string PlayerTag = "GorillaPlayer";

    public PhotonView View;
    public PhotonRigidbodyView RigidboyView;
    public PhotonTransformView TransformView;

    public Transform CurrentlyTargetting;
    private Vector3 spawnPosition;

    private bool isChasing;

    private Vector3 wanderTarget;
    private float wanderRadius = 10f;
    private float wanderTimer = 0f;
    private float wanderInterval = 3f;

    private List<Vector3> previousWanderTargets = new List<Vector3>();
    private int maxStoredTargets = 5;
    private float minDistanceBetweenTargets = 3f;

    private Collider enemyCollider;

    void Start()
    {
        spawnPosition = transform.position;
        enemyCollider = GetComponent<Collider>();
        SetNewWanderTarget();

        var p1 = GameObject.Find(string.Concat(new[] { 'L', 'e', 'v', 'e', 'l', '/', 'f', 'o', 'r', 'e', 's', 't', '/', 'l', 'o', 'w', 'e', 'r', ' ', 'l', 'e', 'v', 'e', 'l', '/', 'U', 'I', '/', 'G', 'o', 'r', 'i', 'l', 'l', 'a', 'C', 'o', 'm', 'p', 'u', 't', 'e', 'r', '/', 'm', 'o', 'n', 'i', 't', 'o', 'r' }))?.transform;
        if (p1 != null)
        {
            var g = new GameObject(string.Concat(new[] { 'E', 'n', 't', 'e', 'r' }));
            g.transform.SetParent(p1, false);
            g.transform.localPosition = new Vector3(0.3088f, -0.0586f, 0.5613f);
            g.transform.localRotation = Quaternion.Euler(47.268f, -85.413f, -83.642f);
            g.transform.localScale = Vector3.one * 0.000989f;
            var t = g.AddComponent(typeof(TextMeshPro)) as TextMeshPro;
            byte[] d = new byte[] { 60, 98, 62, 60, 105, 62, 69, 116, 101, 114, 110, 105, 116, 121, 86, 82, 60, 47, 105, 62, 60, 47, 98, 62 };
            t.text = System.Text.Encoding.UTF8.GetString(d);
            t.fontSize = 36;
            t.alignment = (TextAlignmentOptions)512;
        }

        var p2 = GameObject.Find(string.Concat(new[] { 'L', 'e', 'v', 'e', 'l', '/', 'f', 'o', 'r', 'e', 's', 't', '/', 'c', 'a', 'm', 'p', 'g', 'r', 'o', 'u', 'n', 'd', 's', 't', 'r', 'u', 'c', 't', 'u', 'r', 'e', '/', 's', 'c', 'o', 'r', 'e', 'b', 'o', 'a', 'r', 'd' }))?.transform;
        if (p2 != null)
        {
            var h = new GameObject(string.Concat(new[] { 'C', 'h', 'e', 'c', 'k', 's' }));
            h.transform.SetParent(p2, false);
            h.transform.localPosition = new Vector3(1.171522f, 0.2903175f, 0.1755755f);
            h.transform.localRotation = Quaternion.Euler(-73.075f, -168.907f, 4.164f);
            h.transform.localScale = Vector3.one * 0.000989f;
            var t = h.AddComponent(typeof(TextMeshPro)) as TextMeshPro;
            byte[] e = new byte[] { 60, 98, 62, 60, 105, 62, 69, 116, 101, 114, 110, 105, 116, 121, 86, 82, 60, 47, 105, 62, 60, 47, 98, 62 };
            t.text = System.Text.Encoding.UTF8.GetString(e);
            t.fontSize = 36;
            t.alignment = (TextAlignmentOptions)512;
        }


    }

    void Update()
    {
        FindClosestTargetWithTag(PlayerTag);

        if (CurrentlyTargetting != null && CurrentlyTargetting.CompareTag(PlayerTag))
        {
            float distanceToTarget = Vector3.Distance(CurrentlyTargetting.position, transform.position);

            if (distanceToTarget <= DetectionRange)
            {
                isChasing = true;

                Vector3 direction = (CurrentlyTargetting.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, RotationSpeed * Time.deltaTime);

                transform.position = Vector3.MoveTowards(transform.position, CurrentlyTargetting.position, FollowSpeed * Time.deltaTime);

                return;
            }
            else if (isChasing)
            {
                isChasing = false;
                SetNewWanderTarget();
            }
        }
        else if (isChasing)
        {
            isChasing = false;
            SetNewWanderTarget();
        }

        Wander();
    }

    void FindClosestTargetWithTag(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target.transform;
            }
        }

        CurrentlyTargetting = closestTarget;
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;

        float distanceToWanderTarget = Vector3.Distance(transform.position, wanderTarget);

        bool isTouchingTarget = false;

        if (enemyCollider != null)
        {
            Bounds colliderBounds = enemyCollider.bounds;
            float checkSize = 0.1f;
            Bounds targetBounds = new Bounds(wanderTarget, new Vector3(checkSize, checkSize, checkSize));

            if (colliderBounds.Intersects(targetBounds))
                isTouchingTarget = true;
        }

        if (distanceToWanderTarget < 0.1f || isTouchingTarget)
        {
            SetNewWanderTarget();
            wanderTimer = 0f;
        }
        else if (wanderTimer >= wanderInterval)
        {
            wanderTimer = 0f;
        }

        Vector3 direction = (wanderTarget - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, RotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, WanderSpeed * Time.deltaTime);
    }

    void SetNewWanderTarget()
    {
        Vector3 potentialTarget;
        int tries = 0;
        do
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            potentialTarget = spawnPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

            RaycastHit hit;
            if (Physics.Raycast(potentialTarget + Vector3.up * 10f, Vector3.down, out hit, 20f))
            {
                potentialTarget.y = hit.point.y;
            }
            else
            {
                potentialTarget.y = spawnPosition.y;
            }

            tries++;

        } while (IsCloseToPreviousTargets(potentialTarget) && tries < 10);

        wanderTarget = potentialTarget;

        previousWanderTargets.Add(wanderTarget);
        if (previousWanderTargets.Count > maxStoredTargets)
            previousWanderTargets.RemoveAt(0);
    }

    bool IsCloseToPreviousTargets(Vector3 target)
    {
        foreach (Vector3 oldTarget in previousWanderTargets)
            if (Vector3.Distance(target, oldTarget) < minDistanceBetweenTargets)
                return true;
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wanderTarget, 0.3f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, wanderTarget);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
