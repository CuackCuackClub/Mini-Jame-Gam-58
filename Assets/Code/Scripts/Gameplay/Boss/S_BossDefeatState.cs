using System;
using UnityEngine;

public class S_BossDefeatState : MonoBehaviour
{
    public bool IsBossDefeated { get; private set; }

    public event Action BossDefeated;

    public void MarkBossDefeated()
    {
        if (IsBossDefeated)
        {
            return;
        }

        IsBossDefeated = true;
        BossDefeated?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Mark Boss Defeated")]
    private void DebugMarkBossDefeated()
    {
        MarkBossDefeated();
    }
#endif
}
