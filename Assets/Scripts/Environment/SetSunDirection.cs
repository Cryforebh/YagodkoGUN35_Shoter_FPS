using UnityEngine;

[ExecuteInEditMode]
public class SetSunDirection : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("_SunDirection", transform.forward);
    }
}
