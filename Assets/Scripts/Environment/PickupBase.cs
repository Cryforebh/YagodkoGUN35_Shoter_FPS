using UnityEngine;

[RequireComponent (typeof (SoundPickup))]
public class PickupBase : MonoBehaviour
{
    public float timeRespawn = 60;

    protected float _currentTimeRespawn = 0;
    protected bool _isEquip = false;
    protected SoundPickup _sound;
    protected Collider _collider;
    protected MeshRenderer _meshRenderer;
    protected MeshRenderer[] _meshes;

    private void Start()
    {
        _sound = GetComponent<SoundPickup>();
        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshes = GetComponentsInChildren<MeshRenderer>();
    }

    protected void SetEnable(bool isEnable)
    {
        _isEquip = !isEnable;
        _collider.enabled = isEnable;
        _meshRenderer.enabled = isEnable;
        foreach (var obj in _meshes) obj.enabled = isEnable;
        _sound.PlaySound();
    }

    private void Update()
    {
        if (_isEquip)
        {
            _currentTimeRespawn += Time.deltaTime;
            if (_currentTimeRespawn >= timeRespawn)
            {
                _currentTimeRespawn = 0;
                SetEnable(true);
            }
        }
    }
}
