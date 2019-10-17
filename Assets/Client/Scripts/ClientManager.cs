using System.Collections;
using System.Collections.Generic;
using Assets.Client.Scripts;
using UnityEngine;

public class ClientManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        ClientTCP.instance.Connect();
    }

    private void OnApplicationQuit()
    {
        ClientTCP.instance.client.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
