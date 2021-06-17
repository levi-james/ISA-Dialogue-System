using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DialogueInteract : MonoBehaviour
{
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] Text dialogueText;
    [SerializeField] Text speakerName;
    [SerializeField] GameObject OptionsWrapper;
    [SerializeField] Transform OptionsButtonSpawnParent;
    [SerializeField] GameObject OptionsButton;
    [SerializeField] DialogueObject startdialogueObject;

    bool dialogueStarted = false;
    bool optionSelected = false;
    bool clickDisabled = false;


    bool skipButtonPressed;

    [SerializeField] float typingSpeed;

    //E button graphic
    private GameObject EbtnInstance;
    [SerializeField] GameObject EButton;
    bool alreadySpawned = false;
    [SerializeField] Transform UIposition;

    private void Update()
    {
        /*Skip dialogue input*/
        if (Input.GetKey(KeyCode.E) && dialogueStarted)
        {
            skipButtonPressed = true;
        }
        else
        {
            skipButtonPressed = false;
        }
        Debug.Log("buttonpressed? " + skipButtonPressed);

    }
    private void Start()
    {
        /*Set UI inactive*/
        OptionsWrapper.SetActive(false);
        dialogueCanvas.enabled = false;
        optionSelected = false;
    }
    public void StartDialogue()
    {
        StartCoroutine(DisplayDialogue(startdialogueObject));
    }
    public void StartDialogue(DialogueObject _dialogueObject)
    {
        StartCoroutine(DisplayDialogue(_dialogueObject));
    }

    public void OptionSelected(DialogueObject selectedOption)
    {
        optionSelected = true;
        StartDialogue(selectedOption);
    }
    IEnumerator DisplayDialogue(DialogueObject _dialogueObject)
    {
        yield return null;

        List<GameObject> spawnedButtons = new List<GameObject>();
        dialogueCanvas.enabled = true;


        foreach (var dialogue in _dialogueObject.dialogueSegments)
        {
            dialogueText.text = ""; //show nothing in the text
            speakerName.text = dialogue.speakerName; //get the speaker name from the dialogue and put that as the speakername text

            if (dialogue.dialogueChoices.Count == 0) //if there are no dialogue choices in the current dialogue segment
            {
                
                /* trying out the typewriter effect on my own with stack overflow */
                //char x = dialogue.dialogueText[index];

                //char y;
                //for (int i = 0; i < x; i++)
                //{
                //    y = dialogue.dialogueText[i];
                //    dialogueText.text += y;
                //    Debug.Log(y);
                //    yield return new WaitForSeconds(typingSpeed);
                //}

                foreach (char letter in dialogue.dialogueText.ToCharArray()) //for each letter in the dialogue sentence
                {             
                    dialogueText.text += letter; //add one letter to the text box
                    yield return new WaitForSeconds(typingSpeed); //wait for a bit, then loop 
                }

                char[] charactersInSentence = dialogue.dialogueText.ToCharArray();

                if ( charactersInSentence.Length >= dialogue.dialogueText.Length - 1) //if the characters in the sentence are above or equal to the length of the length of the dialogue sentence
                {
                    if(!skipButtonPressed)
                    {
                        yield return new WaitForSeconds(dialogue.dialogueDisplayTime); //time to display the dialogue sentence
                        dialogueText.text = ""; //show nothing in the text
                    }
                    else if(skipButtonPressed)
                    {
                        yield return new WaitForSeconds(0);
                        dialogueText.text = ""; //show nothing in the text
                    }

                    
                }
                
            }
            else //if there ARE dialogue choices in the dialogue segment
            {
                dialogueText.text = dialogue.dialogueText;
                skipButtonPressed = false;
                OptionsWrapper.SetActive(true); //show the options

                foreach (var option in dialogue.dialogueChoices) //for every option in the dialogue choices
                {

                    GameObject newButton = Instantiate(OptionsButton, OptionsButtonSpawnParent); //add a new option button
                    var eventSystem = EventSystem.current;
                    eventSystem.SetSelectedGameObject(newButton, new BaseEventData(eventSystem));
                    spawnedButtons.Add(newButton); //add the button to the list of spawnedbuttons
                    newButton.GetComponent<UIDialogueOption>().Setup(this, option.followUpDialogue, option.dialogueChoice); //get the text and any other follow up dialogue and go to it

                }
                while (!optionSelected) //while no option is selected
                {
                    yield return null; //do nothing
                }
                dialogueText.text = "";
            }

        }

        OptionsWrapper.SetActive(false);
        dialogueCanvas.enabled = false;
        optionSelected = false;

        clickDisabled = false;

        spawnedButtons.ForEach(x => Destroy(x)); //for every button in the spawnedbutton list -> remove/destroy
        dialogueStarted = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !alreadySpawned)
        {
            EbtnInstance = Instantiate(EButton, transform.position, Quaternion.identity); //spawn "e button graphic" at npc position
            EbtnInstance.transform.position = UIposition.transform.position;

            alreadySpawned = true;
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E) && !dialogueStarted)
            {
                StartDialogue();
                dialogueCanvas.enabled = true;
                dialogueStarted = true;
                Debug.Log("dialogue started");
            }
            
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Destroy(EbtnInstance);
        alreadySpawned = false;
        if(dialogueStarted)
        {
            Destroy(EbtnInstance);

            OptionsWrapper.SetActive(false);
            dialogueCanvas.enabled = false;
            optionSelected = false;
        }
    }

    private void OnMouseDown()
    {
        if(!dialogueStarted && !clickDisabled)
        {
            StartDialogue();
            dialogueCanvas.enabled = true;
            dialogueStarted = true;
            Debug.Log("dialogue started");
            clickDisabled = true;
        }
    }

}
