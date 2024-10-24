using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSoundAugment : Augment
{
    private AudioSource audioSource;

    private bool soundTriggered = false;

    [Header("Augment Parameters")]
    [SerializeField] private AudioClip jumpSound;

    public override void Start()
    {
        base.Start();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = jumpSound;
    }

    public void Awake()
    {
        Branch = AugmentBranch.MARIO_BRANCH;
        Level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsJumping && !soundTriggered)
        {
            audioSource.Play();
            soundTriggered = true;
        }
        else if (!player.IsJumping && soundTriggered)
        {
            soundTriggered = false;
        }
    }
}
