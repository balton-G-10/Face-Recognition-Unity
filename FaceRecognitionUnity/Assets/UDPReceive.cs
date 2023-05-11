using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
public class UDPReceive : MonoBehaviour
{
    public ClientOneway clientOneway1;
    public ClientOneway clientOneway2;
    Thread receiveThread;
    UdpClient client;
    public int port = 5052;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string data;
    int i = 0;
    public bool isLogin;
    public List<string> commonNames = new List<string>();
    public GameObject successScreen;
    public TextMeshProUGUI nameText;
    public void Start()
    {
        clientOneway1.enabled = false;
        clientOneway2.enabled = false;
        isLogin = false;
        successScreen.SetActive(false);
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }


    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (startRecieving)
        {

            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);
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
    }
    private void Update()
    {
        if (data != "")
        {
            if (clientOneway1.enabled == false)
            {
                clientOneway1.clientStart();
                clientOneway1.enabled = true;
            }
            CheckData();
        }
    }
    public void CheckData()
    {
        if (!isLogin)
        {
            if (i < 180)
            {
                commonNames.Add(data);
                i++;
            }
            else
            {
                // group the fruits by name and count them
                var groupedNames = commonNames.GroupBy(commonName => commonNames)
                                          .Select(group => new { Name = group.Key, Count = group.Count() })
                                          .OrderByDescending(group => group.Count);

                // print the most repeated fruit and its count
                var mostRepeatedName = groupedNames.First();
                if(mostRepeatedName.Name[0]=="unknown")
                	return;
                nameText.text = "Welcome:" + mostRepeatedName.Name[0];
                successScreen.SetActive(true);
                Debug.Log($"Most repeated fruit: {mostRepeatedName.Name[0]}, Count: {mostRepeatedName.Count}");
                isLogin = true;
            }
        }
    }
    public void Register()
    {
        if (clientOneway2.enabled == false)
        {
            clientOneway2.enabled = true;
            clientOneway2.clientStart();
        }
    }

}
