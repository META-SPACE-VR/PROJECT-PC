using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Door : NetworkBehaviour
{
    [Networked] private bool isClosed { get; set; } = true;
    public Animator anim;
    
    public override void FixedUpdateNetwork()
    {
        // 네트워크에서 동기화된 문 상태에 따라 애니메이션 업데이트
        anim.SetBool("character_nearby", !isClosed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", true);
            isClosed = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", false);
            isClosed = true;
        }
    }
}
