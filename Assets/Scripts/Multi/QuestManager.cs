using System.Collections.Generic;
using UnityEngine;
using Fusion;
using FusionExamples.Utility;

public class QuestManager : NetworkBehaviour
{
    [Networked] private NetworkBool isBiochemistCleared { get; set; } //로켓연료 조합방법 획득
    [Networked] private NetworkBool isMechanicCleared { get; set; } //보조배터리 회수
    [Networked] private NetworkBool isElectricianCleared { get; set; } //계단에 쌓인 파편 파괴
    [Networked] private NetworkBool isDoctorCleared { get; set; } //환자 치료 
    [Networked] private NetworkBool isGameCleared { get; set; } 

    public static QuestManager Instance => Singleton<QuestManager>.Instance;

    public void CompleteQuest(string job)
    {
        Rpc_CompleteQuest(job);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_CompleteQuest(string job)
    {
        switch (job) {
            case "Biochemist":
                isBiochemistCleared = true;
                break;
            case "Mechanic":
                isMechanicCleared = true; 
                break;
            case "Electrician":
                isElectricianCleared = true;
                break;
            case "Doctor":
                isDoctorCleared = true; 
                break;
        } 
        GameClearCheck();
    }

    private bool GameClearCheck()
    {
        bool isGameClearBool 
            = isBiochemistCleared && isMechanicCleared && isElectricianCleared && isDoctorCleared; 
        return isGameClearBool; 
    }
}
