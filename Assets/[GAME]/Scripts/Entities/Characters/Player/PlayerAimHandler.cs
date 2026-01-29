using System.Collections;
using UnityEngine;

public class PlayerAimHandler
{
    private PlayerCharacter _playerCharacter;
    private InputProcessingService _input;
    private Transform _aimTarget;
    private Camera _camera;

    public PlayerAimHandler(PlayerCharacter playerCharacter, Transform aimTarget)
    {
        _playerCharacter = playerCharacter;
        _aimTarget = aimTarget;

        _input = SL.Get<InputProcessingService>();
        _camera = SL.Get<CamerasService>().GetMainCamera();
    }

    public void Update()
    {
        if (_aimTarget == null)
            return;

        Vector3 mousePos = _input.AimPosition;
        mousePos.z = 10 + _playerCharacter.transform.position.z;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(mousePos);

        _aimTarget.transform.position = worldPosition;
    }
}