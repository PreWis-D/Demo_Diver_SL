using UnityEngine;

public abstract class AbstractBullet : MonoBehaviour
{
    private float _timeToDie = 5f;
    private float _currentTime;

    protected virtual void OnEnable()
    {
        _currentTime = _timeToDie;
    }

    protected virtual void Update()
    {
        _currentTime -= Time.deltaTime;

        if (_currentTime < 0)
            PoolManager.SetPool(this);
    }
}