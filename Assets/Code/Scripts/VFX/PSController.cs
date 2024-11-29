using System.Collections.Generic;
using UnityEngine;

public class PSController : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystems;

    public void Play()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    public void Stop()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }
    }
}
