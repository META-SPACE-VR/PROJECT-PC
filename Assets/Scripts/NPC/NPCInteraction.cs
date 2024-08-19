using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class NPCInteraction : MonoBehaviour
{
    public GameObject player; // Reference to the player
    public GameObject dialoguePanel; // Reference to the dialogue UI panel
    public TMP_Text dialogueText; // Reference to the Text UI element
    public GameObject wheelchair; // Reference to the wheelchair
    public Transform sitArea; // Reference to the sit area on the wheelchair
    public float interactionDistance; // Distance to check if the wheelchair is nearby
    public bool isSittingInWheelchair = false;
    private bool playerNearby = false;
    public bool isInteracting = false;
    private int dialogueStep = 0;
    public Animator npcAnimator;


    private string[] initialDialogueLines = new string[]
    {
        "도와주세요..!",
        "폭발물에 다리가 맞아서 움직일 수가 없어요.. 혹시 탈 것 좀 가져다주실수 있나요..?"
    };

    private string[] wheelchairFoundDialogue = new string[]
    {
        "휠체어를 가지고 오셨네요..! 감사합니다 .."
    };

    private string[] wheelchairNotFoundDialogue = new string[]
    {
        "옆에 의무실에 휠체어가 있었던것 같아요.."
    };

    private bool initialDialogueComplete = false;

    void Start()
    {
        dialoguePanel.SetActive(false); // Initially hide the dialogue panel
        AddEventTriggerListener(dialoguePanel, EventTriggerType.PointerClick, OnDialoguePanelClick);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                playerController.SetCurrentNPC(this);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                playerController.ClearCurrentNPC();
            }
        }
    }

    public void StartInteraction()
    {
        isInteracting = true;
        dialogueStep = 0;
        dialoguePanel.SetActive(true);

        if (!initialDialogueComplete)
        {
            dialogueText.text = initialDialogueLines[dialogueStep];
        }
        else
        {
            if (IsWheelchairNearby())
            {
                dialogueText.text = wheelchairFoundDialogue[dialogueStep];
                SitInWheelchair();
            }
            else
            {
                dialogueText.text = wheelchairNotFoundDialogue[dialogueStep];
            }
        }
    }

    public void AdvanceDialogue()
    {
        if (!initialDialogueComplete)
        {
            dialogueStep++;
            if (dialogueStep < initialDialogueLines.Length)
            {
                dialogueText.text = initialDialogueLines[dialogueStep];
            }
            else
            {
                EndInitialDialogue();
            }
        }
        else if (IsWheelchairNearby())
        {
            dialogueStep++;
            if (dialogueStep < wheelchairFoundDialogue.Length)
            {
                dialogueText.text = wheelchairFoundDialogue[dialogueStep];
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            dialogueStep++;
            if (dialogueStep < wheelchairNotFoundDialogue.Length)
            {
                dialogueText.text = wheelchairNotFoundDialogue[dialogueStep];
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void EndInitialDialogue()
    {
        initialDialogueComplete = true;
        isInteracting = false;
        dialoguePanel.SetActive(false);
    }

    private void EndDialogue()
    {
        isInteracting = false;
        dialoguePanel.SetActive(false);
    }

    private bool IsWheelchairNearby()
    {
        float distance = Vector3.Distance(transform.position, wheelchair.transform.position);
        return distance <= interactionDistance;
    }

    public void SitInWheelchair()
    {
        transform.position = sitArea.position;
        transform.rotation = sitArea.rotation;
        transform.SetParent(wheelchair.transform);
        isSittingInWheelchair = true;
        npcAnimator.SetBool("Laying", false);
        Debug.Log("NPC is now in the wheelchair.");
    }

    public void LayOnBed(Transform bedTransform)
    {
        // Move the NPC to the bed
        transform.SetParent(bedTransform);
        transform.position = bedTransform.position;
        transform.rotation = bedTransform.rotation;
        isSittingInWheelchair = false;
        npcAnimator.SetBool("Laying", true);
        Debug.Log("NPC is now on the bed.");
    }

    private void OnDialoguePanelClick(BaseEventData eventData)
    {
        if (isInteracting)
        {
            AdvanceDialogue();
        }
    }

    private void AddEventTriggerListener(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }
}
