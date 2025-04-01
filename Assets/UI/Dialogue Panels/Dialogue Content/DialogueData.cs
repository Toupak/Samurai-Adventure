using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "DialogueData")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    [TextArea] public List<string> textContent;
    public Sprite characterImage;
    public List<AudioClip> characterSounds;
}

