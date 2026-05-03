using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    public float dieForce = 10.0f;
    public float maxSightDistance = 5.0f;
    public float findWeaponSpeed = 5.0f;
    public float findTargetSpeed = 5.0f;

    [Header("Attack State")]
    public float AttackSpeed = 3.0f;
    public float AttackSpeedKnifeFighter = 5.0f;
    public float AttackStoppingDistance = 5.0f;
    public float AttackStoppingDistanceKnifeFighter = 1.2f;
    public float AttackCloseRange = 7.0f;
    public float AttackKnifeDistanceMax = 1.4f;
}
