using UnityEngine;

public class CheckingNearShots : MonoBehaviour
{
    public float Range = 10f; // дальность проверки
    public LayerMask LayerBullet;
    public bool Shoting = false;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range, LayerBullet);
        if (hitColliders.Length > 0)
        {
            Shoting = true;
            Debug.Log("Рядом был выстрел!");
        }
        else
        {
            Shoting = false;
        }
    }
}
