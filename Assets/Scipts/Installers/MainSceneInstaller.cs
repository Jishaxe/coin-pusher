using UnityEngine.Serialization;
using UnityEngine.tvOS;
using Zenject;

public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
{
    public BoardController.Settings BoardSettings;
    public CoinSpawnController.Settings CoinSpawnerSettings;
    public PusherController.Settings PusherSettings;
    public RemoteController.Settings RemoteSettings;
    public LightbarEffectController.Settings LightbarEffectSettings;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<MainSceneController>().AsSingle();
        
        Container.BindInstance(CoinSpawnerSettings);
        Container.BindInterfacesAndSelfTo<CoinSpawnController>().AsSingle();
        Container.BindFactory<Coin, Coin, Coin.Factory>().FromFactory<CoinFactory>();

        Container.BindInstance(PusherSettings);
        Container.BindInterfacesAndSelfTo<PusherController>().AsSingle();

        Container.BindInstance(BoardSettings);
        Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();

        Container.BindInterfacesAndSelfTo<SaveGameService>().AsSingle();

        Container.BindInstance(RemoteSettings);
        Container.BindInterfacesAndSelfTo<RemoteController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.BindFactory<RawCommand, BaseCommand, BaseCommand.Factory>().FromFactory<CommandFactory>();
        Container.BindInterfacesAndSelfTo<CommandController>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<SecretService>().AsSingle();
        
        Container.BindInstance(LightbarEffectSettings);
        Container.BindInterfacesAndSelfTo<LightbarEffectController>().AsSingle();
    }
}
