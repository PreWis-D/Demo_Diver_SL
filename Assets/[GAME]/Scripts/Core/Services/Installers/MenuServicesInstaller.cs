public class MenuServicesInstaller
{
    public MenuServicesInstaller(InputConfig inputConfig)
    {
        SL.Register(new SceneLoadService(SceneType.Menu));
        SL.Register(new EventProcessingService());
        SL.Register(new InputProcessingService(inputConfig));
    }
}