using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AestroidBlow : MonoBehaviour
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

    private void Start()
    {
        Debug.Log("AestroidBlow Start method called"); // 디버그 로그 추가

        // 원래 돌은 활성화
        gameObject.SetActive(true);

        // 조각들은 비활성화
        foreach (GameObject cell in cells)
        {
            cell.SetActive(false);
        }
    }

    // 폭발
    public void Explode()
    {
        Debug.Log("Explode method called"); // 디버그 로그 추가

        if (crumbleSound != null)
        {
            crumbleSound.Play();
        }

        // 원래 돌 비활성화
        gameObject.SetActive(false);

        // 조각들은 활성화
        foreach (GameObject cell in cells)
        {
            Debug.Log("Activating cell: " + cell.name);
            cell.SetActive(true);

            // 조각들 폭발
            Rigidbody rb = cell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Applying explosion force to cell: " + cell.name);
                rb.isKinematic = false; // Rigidbody가 Kinematic 모드가 아닌지 확인
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

        Debug.Log("Explosion complete.");
    }
}
