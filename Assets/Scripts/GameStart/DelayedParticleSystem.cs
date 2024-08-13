using System.Collections;
using UnityEngine;

public class DelayedParticleSystem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem;

    [SerializeField]
    private float delayTime = 4f; // 지연 시간 (초)

    void Start()
    {
        if (particleSystem != null)
        {
            StartCoroutine(StartParticleSystemAfterDelay());
        }
    }

    IEnumerator StartParticleSystemAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        // particleSystem.SetActive(true);
        particleSystem.Play();
    }
}
