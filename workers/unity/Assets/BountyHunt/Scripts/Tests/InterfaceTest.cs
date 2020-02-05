using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITestA
{
    void GetBlyat();
}

public interface ITestB
{
    void GetBlyat2();
}

public interface ITestC : ITestB
{
    void GetBlyat3();
}

public class TestA : ITestA
{
    public void GetBlyat()
    {
        Debug.Log("GetBlyat Called");
    }
}

public class TestC : ITestA, ITestB
{
    public void GetBlyat3()
    {
        Debug.Log("GetBlyat3 Called");
    }

    public void GetBlyat2()
    {
        Debug.Log("GetBlyat2 Called");
    }

    public void GetBlyat()
    {
        Debug.Log("GetBlyat1 Called");
    }
}


public static class InterfaceTestClass{
    public static void CallInterface(object o)
    {
        switch (o)
        {
            case ITestC c:
                c.GetBlyat3();
                c.GetBlyat2();
                break;
            case ITestA a:
                break;
            case ITestB b:
                b.GetBlyat2();
                break;
            default:
                break;

        }
    }
}

