using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class CamerasService : IService
{
    private CinemachineVirtualCamera _playerCamera;
    private Camera _mainCamera;

    public CamerasService(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    public void Initialize()
    {
        
    }

    public void Cleanup()
    {
        
    }

    public void SetPlayer(PlayerCharacter playerCharacter, CinemachineVirtualCamera playerCamera)
    {
        _playerCamera = playerCamera;
        _playerCamera.Follow = playerCharacter.transform;
    }

    public Camera GetMainCamera()
    {
        return _mainCamera;
    }
}