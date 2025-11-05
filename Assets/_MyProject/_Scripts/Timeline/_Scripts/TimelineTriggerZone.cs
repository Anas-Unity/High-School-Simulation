using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelineTriggerZone : MonoBehaviour
{
    public enum TriggerType
    {
        Once,
        Everytime,
    }


    [Tooltip("This is the GO which will trigger the director to play, i.e PLayer")]
    public GameObject triggringGameObject;
    public PlayableDirector director;
    public TriggerType triggerType;
    public UnityEvent onDirectorPlay;
    public UnityEvent onDirectorFinish;

    public bool m_AlreadyTriggered;

    public void OnTriggerEnter(Collider other)
    {
        print("TimelineTriggerZones function OnTriggerEnter is playing");
        if (other.gameObject != triggringGameObject)
        {
            print("TimelineTriggerZones function OnTriggerEnter first if condition is playing");
            return;
        }
        if (triggerType == TriggerType.Once && m_AlreadyTriggered)
        {
            print("TimelineTriggerZones function OnTriggerEnter second if condition is playing");
            return;
        }

        onDirectorPlay.Invoke();
        director.Play();
        m_AlreadyTriggered = true;
        Invoke(nameof(FinishInvoke), (float)director.duration);
    }

    void FinishInvoke()
    {
        onDirectorFinish.Invoke();
    }
}
