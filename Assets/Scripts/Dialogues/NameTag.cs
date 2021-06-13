using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameTag : MonoBehaviour
{
    public void SetTag(DialogueData data)
    {
        foreach(Transform t in transform)
        {
            Image img = t.GetComponent<Image>();
            if (img)
            {
                img.color = data.color;
            }
        }

        var text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = data.characterName;
    }
}
