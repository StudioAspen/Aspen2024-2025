﻿using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    private Vector3 slideDirection;

    public PlayerSlideState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Slide State");

        player.TransitionToAnimation("Falling");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.IsGrounded = true;

        player.ApplySlide(slideDirection);

        // moving stuff
        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.HandleMovingVelocity();
        player.GroundedMove();

        if (!player.IsAbleToSlide()) player.ChangeState(player.DefaultState);
    }

    public override void FixedUpdate()
    {

    }

    public void SetSlideDirection(Vector3 dir)
    {
        slideDirection = dir;
    }
}