
using UnityEngine;

public class FirebaseCustomYield : CustomYieldInstruction
{
    bool requestEnd;

    public void onRequestStarted()
    {
        Debug.LogWarning("onRequestStarted()");
        requestEnd = false;
    }
    public void onRequestEnd()
    {
        Debug.LogWarning("onRequestEnd()");
        requestEnd = true;
    }

    public override bool keepWaiting => !requestEnd;
}
