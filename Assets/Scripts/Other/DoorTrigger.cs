using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private int _triggerCount = 0;
    private bool _isPrivate = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
            _triggerCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
            _triggerCount--;
    }

    private bool IsValidObject(Collider other) => other.tag == "Player" || (!_isPrivate && other.tag == "Agent");

    public bool IsInTrigger() => _triggerCount > 0;
    public void SetTriggerPrivate(bool isPrivate) => _isPrivate = isPrivate;
}
