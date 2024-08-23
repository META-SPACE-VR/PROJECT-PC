using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;


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
    public GameObject npcStartDialogue;
    public GameObject sitInWheelchairPrompt; // E 버튼을 표시할 UI 요소
    public GameObject giveItemButton; // 아이템 주기 버튼
    private bool hasGivenNSAIDs = false;
    private bool hasGivenCartilageProtectant = false;
    public bool medicineGive = false; // To track if both items have been given
    public InventoryManager InventoryManager;



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
    private bool dialogueFinished = false; // 대화가 끝났는지 여부

    void Start()
    {
        dialoguePanel.SetActive(false);
        sitInWheelchairPrompt.SetActive(false);
        AddEventTriggerListener(dialoguePanel, EventTriggerType.PointerClick, OnDialoguePanelClick);

        StartCoroutine(CheckForItemsPeriodically());
    }   

    private IEnumerator CheckForItemsPeriodically()
    {
        while (true)
        {
            if (playerNearby)
            {
                CheckForItem();
            }
            yield return new WaitForSeconds(1.0f); // Check every second (adjust as needed)
        }
    }

    public void Update()
    {
        Debug.Log(medicineGive);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                player = playerController.gameObject; // player를 설정합니다.
                playerController.SetCurrentNPC(this);
                if (!dialogueFinished && IsWheelchairNearby() && !isSittingInWheelchair)
                {
                    npcStartDialogue.SetActive(true); // 대화 후 E 버튼을 표시
                }

                if (dialogueFinished && IsWheelchairNearby() && !isSittingInWheelchair)
                {
                    sitInWheelchairPrompt.SetActive(true); // 대화 후 E 버튼을 표시
                }
                playerNearby = true; // 플레이어가 근처에 있을 때
                CheckForItem();

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
                sitInWheelchairPrompt.SetActive(false); // 플레이어가 나가면 E 버튼 숨김
                npcStartDialogue.SetActive(false);

            }
        }
    }

    public void StartInteraction()
    {
        isInteracting = true;
        dialogueStep = 0;
        dialoguePanel.SetActive(true);
        npcStartDialogue.SetActive(false);

        if (!initialDialogueComplete)
        {
            dialogueText.text = initialDialogueLines[dialogueStep];
        }
        else
        {
            if (IsWheelchairNearby() && dialogueFinished)
            {
                SitInWheelchair();
            }
            if (IsWheelchairNearby() && !dialogueFinished)
            {
                dialogueText.text = wheelchairFoundDialogue[dialogueStep];
                SitInWheelchair();
                dialogueFinished = true;
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

        if (IsWheelchairNearby())
        {
            sitInWheelchairPrompt.SetActive(true); // 대화 후 E 버튼을 표시
        }
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
        dialoguePanel.SetActive(false);
        sitInWheelchairPrompt.SetActive(false); // E 버튼 숨김
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

    // 플레이어가 E 키를 눌렀을 때 휠체어에 앉히는 메서드
    public void OnSitInWheelchairButtonPressed()
    {
        if (IsWheelchairNearby() && !isSittingInWheelchair)
        {
            SitInWheelchair();
        }
    }

    private void CheckForItem()
    {
        Debug.Log("Checking for item...");
        
        Transform pickedItemPosition = player.transform.Find("PickedItemPosition");
        
        if (pickedItemPosition != null && pickedItemPosition.childCount > 0)
        {
            GameObject pickedItem = pickedItemPosition.GetChild(0).gameObject;
            string pickedItemName = pickedItem.name;

            Debug.Log($"Picked item: {pickedItemName}");

            // Handle the item based on its name
            if (pickedItemName == "NSAIDs" || pickedItemName == "연골보호제")
            {
                if (pickedItemName == "NSAIDs")
                {
                    hasGivenNSAIDs = true;
                }
                else if (pickedItemName == "연골보호제")
                {
                    hasGivenCartilageProtectant = true;
                }

                // Check if both items have been given
                medicineGive = hasGivenNSAIDs && hasGivenCartilageProtectant;
                
                string displayName = pickedItemName;
                giveItemButton.SetActive(true);
                giveItemButton.GetComponentInChildren<TMP_Text>().text = $"{displayName} 투여하기";
                Debug.Log($"Button activated with text: {giveItemButton.GetComponentInChildren<TMP_Text>().text}");
            }
            else
            {
                giveItemButton.SetActive(false);
                Debug.Log("Item is not valid for giving.");
            }
        }
        else
        {
            giveItemButton.SetActive(false);
            Debug.Log("No item found in PickedItemPosition.");
        }
    }

}
