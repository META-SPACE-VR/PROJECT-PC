
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EngineDoorController : MonoBehaviour
{
    public Animator anim;
    public LabScreenController engineScreen;

    private void Update()
    {
        if (engineScreen.isSucceeded)
        {
            anim.SetBool("character_nearby", true);
        }
        else
        {
            anim.SetBool("character_nearby", false);
        }
    }
}
