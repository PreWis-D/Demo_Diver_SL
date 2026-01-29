using System.Collections;
using UnityEngine;

public class HitFx : MonoBehaviour
{
    private ParticleSystem[] _particleSystems;

    public void Init(ResourceType type)
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();

        var main = _particleSystems[0].main;
        main.stopAction = ParticleSystemStopAction.Callback;

        _particleSystems[0].Play();
    }

    private void OnParticleSystemStopped()
    {
        PoolManager.SetPool(this);
    }
}