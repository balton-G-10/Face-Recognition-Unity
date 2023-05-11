using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class UDPSend : MonoBehaviour
{

    Thread receiveThread;
    UdpClient client;
    public int port = 8001;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string data;
    public TMP_InputField inputField;
    public void Start()
    {
        client = new UdpClient();
    }


    // receive thread
    public void SendData(string data)
    {
        try
        {
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            byte[] dataByte = Encoding.UTF8.GetBytes(data);
            client.Send(dataByte, dataByte.Length, anyIP);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
        if (printToConsole)
        {
            Debug.Log(data);
        }
    }
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     SendData("1:Atharva");
        // }
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     SendData("0");
        // }
    }
    public void Register()
    {
        if (inputField.text=="")
        {
            Debug.LogError("Please Enter the Name");
            return;
        }
        SendData("1:"+inputField.text);
    }
}
