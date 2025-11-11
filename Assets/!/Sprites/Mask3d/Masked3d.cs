using UnityEngine;

public class Masked3d : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().material.renderQueue = 3201;
    }
}
