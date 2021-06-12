using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    [SerializeField] private float amount;
    [SerializeField] private float speed;
    Transform[] children;

    private void Awake() 
    {
        children = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    private void Update() 
    {
        for (int i = 0; i < children.Length; i++)
        {
            Vector3 offset = Wob(Time.time + i);
            children[i].position = children[i].position + offset;
        }
    }

    private Vector2 Wob(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f) * speed, Mathf.Cos(time * 2.6f) * speed) * amount;
    }
}
