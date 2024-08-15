using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleRock : MonoBehaviour
{
    [SerializeField]
    List<GameObject> cells; // 조각들

    [SerializeField]
    float explosionForce = 5;

    [SerializeField]
    float explosionRadius = 5;

    [SerializeField]
    float explosionUpward = 5;

    [SerializeField]
    AudioSource crumbleSound;

    [SerializeField]
    float cellDestroyTime = 3f;

    private void Start() {
        // 원래 돌은 활성화
        gameObject.SetActive(true);

        // 조각들은 비활성화
        foreach(GameObject cell in cells) {
            cell.SetActive(false);
        }
    }

    // 폭발
    public void Explode() {
        Debug.Log("Explode");

        crumbleSound.Play();

        // 원래 돌 비활성화
        gameObject.SetActive(false);

        // 조각들은 활성화
        foreach(GameObject cell in cells) {
            cell.SetActive(true);
            Destroy(cell, cellDestroyTime);
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
    }
}
