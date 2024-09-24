using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public PlayerController playerMovement;

    private void Update()
    {
        //if (playerMovement.Jump)
        //{
        //    // camera start following
        //    vCam.Priority = 10;
        //}
        //else
        //{
        //    //stop follow
        //    vCam.Priority = 0;
        //}
    }
}
