using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class WebcamReceiver : MonoBehaviour
{
    public int port = 8080;
    public RawImage displayImage;

    private Texture2D receivedTexture;
    private Socket clientSocket;
    public List<byte[]> recivedBuffer = new List<byte[]>();

    void Start()
    {
        // Connect to the Python server
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect("localhost", port);
        receivedTexture = new Texture2D(1, 1); // Placeholder texture

    }

    void ReceiveVideoFrames()
    {

        byte[] buffer = new byte[1024];
        // Receive bytes into buffer
        int bytesRead = clientSocket.Receive(buffer);
        if (bytesRead <= 0)
        {
            // If no bytes read, connection closed
            Debug.Log("Connection closed");
            return;
        }
        print(BitConverter.ToString(buffer));
        int numChunks = BitConverter.ToInt32(buffer, 0);
        print(numChunks);
        for (int i = 0; i < numChunks + 1; i++)
        {
            byte[] buffer2 = new byte[1024];
            // Receive bytes into buffer
            int bytesRead2 = clientSocket.Receive(buffer2);
            print("hello");
            if (bytesRead2 > 0)
            {
                recivedBuffer.Add(buffer2);
            }
        }
        print(recivedBuffer.Count);
        //// Write received bytes to the memory stream
        //receiveBuffer.Write(buffer, 0, bytesRead);

        //// Check if memory stream contains a complete image
        //byte[] imageData = receiveBuffer.ToArray();
        //// Load image from memory stream
        //receivedTexture.LoadImage(imageData);
        //displayImage.texture = receivedTexture;

        //// Clear memory stream for next image
        //receiveBuffer.SetLength(0);


    }

    void Update()
    {
        ReceiveVideoFrames();

    }




    void OnDestroy()
    {
        // Close the client socket if the object is destroyed
        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Close();
        }
    }
}
