namespace Zenject
{
    public class TestInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITest>().To<Test2>().AsCached();
        }
    }
}