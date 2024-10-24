using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using static EnemySpawner;
using UnityEngine.InputSystem.HID;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    [Header("Weapon: References")]
    [SerializeField, Self] private CapsuleCollider capsuleCollider;
    [SerializeField, Anywhere] private GameObject trailObject;
    [SerializeField, Parent] private Entity holderEntity;
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
    [HideInInspector] public UnityEvent<Entity, Entity, Vector3> OnWeaponHit = new UnityEvent<Entity, Entity, Vector3>();

    [Header("Weapon: Combo")]
    public List<ComboDataSO> Combos;
    private Vector2Int damageRange;

    [Header("Weapon: Impact Frames")]
    [SerializeField] private float impactFramesDuration = 0.15f;
    private List<Entity> enemiesHitByCurrentAttack = new List<Entity>();

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

        Entity enemy = other.GetComponentInParent<Entity>();

        Vector3 hitPoint = other.ClosestPointOnBounds(colliderStartTransform.position);

        AttemptToHitEnemy(enemy, hitPoint, true);
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

                Debug.DrawLine(currPoint, prevPoint, Color.red, 2f);
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
            Vector3 hitPoint = hit.collider.ClosestPointOnBounds(hit.point);
            if (hit.distance == 0) hitPoint = hit.collider.ClosestPointOnBounds((colliderStartTransform.position + colliderEndTransform.position) / 2);

            Entity enemy = hit.collider.GetComponentInParent<Entity>();

            AttemptToHitEnemy(enemy, hitPoint, false);
        }
    }

    private void AttemptToHitEnemy(Entity victim, Vector3 hitPoint, bool fromTrigger)
    {
        if (victim == null) return;
        if (victim.Team == holderEntity.Team) return;
        if (victim.CurrentState == victim.EntityDeathState) return;

        if (enemiesHitByCurrentAttack.Contains(victim)) return;
        enemiesHitByCurrentAttack.Add(victim);

        HitEnemy(victim, hitPoint, fromTrigger);
    }

    private void HitEnemy(Entity victim, Vector3 hitPoint, bool fromTrigger)
    {
        StartImpactFrames(0.1f);
        CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

        CreateTempHitVisual(hitPoint, fromTrigger ? Color.green : Color.red, 1.5f);

        victim.TakeDamage(GetRandomDamage(), hitPoint, holderEntity);

        OnWeaponHit?.Invoke(holderEntity, victim, hitPoint);
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
        DOTween.Kill("ImpactFrames");
        Time.timeScale = 1f;

        float speedUpTime = impactFramesDuration / 4f;

        Sequence impactFrameSequence = DOTween.Sequence().SetId("ImpactFrames");
        impactFrameSequence.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScale, impactFramesDuration - speedUpTime).SetEase(Ease.OutQuint)).SetUpdate(true);
        impactFrameSequence.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, speedUpTime).SetEase(Ease.InCubic)).SetUpdate(true);
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
