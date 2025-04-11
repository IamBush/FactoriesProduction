namespace Universal
{
    using Controllers;
    using Managers;
    using Objects;
    using VContainer;
    using VContainer.Unity;
    
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<AssetLoadController>(Lifetime.Singleton);
            builder.Register<ProductionManager>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<UIManager>();
            builder.RegisterComponentInHierarchy<PlayerMovementController>();
            builder.RegisterComponentInHierarchy<FactoriesController>();
            builder.RegisterComponentInHierarchy<SoundManager>();
            builder.RegisterComponentInHierarchy<SceneController>();
        }
    }
}