using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private TestObject testObject;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<HelloWorldService>(Lifetime.Scoped);
        builder.RegisterEntryPoint<GamePresenter>(Lifetime.Singleton);

        builder.RegisterComponent(testObject);
    }
}
