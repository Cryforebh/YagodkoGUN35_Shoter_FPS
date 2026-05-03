using UnityEngine;

public class SmoothTime : MonoBehaviour
{
    [SerializeField] private float _targetTime = 1;

    private void Update()
    {
        Time.timeScale = _targetTime;
    }
}
