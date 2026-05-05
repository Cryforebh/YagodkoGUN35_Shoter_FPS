using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCameraBase killCam;

    public void EnableKillCam()
    {
        killCam.Priority = 20;
    }
}
