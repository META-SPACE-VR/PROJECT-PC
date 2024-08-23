using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MixerCoverController : NetworkBehaviour
{
    [Networked] public bool isClosed { get; set; }  = false;

    public void ToggleClose()
    {
        isClosed = !isClosed;
    }
}
