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

        // Toggle interaction mode on key press
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isInteracting)
            {
                ExitInteraction();
            }
            else
            {
                EnterInteraction();
            }
        }
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
                interactionPrompt.SetActive(true); // Show the interaction prompt
                isPlayerInRange = true;
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
            // You could disable player movement or camera control here if needed
            playerController.SetInputEnabled(false); // Disable player input
        }
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for UI interaction
    }

    private void UnlockCameraControl()
    {
        if (playerController != null)
        {
            // Re-enable player movement or camera control here
            playerController.SetInputEnabled(true); // Enable player input
        }
        // You could re-lock the cursor if necessary
        // Cursor.lockState = CursorLockMode.Locked;
    }

    // This method returns whether the player is currently interacting
    public bool IsInteracting()
    {
        return isInteracting;
    }
}
