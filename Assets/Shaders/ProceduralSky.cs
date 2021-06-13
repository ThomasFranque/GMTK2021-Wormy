using UnityEngine;

[ExecuteAlways]
public class ProceduralSky : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private GameObject _light;

    [SerializeField] private float _time = 0;

    // Update is called once per frame
    private void Update()
    {
        _mat.SetVector("_direction", _light.transform.forward);
        _mat.SetFloat("_time", _time);
    }
}