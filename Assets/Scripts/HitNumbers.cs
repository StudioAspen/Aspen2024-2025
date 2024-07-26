using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitNumbers : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private float minTextSize = 200f;
    [SerializeField] private float maxTextSize = 400f;
    [SerializeField] private int maxDamage = 10000;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void ActivateHitNumberText(int damage)
    {
        numberText.text = damage.ToString();
        numberText.fontSize = (maxTextSize - minTextSize) * (damage / (float)maxDamage) + minTextSize;

        StartCoroutine(FloatUpAndFadeCoroutine(2f, 0.5f));
    }

    private IEnumerator FloatUpAndFadeCoroutine(float duration, float speed)
    {
        float fullColorDuration = duration / 2;

        for(float t = 0; t < fullColorDuration; t += Time.unscaledDeltaTime)
        {
            numberText.color = Color.red;
            transform.Translate(speed * Vector3.up * Time.unscaledDeltaTime);
            yield return null;
        }

        for(float t = 0; t < fullColorDuration; t += Time.unscaledDeltaTime)
        {
            transform.Translate(speed * Vector3.up * Time.unscaledDeltaTime);
            numberText.color = new Color(255f, 0f, 0f, 1 - Mathf.Pow(t / fullColorDuration, 2));
            yield return null;
        }
        numberText.color = Color.clear;

        Destroy(gameObject);
    }
}
