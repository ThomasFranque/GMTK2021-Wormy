using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Visuals/Static Particle")]
public class StaticParticles : ScriptableObject
{
    [SerializeField] private int _initialPoolSize = 5;
    [SerializeField] private GameObject _particlePrefab;
    public Queue<ParticleSystem> PSQueue { get; private set; }

    public ParticleSystem NextUp => PSQueue.Peek();
    public ParticleSystem LastUsed { get; private set; }

    private void CheckIfCreated()
    {
        if (PSQueue == default)
            CreateParticles(_initialPoolSize);
    }

    private ParticleSystem CreateParticles(int amount)
    {
        ParticleSystem lastCreated = default;
        if (PSQueue == default)
            PSQueue = new Queue<ParticleSystem>(amount);
        for (int i = 0; i < amount; i++)
        {
            GameObject newGO = GameObject.Instantiate(_particlePrefab);
            lastCreated = newGO.GetComponent<ParticleSystem>();
            PSQueue.Enqueue(lastCreated);
            newGO.hideFlags = HideFlags.HideInInspector;
            DontDestroyOnLoad(newGO);
        }

        return lastCreated;
    }

    private void MoveFirstToLast()
    {
        ParticleSystem f = PSQueue.Dequeue();
        PSQueue.Enqueue(f);
    }

    private ParticleSystem GetNextUsable()
    {
        int _collectionSize = PSQueue.Count;
        ParticleSystem nextUsable = default;
        for (int i = 0; i < _collectionSize && nextUsable == default; i++)
        {
            if (NextUp == default)
            {
                PSQueue.Dequeue();
                continue;
            }
            if (NextUp.isPlaying)
                MoveFirstToLast();
            else
                nextUsable = NextUp;
        }

        if (nextUsable == default)
        {
            nextUsable = CreateParticles(1);
        }

        return nextUsable;
    }

    public void PlayPS(Vector3 worldPos, Transform parent = default)
    {
        CheckIfCreated();

        LastUsed = GetNextUsable();
        LastUsed.transform.SetParent(parent);
        LastUsed.transform.position = worldPos;
        LastUsed.Play();
    }

    public void StopLastUsedPS()
    {
        CheckIfCreated();
        
        LastUsed.Stop();
    }

    public void ResetLastUsedPS()
    {
        CheckIfCreated();

        LastUsed.Stop();
        LastUsed.Play();
    }

    public void StopAll()
    {
        CheckIfCreated();

        foreach (ParticleSystem p in PSQueue)
            p.Stop();
    }

    public void ResetAll()
    {
        CheckIfCreated();

        foreach (ParticleSystem p in PSQueue)
        {
            p.Stop();
            p.Play();
        }
    }

}