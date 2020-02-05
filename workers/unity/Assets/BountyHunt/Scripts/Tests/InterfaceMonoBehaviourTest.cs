using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceMonoBehaviourTest : MonoBehaviour, ITestC
{
    // Start is called before the first frame update
    void Start()
    {
        TestA a = new TestA();
        TestC c = new TestC();
        //InterfaceTestClass.CallInterface(a);
        InterfaceTestClass.CallInterface(c);
        //InterfaceTestClass.CallInterface(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetBlyat3()
    {
        Debug.Log("MB GetBlyat3 Called");
    }

    public void GetBlyat2()
    {
        Debug.Log("MB GetBlyat2 Called");
    }
}
