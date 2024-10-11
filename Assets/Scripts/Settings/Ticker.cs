using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ticker : MonoBehaviour
{
    public static Ticker Instance;

    [SerializeField] private float tickDuration = 0.2f;
    private float tickTimer;

    [HideInInspector] public UnityEvent OnTick = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;

        if(tickTimer > tickDuration)
        {
            tickTimer = 0f;

            OnTick?.Invoke();
            Debug.Log("Tick");
        }
    }
}
