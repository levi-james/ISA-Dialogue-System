using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueObject", menuName ="Dialogue")]
public class DialogueObject : ScriptableObject
{
    [Header("Dialogue")]
    public List<DialogueSegment> dialogueSegments= new List<DialogueSegment>();

    [Header("Follow on dialogue - optional")]
    public DialogueObject endDialogue;

}

[System.Serializable]
public struct DialogueSegment
{
    public string dialogueText;
    public string speakerName;
    public float dialogueDisplayTime;
    public List<DialogueChoice> dialogueChoices;
    

}

[System.Serializable]
public struct DialogueChoice
{
    public string dialogueChoice;
    public DialogueObject followUpDialogue;
}


