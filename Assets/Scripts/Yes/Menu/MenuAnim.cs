using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnim : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;

    public void TweenDown()
    {
        if (transform.position.y < 1) return;
        gameObject.SetActive(true);
        LeanTween.moveY(gameObject, 0, _speed);
    }

    private void OnTriggerExit(Collider other)
    {
        LeanTween.moveY(gameObject, 15, _speed);
    }
}
