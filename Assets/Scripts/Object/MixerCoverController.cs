using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MixerCoverController : MonoBehaviour
{
    public bool IsClosed { get; set; }  = false;

    public void ToggleClose()
    {
        IsClosed = !IsClosed;
    }
}
