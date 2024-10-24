using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : MonoBehaviour
{
    [Header("Augment Attributes")]
    [SerializeField] public AugmentBranch Branch;
    [SerializeField] public int Level;

    protected Player player;

    public virtual void Start()
    {
        player = GetComponent<AugmentManager>().Player.GetComponent<Player>();
    }
}
