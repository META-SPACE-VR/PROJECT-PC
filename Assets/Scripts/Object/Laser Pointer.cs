using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [SerializeField]
    GameObject laser;

    [SerializeField]
    float shootDuration;

    [SerializeField]
    AudioSource shootSound;

    public void Shoot() {
        StartCoroutine(ShootFlow());
    }

    IEnumerator ShootFlow() {
        laser.SetActive(true);
        shootSound.Play();

        yield return new WaitForSeconds(shootDuration);

        laser.SetActive(false);
    }
}
