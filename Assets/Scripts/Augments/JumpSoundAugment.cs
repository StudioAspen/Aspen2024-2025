using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSoundAugment : Augment
{
    private Player player;
    private AudioSource audioSource;

    private bool soundTriggered = false;

    [SerializeField] private AudioClip jumpSound;

    void Awake()
    {
        jumpSound = (AudioClip)Resources.Load("sm64_mario_yahoo");
        player = GetComponent<Player>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = jumpSound;
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

    public override AugmentBranch GetBranch() { return AugmentBranch.MARIO_BRANCH;  }
    public override int GetLevel() { return 1; }
}
