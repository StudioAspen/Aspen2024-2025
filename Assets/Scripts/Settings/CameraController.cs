using Cinemachine;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private CinemachineVirtualCamera vCam;
    [SerializeField, Self] private CinemachineInputProvider inputProvider;
    [SerializeField, Scene] private Player player;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Start()
    {
        AttachToPlayer();
    }

    private void AttachToPlayer()
    {
        vCam.LookAt = player.transform;
        vCam.Follow = player.transform;
    }

    public void DisableCameraInputs()
    {
        inputProvider.enabled = false;
    }

    public void EnableCameraInputs()
    {
        inputProvider.enabled = true;
    }
}
