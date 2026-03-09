using NaughtyAttributes;
using System;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public event Action OnStartGeneration;
    public event Action OnEndGeneration;

    protected void DispatchOnStartGenerationEvent()
    {
        OnStartGeneration?.Invoke();
    }

    protected void DispatchOnEndGenerationEvent()
    {
        OnEndGeneration?.Invoke();
    }

 }
