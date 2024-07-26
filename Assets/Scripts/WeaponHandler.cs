using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private bool canHit;

    [Header("Combo")]
    public Combo Combo;

    [Header("Impact Frames")]
    [SerializeField] private float impactFramesDuration = 0.3f;
    private Coroutine impactFramesCoroutine;
    private List<Enemy> enemiesHitByCurrentAttack = new List<Enemy>();

    private void OnTriggerStay(Collider other)
    {
        if (!canHit) return;

        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemiesHitByCurrentAttack.Contains(enemy)) return;
            enemiesHitByCurrentAttack.Add(enemy);

            StartImpactFrames(0.025f);
            CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.GetComponent<Collider>().enabled = false;
            temp.transform.localScale = 0.1f * Vector3.one;
            temp.transform.position = hitPoint;
            temp.GetComponent<Renderer>().material.color = Color.black;
            Destroy(temp, 2f);

            enemy.TakeDamage(Random.Range(100, 10000), hitPoint);
        }
    }

    public void StartImpactFrames(float timeScale)
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

    public void ClearEnemiesHitList()
    {
        enemiesHitByCurrentAttack.Clear();
    }

    public void EnableTriggers()
    {
        canHit = true;
    }

    public void DisableTriggers()
    {
        canHit = false;
    }
}
