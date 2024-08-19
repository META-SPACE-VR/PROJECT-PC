using UnityEngine;
using TMPro;
using Fusion;

public class TriggerArea : MonoBehaviour
{
    private Camera mainCamera;
    private Player playerController; // Reference to the player controller

    public Transform screenViewTransform;
    public Transform originalViewTransform; // Reference to the original view transform
    public ScreenUIManager screenUIManager; // Reference to the screen UI manager
    public GameObject interactionPrompt; // Reference to the interaction prompt UI

    private bool isPlayerInRange = false;
    private bool isInteracting = false;

    void Start()
    {
        // Validate required references
        if (screenViewTransform == null) Debug.LogError("screenViewTransform is not assigned.");
        if (originalViewTransform == null) Debug.LogError("originalViewTransform is not assigned.");
        if (screenUIManager == null) Debug.LogError("screenUIManager is not assigned.");
        if (interactionPrompt == null) Debug.LogError("interactionPrompt is not assigned.");

        interactionPrompt.SetActive(false); // Hide the interaction prompt by default
    }

    void Update()
    {
        if (mainCamera == null || screenViewTransform == null || originalViewTransform == null || playerController == null || screenUIManager == null || interactionPrompt == null)
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
                mainCamera = playerController.GetComponentInChildren<Camera>(); // Get the camera from the player

                originalViewTransform = playerController.transform.Find("Camera Offset");

                if (originalViewTransform == null)
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
        if (mainCamera != null)
        {
            // Move the camera to the interaction position
            mainCamera.transform.position = screenViewTransform.position;
            mainCamera.transform.rotation = screenViewTransform.rotation;

            LockCameraControl();
            screenUIManager.ShowScreenUI(); // Show the screen UI
            interactionPrompt.SetActive(false); // Hide the interaction prompt
            isInteracting = true;
        }
    }

    public void ExitInteraction()
    {
        if (mainCamera != null)
        {
            // Move the camera back to the original position
            mainCamera.transform.position = originalViewTransform.position;
            mainCamera.transform.rotation = originalViewTransform.rotation;

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
            playerController.SetInputEnabled(false); // Disable player input
            Cursor.lockState = CursorLockMode.None;  // Unlock the cursor for UI interaction
            Cursor.visible = true; 
        }
    }

    private void UnlockCameraControl()
    {
        if (playerController != null)
        {
            // Re-enable player movement or camera control here
            playerController.SetInputEnabled(true);  // Enable player input
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
    public Player GetInteractingPlayer() {
        return isInteracting ? playerController : null;
    }
}
