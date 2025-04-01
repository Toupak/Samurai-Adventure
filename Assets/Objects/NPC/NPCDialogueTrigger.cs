using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTrigger : Interactable
{
    public DialogueData dialogueData;

    void Start()
    {
        OnTrigger.AddListener(TriggerDialogue);
    }

    private void TriggerDialogue()
    {
        DialoguePanel.Instance.DisplayDialogue(dialogueData);
    }
}
