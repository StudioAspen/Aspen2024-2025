using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Coroutine takeDamageCoroutine;

    [SerializeField] private HitNumbers hitNumberPrefab;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        IgnoreCollisionsWithSelf();
    }

    private void IgnoreCollisionsWithSelf()
    {
        Collider[] colliders = GetComponents<Collider>();

        foreach(Collider c1 in colliders)
        {
            foreach(Collider c2 in colliders)
            {
                Physics.IgnoreCollision(c1, c2);
            }
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        if(takeDamageCoroutine != null) StopCoroutine(takeDamageCoroutine);
        takeDamageCoroutine = StartCoroutine(TakeDamageCoroutine(0.1f));

        HitNumbers hitNumber = Instantiate(hitNumberPrefab, hitPoint, Quaternion.identity);
        hitNumber.ActivateHitNumberText(damage);
    }

    private IEnumerator TakeDamageCoroutine(float fadeDuration)
    {
        animator.CrossFadeInFixedTime("Hit_F_1_InPlace", fadeDuration);

        float hitDuration = GetAnimationDuration("Hit_F_1_InPlace");
        yield return new WaitForSeconds(hitDuration);

        animator.CrossFadeInFixedTime("FlatMovement", fadeDuration);
    }

    private float GetAnimationDuration(string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        if (clips == null) return 0f;
        if (clips.Length == 0) return 0f;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName) return clip.length;
        }

        return 0f;
    }
}
