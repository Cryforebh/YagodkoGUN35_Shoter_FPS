using Cinemachine;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public CharacterAiming characterAiming;
    [HideInInspector] public CinemachineImpulseSource cameraShake;
    [HideInInspector] public Animator animator;

    public Vector2[] recoilPattern;
    public string nameRecoil = "default";
    public float duration;
    public float recoilModifier = 1.0f;

    float verticalRecoil;
    float horizontalRecoil;
    float time;
    int index;

    int recoilLayerIndex = -1;

    private void Awake()
    {
        cameraShake = GetComponent<CinemachineImpulseSource>();
        characterAiming = GetComponent<CharacterAiming>();
    }

    private void Start()
    {
        if (animator)
        {
            recoilLayerIndex = animator.GetLayerIndex("Recoil Layer");
        }
    }

    public void Reset()
    {
        index = 0;
    }

    int NextIndex(int index)
    {
        return (index + 1) % recoilPattern.Length;
    }

    public void GenerateRecoil(string weaponName)
    {

        if (!characterAiming)
        {
            return;
        }

        time = duration;

        cameraShake.GenerateImpulse(Camera.main.transform.forward);

        horizontalRecoil = recoilPattern[index].x;
        verticalRecoil = recoilPattern[index].y;

        index = NextIndex(index);

        if (animator)
        {
            if (nameRecoil == "default")
                animator.Play("weapon_" + weaponName + "_recoil", recoilLayerIndex, 0.0f);
            else
                animator.Play("weapon_" + nameRecoil + "_recoil", recoilLayerIndex, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!characterAiming)
        {
            return;
        }

        if (time > 0)
        {
            var nextY = (((verticalRecoil / 10) * Time.deltaTime) / duration) * recoilModifier;
            var nextX = (((horizontalRecoil / 10) * Time.deltaTime) / duration) * recoilModifier;
            //characterAiming.yAxis -= nextY;
            //characterAiming.xAxis -= nextX;

            characterAiming.AddCinemachineTargetYaw(nextX);
            characterAiming.AddCinemachineTargetPitch(nextY);

            time -= Time.deltaTime;
        }

    }
}
