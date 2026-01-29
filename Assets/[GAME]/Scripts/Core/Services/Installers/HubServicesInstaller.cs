public class HubServicesInstaller
{
    public HubServicesInstaller(HubGameComponents gameComponents)
    {
        SL.Register(new EventProcessingService());
        SL.Register(new SceneLoadService(SceneType.Hub));
        SL.Register(gameComponents);
        SL.Register(new CharactersService( gameComponents.CharacterConfigs));
        SL.Register(new InputProcessingService(gameComponents.InputConfig));
        SL.Register(new CamerasService(gameComponents.MainCamera));
        SL.Register(new ResourcesService());
        SL.Register(new GeneralComponentsService(gameComponents.GeneralSettingsConfig, gameComponents.PrefabsConfig, gameComponents.HandItemsConfig));
        SL.Register(new SpritesService(gameComponents.SpritesConfig));
        SL.Register(new FactoriesService());

        SL.Register(new GameStateService());
    }
}