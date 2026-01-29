using System;
using UnityEngine;

public abstract class AbstractHandItem : MonoBehaviour
{
    [SerializeField] protected Transform View;

    protected InputProcessingService Input;

    public bool IsOpened { get; private set; } = true;
    public bool Enabled { get; private set; } = true;
    public HandItemData Data { get; private set; }
    public abstract HandItemType Type { get; }

    public virtual void Init(HandItemData itemData)
    {
        Data = itemData;
        Input = SL.Get<InputProcessingService>();
    }

    public void ChangeEnabledState(bool isEnable)
    {
        Enabled = isEnable;
    }

    public bool HasVisible()
    {
        return View.gameObject.activeSelf;
    }

    public abstract bool CanInteract();
    protected abstract void HandleVisible();

    protected virtual void Update()
    {
        if (Enabled == false)
        {
            if (View.gameObject.activeSelf)
                View.gameObject.SetActive(false);

            return;
        }

        HandleVisible();
    }
}