using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedResolution : MonoBehaviour
{
    void Awake() {
        Screen.SetResolution(1920, 1080, false);
        Screen.fullScreen = false;
    }
}
