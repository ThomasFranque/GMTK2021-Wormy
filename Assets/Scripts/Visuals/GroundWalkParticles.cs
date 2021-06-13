using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWalkParticles : MonoBehaviour
{
    private GarryController controller;
    [SerializeField] private ParticleSystem particles;
    private ParticleSystem instance;
    private RaycastHit hit;
    private void Awake() 
    {
        controller = GetComponentInParent<GarryController>();
        instance = Instantiate(particles, Vector3.zero, Quaternion.identity);
        instance.transform.forward = Vector3.up;
    }

    private void Update() 
    {
        instance.gameObject.SetActive(controller.Grounded);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, ~LayerMask.GetMask("Player")))
        {
            instance.transform.position = hit.point;
        }
    }
}
