using Services;
using Zenject;

public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
{
    public UIController UIController;
    public LocalizationService.Settings LocalizationSettings;
    public BoardController.Settings BoardSettings;
    public CoinSpawnController.Settings CoinSpawnerSettings;
    public PusherController.Settings PusherSettings;
    public RemoteController.Settings RemoteSettings;
    public LightbarEffectController.Settings LightbarEffectSettings;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CampaignModel>().AsSingle();
        Container.BindInterfacesAndSelfTo<BoardItemsModel>().AsSingle();
        
        Container.BindInstance(UIController);
        Container.BindInterfacesAndSelfTo<MainSceneController>().AsSingle();

        Container.BindInstance(LocalizationSettings);
        Container.BindInterfacesAndSelfTo<LocalizationService>().AsSingle();
        
        Container.BindInstance(CoinSpawnerSettings);
        Container.BindInterfacesAndSelfTo<CoinSpawnController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindFactory<Coin, Coin, Coin.Factory>().FromFactory<CoinFactory>();

        Container.BindInstance(PusherSettings);
        Container.BindInterfacesAndSelfTo<PusherController>().AsSingle();

        Container.BindInstance(BoardSettings);
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();

        Container.BindInterfacesAndSelfTo<SaveGameService>().AsSingle();

        Container.BindInterfacesAndSelfTo<ItemSpawnerProvider>().AsSingle();

        Container.BindInstance(RemoteSettings);
        Container.BindInterfacesAndSelfTo<RemoteController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.BindFactory<RawCommand, BaseCommand, BaseCommand.Factory>().FromFactory<CommandFactory>();
        Container.BindInterfacesAndSelfTo<CommandController>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<SecretService>().AsSingle();
        
        Container.BindInstance(LightbarEffectSettings);
        Container.BindInterfacesAndSelfTo<LightbarEffectController>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<ImageProvisionService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<CoinMarkingService>().AsSingle();
    }
}
