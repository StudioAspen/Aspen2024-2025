using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon: References")]
    [SerializeField, Self] private CapsuleCollider capsuleCollider;
    [SerializeField, Anywhere] private GameObject trailObject;
    private Animator animator;

    [Header("Weapon: Settings")]
    [SerializeField] private AnimatorOverrideController overrideAnimator;

    [Header("Weapon: Collisions")]
    [SerializeField] private LayerMask damageableCollidersLayerMask;
    [SerializeField] private Transform colliderStartTransform;
    [SerializeField] private Transform colliderEndTransform;
    private Ray currentFrameCollisionRay;
    private Ray previousFrameCollisionRay;
    private int currentHitFrame;

    [Header("Weapon: Combo")]
    public List<ComboDataSO> Combos;
    private Vector2Int damageRange;

    [Header("Weapon: Impact Frames")]
    [SerializeField] private float impactFramesDuration = 0.15f;
    private Coroutine impactFramesCoroutine;
    private List<Enemy> enemiesHitByCurrentAttack = new List<Enemy>();

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();

        AssignColliderStartEndPositions();

        animator.runtimeAnimatorController = overrideAnimator;
    }

    private void Update()
    {
        trailObject.SetActive(capsuleCollider.enabled);

        HandleHitDetectionBetweenFrames();
    }

    private void OnTriggerStay(Collider other)
    {
        HandleHitDetectionOnTrigger(other);
    }

    private void HandleHitDetectionOnTrigger(Collider other)
    {
        if (!capsuleCollider.enabled) return; // if weapon is not trying to hit, then dont bother to handle hit detection

        Enemy enemy = other.GetComponentInParent<Enemy>(); // body parts dont contain the Enemy script but the parent does

        if (enemy == null) return;

        if (enemiesHitByCurrentAttack.Contains(enemy)) return; // stops if we already hit the enemy once
        enemiesHitByCurrentAttack.Add(enemy);

        Vector3 hitPoint = other.ClosestPointOnBounds(colliderStartTransform.position);
        HitEnemy(enemy, hitPoint, true);
    }

    private void HandleHitDetectionBetweenFrames()
    {
        if (!capsuleCollider.enabled) // if weapon is not trying to hit, reset our current hit frame and dont bother to handle hit detection
        {
            currentHitFrame = 0;
            return;
        }

        previousFrameCollisionRay = currentFrameCollisionRay; // makes the previous ray always one frame behind the current by assigning this before the 2 lines below

        Vector3 hiltToTipVector = colliderEndTransform.position - colliderStartTransform.position;
        currentFrameCollisionRay = new Ray(colliderStartTransform.position, hiltToTipVector); // the ray starting from the hilt to the tip

        if(currentHitFrame > 0)
        {
            // divides our sword into multiple segments to cast rays between the prev and curr frame. essentially works like a blade trail but with rays.
            int segments = (int)Mathf.Ceil(hiltToTipVector.magnitude / capsuleCollider.radius);
            for(int i = 0; i <= segments; i++)
            {
                Vector3 currPoint = currentFrameCollisionRay.origin + i / (float)segments * currentFrameCollisionRay.direction;
                Vector3 prevPoint = previousFrameCollisionRay.origin + i / (float)segments * previousFrameCollisionRay.direction;

                CheckCollisionsWithRays(new Ray(prevPoint, currPoint-prevPoint), Vector3.Distance(currPoint, prevPoint));

                Debug.DrawLine(currPoint, prevPoint, Color.red, 2f); // debug draw the trail gizmos for 2 seconds
            }
        }

        currentHitFrame++;
    }

    private void CheckCollisionsWithRays(Ray ray, float distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, damageableCollidersLayerMask); // grab all body parts hit

        if (hits == null) return;
        if (hits.Length == 0) return;

        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>(); // body parts dont contain the Enemy script but the parent does

            if (enemy == null) continue;

            if (enemiesHitByCurrentAttack.Contains(enemy)) continue; // checks next hit if we already hit the enemy once
            enemiesHitByCurrentAttack.Add(enemy);

            Vector3 hitPoint = hit.collider.ClosestPointOnBounds(hit.point);
            if (hit.distance == 0) hitPoint = hit.collider.ClosestPointOnBounds((colliderStartTransform.position + colliderEndTransform.position) / 2);

            HitEnemy(enemy, hitPoint, false);
        }
    }

    private void HitEnemy(Enemy enemy, Vector3 hitPoint, bool fromTrigger)
    {
        StartImpactFrames(0.1f);
        CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

        CreateTempHitVisual(hitPoint, fromTrigger ? Color.green : Color.red, 1.5f);

        enemy.TakeDamage(GetRandomDamage(), hitPoint);
    }

    private void CreateTempHitVisual(Vector3 pos, Color color, float duration)
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "TempHitVisual";
        temp.GetComponent<Collider>().enabled = false;
        temp.transform.localScale = 0.1f * Vector3.one;
        temp.transform.position = pos;
        temp.GetComponent<Renderer>().material.color = color;
        Destroy(temp, duration);
    }

    private void StartImpactFrames(float timeScale)
    {
        if (impactFramesCoroutine != null) StopCoroutine(impactFramesCoroutine);
        StartCoroutine(ImpactFramesCoroutine(timeScale, impactFramesDuration));
    }

    private IEnumerator ImpactFramesCoroutine(float timeScale, float duration)
    {
        float speedUpTime = duration / 4;

        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration - speedUpTime);

        for (float t = 0; t < speedUpTime; t += Time.unscaledDeltaTime)
        {
            Time.timeScale = Mathf.Lerp(timeScale, 1f, t / speedUpTime);
            yield return null;
        }

        Time.timeScale = 1f;
    }

    private void AssignColliderStartEndPositions()
    {
        colliderStartTransform.localPosition = capsuleCollider.center - (0.5f * capsuleCollider.height - capsuleCollider.radius) * Vector3.up;
        colliderEndTransform.localPosition = capsuleCollider.center + (0.5f * capsuleCollider.height - capsuleCollider.radius) * Vector3.up;
    }

    public void ClearEnemiesHitList()
    {
        enemiesHitByCurrentAttack.Clear();
    }

    public void EnableTriggers()
    {
        capsuleCollider.enabled = true;
    }

    public void DisableTriggers()
    {
        capsuleCollider.enabled = false;
    }

    public void SetDamageRange(Vector2Int newRange)
    {
        damageRange = newRange;
    }

    private int GetRandomDamage()
    {
        return Random.Range(damageRange.x, damageRange.y);
    }
}
