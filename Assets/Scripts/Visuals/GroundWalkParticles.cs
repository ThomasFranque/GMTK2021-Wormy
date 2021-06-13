using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWalkParticles : MonoBehaviour
{
    private GarryController controller;
    [SerializeField] private ParticleSystem particles;
    private ParticleSystem instance;
    private ParticleSystem.MainModule mainModule;
    private RaycastHit hit;
    private void Awake() 
    {
        controller = GetComponentInParent<GarryController>();
        instance = Instantiate(particles, Vector3.zero, Quaternion.identity);
        instance.transform.forward = Vector3.up;
        mainModule = instance.main;
    }

    private void Update() 
    {
        instance.gameObject.SetActive(controller.Grounded);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, ~LayerMask.GetMask("Player")))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            Texture2D tex = renderer.material.mainTexture as Texture2D;
            Vector2 coord = hit.textureCoord;
            coord.x *= tex.width;
            coord.y *= tex.height;

            Vector2 tiling = renderer.material.mainTextureScale;
            Color color = tex.GetPixel(Mathf.FloorToInt(coord.x * tiling.x) , Mathf.FloorToInt(coord.y * tiling.y));
            UniversalGameData.WalkingColor = color;
            mainModule.startColor = color;
            instance.transform.position = hit.point;
        }
    }
}
