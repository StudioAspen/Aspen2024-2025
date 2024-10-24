using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumSystem : MonoBehaviour
{
    private int momentum;
    public int Momentum => momentum;

    private float timer;
    [SerializeField] private float baseTimeBetween = 3f;
    [SerializeField] private float timeBetweenMultiplier = 0.975f;
    private float timeBetween;

    public void AddMomentum() {
        momentum++;
        timer = 0;
        timeBetween = timeBetween*timeBetweenMultiplier;
    }

    private void Reset() {
        timer = 0;
        timeBetween = baseTimeBetween;
        momentum = 0;
    }
    void Start() {
        timeBetween = baseTimeBetween;
    }
    // Update is called once per frame
    void Update()
    {
        if (momentum > 0) {
            timer+=Time.deltaTime; 
        }
        if (timer > timeBetween) {
            Reset();
        }
    }
}
