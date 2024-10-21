using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int momentum;
    private float timer;
    [SerializeField] private float baseTimeBetween;
    [SerializeField] private float timeBetweenMultiplier;
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
