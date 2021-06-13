using System;
using System.Collections.Generic;
using UnityEngine;

public class YeetableRock : MonoBehaviour
{
    [SerializeField] private LayerMask _rockMask;
    [SerializeField] private bool _drawGizmos;
    [Space]
    [SerializeField] private StaticParticles _teaseParticles;
    [SerializeField] private StaticParticles _breakParticles;

    [Header("Desing variables")]
    [SerializeField] private float _neighborRadius = 1;
    [SerializeField] private float _speedMagnitudeToTrigger = 0.1f;
    [SerializeField] private float _explosionIntensityMod = 1f;
    [SerializeField] private float _explosionYSpeedMod = 1f;
    [SerializeField] private float _SpeedGainOnBreakMod = 1f;

    Rigidbody _rb;
    Collider _c;

    private bool collisionYeetLock;
    private YeetableRock[] _neighbors;
    private void Awake()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
        _c = GetComponent<Collider>();
        _rb.isKinematic = true;
    }
    private void Start()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _neighborRadius, _rockMask);

        _neighbors = new YeetableRock[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            YeetableRock rocc = hits[i].GetComponent<YeetableRock>();
            if (rocc != default)
                rocc.OnYeeted += Yeet;
            _neighbors[i] = rocc;
        }
    }

    public void Yeet(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode, Collider garryC)
    {
        if (collisionYeetLock) return;
        _rb.isKinematic = false;
        _rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, mode);
        collisionYeetLock = true;
        QueueDestruction();
        OnYeeted?.Invoke(explosionForce, explosionPosition, explosionRadius, upwardsModifier, mode, garryC);
        Physics.IgnoreCollision(_c, garryC, true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (collisionYeetLock) return;
        GarryController garryController = other.collider.GetComponentInParent<GarryController>();
        if (garryController != default)
        {
            float mag = garryController.Rb.velocity.magnitude;
            if (mag >= _speedMagnitudeToTrigger)
            {
                Yeet(6 * _explosionIntensityMod, garryController.transform.position, 4, UnityEngine.Random.value * 30f * _explosionYSpeedMod, ForceMode.Impulse, other.collider);
                _breakParticles.PlayPS(other.contacts[0].point);
                garryController.Rb.AddForce(other.contacts[0].normal * 12 * _SpeedGainOnBreakMod, ForceMode.Impulse);
            }
            else
                _teaseParticles.PlayPS(other.contacts[0].point);
            Debug.Log("Garry Hit \"" + name + "\" with Mag: " + mag);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, _neighborRadius);
    }

    private void QueueDestruction()
    {
        LeanTween.scale(gameObject, Vector3.zero, UnityEngine.Random.Range(0.5f, 0.7f))
            .setDelay(UnityEngine.Random.Range(1f, 5f))
            .setEaseInBack()
            .setOnComplete(() => Destroy(gameObject));
    }

    private Action<float, Vector3, float, float, ForceMode, Collider> OnYeeted;
}