using UnityEngine;

public class MenuEntryPoint : AbstractEntryPoint
{
    [SerializeField] private MenuUIManager _menuUIManager;
    [SerializeField] private InputConfig _inputConfig;

    private void Start()
    {
        SL.Init();
        new MenuServicesInstaller(_inputConfig);

        _menuUIManager.Init();
        SL.Get<InputProcessingService>().EnableInput();
    }

    private void OnDestroy()
    {
        _menuUIManager.Unsubscribe();
    }
}