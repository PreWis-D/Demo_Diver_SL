using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugFirstScenePanel : AbstractPanel
{
    [SerializeField] private TMP_Text _deviceText;

    private InputProcessingService _input;
    private InputDevice _inputDevice;

    public override PanelType Type => PanelType.DebugFirstScene;

    public override void Init()
    {
        base.Init();

        _input = SL.Get<InputProcessingService>();
    }

    public void Update()
    {
        if (_input.InputControl != null)
            _inputDevice = _input.InputControl.device;

        if (_inputDevice != null)
            _deviceText.SetText($"Device: {_inputDevice.name}");
    }
}