using UnityEngine;

public class HelloWorldService
{
    public void HelloWorld(string text)
    {
        Debug.Log(string.Format("{0} : {1}", text, "HelloWorld"));
    }
}
