using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class Goods : NetworkBehaviour
{
    [Networked] Player playerInRange { get; set; } = null;
    [Networked] NetworkBool hasFood { get; set; } = true;

    [SerializeField]
    GameObject spaceFood; // 우주식량

    [SerializeField]
    Transform spawnPoint; // 스폰 포인트

    [Networked] NetworkBool triggerSpawn { get; set; } = false;

    [SerializeField]
    GameObject interactionPrompt; // Interaction prompt

    public override void FixedUpdateNetwork() {
        if(triggerSpawn && hasFood) {
            SpawnFood();
        }
    }

    public void TriggerSpawnFood() {
        triggerSpawn = true;
    }

    private void SpawnFood() {
        hasFood = false;

        NetworkObject spawnedFood = Runner.Spawn(spaceFood, spawnPoint.position, Quaternion.identity);
        Collectable foodCollectable = spawnedFood.GetComponent<Collectable>();
        foodCollectable.InventoryManager = InventoryManager.Instance;

        Rigidbody foodRigid = spawnedFood.GetComponent<Rigidbody>();

        // 식량이 날아갈 방향 계산
        Vector3 direction = playerInRange.gameObject.transform.position - transform.position;
        direction -= Vector3.up * direction.y;
        direction.Normalize();
        
        float xzPower = 2.5f;
        float yPower = 4f;
        Vector3 force = xzPower * direction + Vector3.up * yPower;
        foodRigid.AddForce(force, ForceMode.Impulse);

        playerInRange = null;
        interactionPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if(playerInRange != null) return;

        if(other.CompareTag("Player") && hasFood) {
            Player player = other.GetComponent<Player>();

            if(player && player.HasInputAuthority) {
                playerInRange = player;
                player.SetCurrentGoods(this);
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player") && hasFood && other.gameObject == playerInRange.gameObject) {
            playerInRange.SetCurrentGoods(null);
            playerInRange = null;
            interactionPrompt.SetActive(false);
        }
    }
}
