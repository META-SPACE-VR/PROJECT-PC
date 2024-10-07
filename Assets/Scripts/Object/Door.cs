using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Door : NetworkBehaviour
{
    [Networked] private bool IsClosed { get; set; } = true;
    public Animator anim;
    
    public override void FixedUpdateNetwork()
    {
        // 네트워크에서 동기화된 문 상태에 따라 애니메이션 업데이트
        anim.SetBool("character_nearby", !IsClosed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", true);
            IsClosed = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", false);
            IsClosed = true;
        }
    }
}
