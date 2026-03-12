using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public abstract class Generator : MonoBehaviour
{
    public event Action OnStartGeneration;
    public event Action OnEndGeneration;
    public enum SplitType
    {
        Instant, Overtime, Space
    }

    public SplitType splitType;
    public float splitDelay = 0.05f;
    public Color[] colors = { Color.green, Color.red, Color.cyan, Color.black, new Color(255, 175, 0, 1), Color.blue };

    public AudioSource audioSource;

    //[Space(100)]

    protected void DispatchOnStartGenerationEvent()
    {
        OnStartGeneration?.Invoke();
    }

    protected void DispatchOnEndGenerationEvent()
    {
        OnEndGeneration?.Invoke();
    }

    public IEnumerator CustomWait(SplitType splitType, float splitDelay)
    {
        switch (splitType)
        {
            case SplitType.Overtime:
                yield return new WaitForSeconds(splitDelay);
                break;
            case SplitType.Space:
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                yield return null;
                break;
        }
    }
}
