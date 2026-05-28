using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BabyStateContext
{
    public BabyStateController Controller;
    public XRSocketInteractor DiaperSocket;
    public XRSocketInteractor MouthSocket;

    public float IdleToNeedDelay = 15f;
    public float FeedDuration = 5f;
    public float SpitForce = 2f;
}
