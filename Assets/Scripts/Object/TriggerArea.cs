using UnityEngine;
using TMPro;
using Fusion;
using System.Collections;

public class TriggerArea : MonoBehaviour
{
    private Transform attachPoint;
    private Transform camTarget;
    private Player playerController; // Reference to the player controller

    public Transform screenViewTransform;
    public ScreenUIManager screenUIManager; // Reference to the screen UI manager
    public GameObject interactionPrompt; // Reference to the interaction prompt UI

    private bool isPlayerInRange = false;
    private bool isInteracting = false;
    private bool canExitInteraction = true;  // Interaction을 종료할 수 있는지 여부를 제어


    void Start()
    {
        // Validate required references
        if (screenViewTransform == null) Debug.LogError("screenViewTransform is not assigned.");
        if (attachPoint == null) Debug.LogError("originalViewTransform is not assigned.");
        if (screenUIManager == null) Debug.LogError("screenUIManager is not assigned.");
        if (interactionPrompt == null) Debug.LogError("interactionPrompt is not assigned.");

        interactionPrompt.SetActive(false); // Hide the interaction prompt by default
    }

    void Update()
    {
        if (camTarget == null || screenViewTransform == null || attachPoint == null || playerController == null || screenUIManager == null || interactionPrompt == null)
        {
            return;
        }

        // The interaction logic is now handled by the Player's HandleTriggerInteraction method.
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger area
        if (other.CompareTag("Player"))
        {
            // Attempt to get the Player component from the object
            playerController = other.GetComponent<Player>();

            // Ensure that we only interact with the local player's camera
            if (playerController != null && playerController.HasInputAuthority)
            {
                attachPoint = playerController.transform.Find("Attach Point"); // Get the camera from the player
                camTarget = playerController.transform.Find("CamTarget"); // Get the camera from the player

                if (attachPoint == null)
                {
                    Debug.LogError("OriginalViewTransform (Camera Offset) not found in player hierarchy.");
                }

                interactionPrompt.SetActive(true); // Show the interaction prompt
                isPlayerInRange = true;
                playerController.SetCurrentTriggerArea(this);  // Inform the player that they are in range

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the player exited the trigger area
        if (other.CompareTag("Player") && playerController != null && other.gameObject == playerController.gameObject)
        {
            isPlayerInRange = false;
            interactionPrompt.SetActive(false); // Hide the interaction prompt
            playerController.ClearCurrentTriggerArea();  // Inform the player that they are out of range
        }
    }

    public void EnterInteraction()
    {
        if (camTarget != null)
        {
            // Move the camera to the interaction position
            camTarget.transform.position = screenViewTransform.position;
            camTarget.transform.rotation = screenViewTransform.rotation;

            LockCameraControl();
            screenUIManager.ShowScreenUI(); // Show the screen UI
            interactionPrompt.SetActive(false); // Hide the interaction prompt
            isInteracting = true;

            canExitInteraction = false;
            StartCoroutine(EnableExitInteractionAfterDelay(1f));

        }
    }

    public void ExitInteraction()
    {
        if (camTarget != null && canExitInteraction)  // ExitInteraction이 가능해야만 호출
        {
            // Move the camera back to the original position
            camTarget.transform.position = attachPoint.position;
            camTarget.transform.rotation = attachPoint.rotation;

            UnlockCameraControl();
            screenUIManager.HideScreenUI(); // Hide the screen UI
            interactionPrompt.SetActive(false); // Hide the interaction prompt
            isInteracting = false;
        }
    }

    private void LockCameraControl()
    {
        if (playerController != null)
        {
            Cursor.lockState = CursorLockMode.None;  // Unlock the cursor for UI interaction
            Cursor.visible = true;
        }
    }

    private void UnlockCameraControl()
    {
        if (playerController != null)
        {
            // Re-enable player movement or camera control here
            Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor back to gameplay mode
            Cursor.visible = false;
        }
        // You could re-lock the cursor if necessary
    }

    // This method returns whether the player is currently interacting
    public bool IsInteracting()
    {
        return isInteracting;
    }

    // give player who interacting
    public Player GetInteractingPlayer()
    {
        return isInteracting ? playerController : null;
    }
    private IEnumerator EnableExitInteractionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canExitInteraction = true;  // 1초 후에 상호작용 종료 가능
    }
}
