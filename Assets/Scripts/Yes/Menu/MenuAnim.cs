using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnim : MonoBehaviour
{
    Vector3 pos;
    private void Start()
    {
        pos = transform.position;
    }
    public void TweenDown()
    {
        gameObject.SetActive(true);
        transform.position += Vector3.up * 50f;
        LeanTween.moveY(gameObject, pos.y, 1f);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
