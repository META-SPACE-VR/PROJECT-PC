using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator anim;

    /*
    public Transform player;
    public Transform door;

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, door.position);

        if(distance <= 10)
        {
            anim.SetBool("character_nearby", true);
        }
        else
        {
            anim.SetBool("character_nearby", false);
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", false);
        }
    }
}
