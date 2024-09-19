using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private CapsuleCollider capsuleCollider;
    private Animator animator;

    [Header("Settings")]
    [SerializeField] private AnimatorOverrideController overrideAnimator;

    [Header("Collisions")]
    [SerializeField] private LayerMask damageableCollidersLayerMask;
    [SerializeField] private Transform colliderStartTransform;
    [SerializeField] private Transform colliderEndTransform;
    private Ray currentFrameCollisionRay;
    private Ray previousFrameCollisionRay;
    private int currentHitFrame;

    [Header("Combo")]
    public List<PlayerComboActionStateSO> Combos;
    private Vector2Int damageRange;

    [Header("Impact Frames")]
    [SerializeField] private float impactFramesDuration = 0.3f;
    private Coroutine impactFramesCoroutine;
    private List<Enemy> enemiesHitByCurrentAttack = new List<Enemy>();

    [Header("Trail")]
    [SerializeField] private GameObject trailObject;

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
        if (!capsuleCollider.enabled) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy == null) return;

        if (enemiesHitByCurrentAttack.Contains(enemy)) return;
        enemiesHitByCurrentAttack.Add(enemy);

        StartImpactFrames(0.1f);
        CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

        Vector3 hitPoint = other.ClosestPointOnBounds(colliderStartTransform.position);

        CreateTempHitVisual(hitPoint, Color.green, 1.5f);

        enemy.TakeDamage(GetRandomDamage(), hitPoint);
    }

    private void HandleHitDetectionBetweenFrames()
    {
        if (!capsuleCollider.enabled)
        {
            currentHitFrame = 0;
            return;
        }

        previousFrameCollisionRay = currentFrameCollisionRay;

        Vector3 dir = colliderEndTransform.position - colliderStartTransform.position;
        currentFrameCollisionRay = new Ray(colliderStartTransform.position, dir);

        if(currentHitFrame > 0)
        {
            int segments = (int)Mathf.Ceil(dir.magnitude / capsuleCollider.radius);
            for(int i = 0; i <= segments; i++)
            {
                Vector3 currPoint = currentFrameCollisionRay.origin + i / (float)segments * currentFrameCollisionRay.direction;
                Vector3 prevPoint = previousFrameCollisionRay.origin + i / (float)segments * previousFrameCollisionRay.direction;

                CheckCollisionsWithRays(new Ray(prevPoint, currPoint-prevPoint), Vector3.Distance(currPoint, prevPoint));

                Debug.DrawLine(currPoint, prevPoint, Color.red);
            }
        }

        currentHitFrame++;
    }

    private void CheckCollisionsWithRays(Ray ray, float distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, damageableCollidersLayerMask);

        if (hits == null) return;
        if (hits.Length == 0) return;

        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

            if (enemy == null) continue;

            if (enemiesHitByCurrentAttack.Contains(enemy)) continue;
            enemiesHitByCurrentAttack.Add(enemy);

            StartImpactFrames(0.1f);
            CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

            Vector3 hitPoint = hit.collider.ClosestPointOnBounds(hit.point);
            if (hit.distance == 0) hitPoint = hit.collider.ClosestPointOnBounds((colliderStartTransform.position + colliderEndTransform.position) / 2);

            CreateTempHitVisual(hitPoint, Color.red, 1.5f);

            enemy.TakeDamage(GetRandomDamage(), hitPoint);
        }
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
