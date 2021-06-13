using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWalkParticles : MonoBehaviour
{
    private GarryController controller;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Color color;
    private ParticleSystem instance;
    private ParticleSystem.MainModule mainModule;
    ParticleSystemRenderer particleSystemRenderer;
    private RaycastHit hit;
    private void Awake()
    {
        controller = GetComponentInParent<GarryController>();
        instance = Instantiate(particles, Vector3.zero, Quaternion.identity);
        particleSystemRenderer = instance.gameObject.GetComponent<ParticleSystemRenderer>();
        instance.transform.forward = Vector3.up;
        mainModule = instance.main;
    }

    private void Update()
    {
        instance.gameObject.SetActive(controller.Grounded);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, ~LayerMask.GetMask("Player")))
        {
            // Renderer renderer = hit.collider.GetComponent<Renderer>();
            // if (renderer)
            // {
            //     Texture2D tex = renderer.material.mainTexture as Texture2D;
            //     if (renderer && tex)
            //     {
            //         Vector2 coord = hit.textureCoord;

            //         coord.x *= tex.width;
            //         coord.y *= tex.height;

            //         Vector2 tiling = renderer.material.mainTextureScale;
            //         color = tex.GetPixel(Mathf.FloorToInt(coord.x * tiling.x), Mathf.FloorToInt(coord.y * tiling.y));
            //         UniversalGameData.WalkingColor = color;
            //     }
            //     else if (renderer)
            //     {
            //         color = renderer.material.color;
            //         UniversalGameData.WalkingColor = color;
            //         mainModule.startColor = color;
            //     }

            //     // Brighten it up a bit
            //     float h;
            //     float s;
            //     float v;
            //     Color.RGBToHSV(color, out h, out s, out v);
            //     v = Mathf.Clamp(v + 0.25f, 0, 1);
            //     color = Color.HSVToRGB(h, s, v);
            //     particleSystemRenderer.material.SetColor("_BaseColor", color);
            // }

            mainModule.startColor = UniversalGameData.WalkingColor;
            instance.transform.forward = hit.normal;
            instance.transform.position = hit.point;
        }
    }
}