using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearController : MonoBehaviour
{
    public CanvasGroup gameClearGroup;
    public CanvasGroup[] otherGroups;

    public void Clear()
    {
        foreach (CanvasGroup cg in otherGroups)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        gameClearGroup.interactable = true;
        gameClearGroup.blocksRaycasts = true;
        gameClearGroup.alpha = 1.0f;
    }
}
