using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitNumbers : MonoBehaviour
{
    public TMP_Text numberText;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void ActivateHitNumberText(int damage)
    {
        numberText.text = damage.ToString();
        numberText.fontSize = (500 - 200) * (damage / 10000) + 200;

        StartCoroutine(FloatUpAndFadeCoroutine(2f, 0.5f));
    }

    private IEnumerator FloatUpAndFadeCoroutine(float duration, float speed)
    {
        for(float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            transform.Translate(speed * Vector3.up * Time.unscaledDeltaTime);
            numberText.color = new Color(255f, 0f, 0f, 1 - Mathf.Pow(t / duration, 2));
            yield return null;
        }
        numberText.color = Color.clear;

        Destroy(gameObject);
    }
}
