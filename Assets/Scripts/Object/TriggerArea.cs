using UnityEngine;
using TMPro;
using Fusion;

public class TriggerArea : NetworkBehaviour
{
    private Transform attachPoint;
    private Transform camTarget;
    [Networked] private Player playerController { get; set; } // Reference to the player controller

    public Transform screenViewTransform;
    public ScreenUIManager screenUIManager; // Reference to the screen UI manager
    public GameObject interactionPrompt; // Reference to the interaction prompt UI

    [Networked] private NetworkBool isPlayerInRange { get; set; } = false;
    [Networked] public NetworkBool isInteracting { get; set; } = false;

    private Player colliderFlag { get; set; } = null;

    public override void Spawned()
    {
        // Validate required references
        if (screenViewTransform == null) Debug.LogError("screenViewTransform is not assigned.");
        if (attachPoint == null) Debug.LogError("originalViewTransform is not assigned.");
        if (screenUIManager == null) Debug.LogError("screenUIManager is not assigned.");
        
        if (interactionPrompt == null) Debug.LogError("interactionPrompt is not assigned.");
        else interactionPrompt.SetActive(false); // Hide the interaction prompt by default
    }

    public override void FixedUpdateNetwork()
    {
        // if (camTarget == null || screenViewTransform == null || attachPoint == null || playerController == null || screenUIManager == null || interactionPrompt == null)
        // {
        //     return;
        // }

        Debug.Log(colliderFlag);

        // The interaction logic is now handled by the Player's HandleTriggerInteraction method.
        if(colliderFlag) {
            Debug.Log("Why 1+1");
            if(!isInteracting && playerController != colliderFlag) {
                Debug.Log("Why 1+1");
                // Attempt to get the Player component from the object
                playerController = colliderFlag;

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

            colliderFlag = null;
        }

        Runner.TryGetPlayerObject(Runner.LocalPlayer, out NetworkObject localObj);
        if(playerController && localObj && playerController != localObj.GetComponent<Player>()) {
            isPlayerInRange = false;
            interactionPrompt.SetActive(false); // Hide the interaction prompt
            playerController.ClearCurrentTriggerArea();  // Inform the player that they are out of range
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Check if the player entered the trigger area
        if (other.CompareTag("Player"))
        {
            colliderFlag = other.GetComponent<Player>();
        }
    }

    public void EnterInteraction()
    {
        if (camTarget != null)
        {
            Debug.Log("Why Enter");
            // Move the camera to the interaction position
            camTarget.transform.position = screenViewTransform.position;
            camTarget.transform.rotation = screenViewTransform.rotation;

            LockCameraControl();
            screenUIManager.ShowScreenUI(); // Show the screen UI
            interactionPrompt.SetActive(false); // Hide the interaction prompt
            isInteracting = true;
        }
    }

    public void ExitInteraction()
    {
        if (camTarget != null)
        {
            Debug.Log("Why fucking exit");
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
