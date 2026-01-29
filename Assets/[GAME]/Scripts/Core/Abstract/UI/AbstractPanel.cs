using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public abstract class AbstractPanel : MonoBehaviour
{
    [SerializeField] protected RectTransform Canvas;

    public abstract PanelType Type { get; }

    public bool IsShown { get; protected set; } = true;

    public virtual void Init() { }
    public virtual void Init(object[] arr) { }

    public async void Show(Action callback = null)
    {
        if (!IsShown)
        {
            IsShown = true;

            Canvas.gameObject.SetActive(true);
            await OnShow();

            callback?.Invoke();
        }
    }

    public async void Hide(Action callback = null)
    {
        if (IsShown)
        {
            IsShown = false;

            await OnHide();
            Canvas.gameObject.SetActive(false);

            callback?.Invoke();
        }
    }

    public virtual async UniTask OnShow() { }
    public virtual async UniTask OnHide() { }
}
