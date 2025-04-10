namespace Universal
{
    using Controllers;
    using Managers;
    using VContainer;
    using VContainer.Unity;
    
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ProductionManager>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<UIManager>();
            builder.RegisterComponentInHierarchy<PlayerMovementController>();
            builder.RegisterComponentInHierarchy<FactoriesController>();
        }
    }
}