using System;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public event Action StartGenerating;
    public event Action EndGenerating;

    protected void StartGenerator()
    {
        StartGenerating?.Invoke();
    }

    protected void StopGenerating()
    {
        EndGenerating?.Invoke();
    }
}
