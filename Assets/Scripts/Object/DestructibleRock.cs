using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DestructibleRock : MonoBehaviour
{
    // 활성화 여부
    bool isAlive;

    [SerializeField]
    List<GameObject> cells; // 조각들

    [SerializeField]
    float explosionForce = 5; // 폭발 힘

    [SerializeField]
    float explosionRadius = 5; // 폭발 반경

    [SerializeField]
    float explosionUpward = 5; 

    [SerializeField]
    AudioSource crumbleSound;

    [SerializeField]
    float cellDestroyTime = 3f;

    void Start() {
        // 원래 돌은 활성화
        isAlive = true;

        // 조각들은 비활성화
        foreach(GameObject cell in cells) {
            cell.SetActive(false);
        }
    }

    // 폭발
    public void Explode() {
        Debug.Log("Explode");

        crumbleSound.Play();

        // 조각들은 활성화
        foreach(GameObject cell in cells) {
            cell.SetActive(true);
            Destroy(cell, cellDestroyTime);
            Destroy(gameObject.transform.parent.gameObject, cellDestroyTime);
        }

        // 조각들 폭발
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        Debug.Log(colliders.Length);

        foreach(Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null) {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

        isAlive = false;

        gameObject.SetActive(false);
    }
}