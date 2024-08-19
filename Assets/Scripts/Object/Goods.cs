using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Goods : NetworkBehaviour
{
    GameObject playerInRange = null;
    [Networked] bool hasFood { get; set; } = true;

    [SerializeField]
    GameObject spaceFood; // 우주식량

    [SerializeField]
    GameObject interactionPrompt; // Interaction prompt

    [SerializeField]
    KeyCode interactionKey;

    void Awake() {
        interactionPrompt.SetActive(false);
    }

    private void Update() {
        if(playerInRange && Input.GetKeyDown(interactionKey)) {
            SpawnFood();
        }
    }

    private void SpawnFood() {
        // 우주식량 스폰 
        spaceFood.SetActive(true);
        Rigidbody foodRigid = spaceFood.GetComponent<Rigidbody>();

        // 식량이 날아갈 방향 계산
        Vector3 direction = playerInRange.transform.position - transform.position;
        direction -= Vector3.up * direction.y;
        direction.Normalize();
        
        float xzPower = 2.5f;
        float yPower = 4f;
        Vector3 force = xzPower * direction + Vector3.up * yPower;
        foodRigid.AddForce(force, ForceMode.Impulse);

        playerInRange = null;
        interactionPrompt.SetActive(false);
        hasFood = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && hasFood) {
            Player player = other.GetComponent<Player>();

            if(player && player.HasInputAuthority) {
                playerInRange = player.gameObject;
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player") && hasFood && other.gameObject == playerInRange) {
            playerInRange = null;
            interactionPrompt.SetActive(false);
        }
    }
}
