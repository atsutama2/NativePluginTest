using VContainer.Unity;

public class GamePresenter :
    IInitializable,
    IPostInitializable,
    IFixedTickable,
    IPostFixedTickable,
    ITickable,
    IPostTickable,
    ILateTickable,
    IPostLateTickable
{
    private readonly HelloWorldService hellowWorldService;

    public GamePresenter(HelloWorldService _helloWOrldService)
    {
        hellowWorldService = _helloWOrldService;
    }


    // Start()直前に呼ばれる.
    public void Initialize()
    {
        hellowWorldService.HelloWorld("Initialize");
    }


    // Start()直後に呼ばれる.
    public void PostInitialize()
    {
        hellowWorldService.HelloWorld("Postinitialize");
    }


    // FixedUpdate()直前に呼ばれる.
    public void FixedTick()
    {
        hellowWorldService.HelloWorld("FixedTick");
    }


    // FixedUpdate()直後に呼ばれる.
    public void PostFixedTick()
    {
        hellowWorldService.HelloWorld("PostFixedTick");
    }


    // Update()直前に呼ばれる.
    public void Tick()
    {
        hellowWorldService.HelloWorld("Tick");
    }


    // Update()直後に呼ばれる.
    public void PostTick()
    {
        hellowWorldService.HelloWorld("PostTick");
    }


    // LateUpdate()直前に呼ばれる.
    public void LateTick()
    {
        hellowWorldService.HelloWorld("LateTick");
    }


    // LateUpdate()直後に呼ばれる.
    public void PostLateTick()
    {
        hellowWorldService.HelloWorld("PostLateTick");
    }

}