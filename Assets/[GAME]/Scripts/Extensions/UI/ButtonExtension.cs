using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonExtension : MonoBehaviour
{
    private Button _button;
    private Action _action;

    public bool Interctable
    {
        get => _button.interactable;
        set => _button.interactable = value;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void Init(Action action)
    {
        _action = action;
    }

    private void OnButtonClicked()
    {
        if (_action == null)
            Debug.LogWarning($"Button not initialized!", transform);

        _action();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }
}
