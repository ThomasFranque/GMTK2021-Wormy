using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public Color color = Color.white;
    [TextArea]
    public string[] lines;
    public bool giveLeaf;
}
