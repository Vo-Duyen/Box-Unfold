using UnityEngine;

public class Masked3d : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().material.renderQueue = 3201;
        // MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        // meshRenderer.receiveShadows = true;
    }
}
