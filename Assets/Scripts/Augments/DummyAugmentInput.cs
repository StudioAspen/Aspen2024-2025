using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAugmentInput : MonoBehaviour
{
    private AugmentManager augmentManager;

    // Start is called before the first frame update
    void Start()
    {
        augmentManager = FindAnyObjectByType<AugmentManager>();
        augmentManager.IncrementLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Debug.Log(augmentManager.AddAugment<JumpSoundAugment>() ? "success" : "failure");
        }
    }
}
