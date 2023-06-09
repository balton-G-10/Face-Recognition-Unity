﻿using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverOneway
{
    private readonly Thread receiveThread;
    private bool running;

    public ReceiverOneway()
    {
        receiveThread = new Thread((object callback) =>
        {
            using (var socket = new PullSocket())
            {
                socket.Connect("tcp://localhost:5555");

                while (running)
                {
                    string message = socket.ReceiveFrameString();
                    Data data = JsonUtility.FromJson<Data>(message);
                    ((Action<Data>)callback)(data);
                }
            }
        });
    }

    public void Start(Action<Data> callback)
    {
        running = true;
        receiveThread.Start(callback);
    }

    public void Stop()
    {
        running = false;
        receiveThread.Join();
    }
}

public class ClientOneway : MonoBehaviour
{
    private readonly ConcurrentQueue<Action> runOnMainThread = new ConcurrentQueue<Action>();
    private ReceiverOneway receiver;
    private Texture2D tex;
    public RawImage image;

    public void Start()
    {
        
    }
    public void clientStart()
    {
        tex = new Texture2D(2, 2, TextureFormat.RGB24, mipChain: false);
        image.texture = tex;

        ForceDotNet.Force();
        receiver = new ReceiverOneway();
        receiver.Start((Data d) => runOnMainThread.Enqueue(() =>
            {
                Debug.Log(d.image);
                tex.LoadImage(d.image);
            }
        ));
    }

    public void Update()
    {
        if (!runOnMainThread.IsEmpty)
        {
            Action action;
            while (runOnMainThread.TryDequeue(out action))
            {
                action.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        receiver.Stop();
        NetMQConfig.Cleanup();
    }
}

[Serializable]
public struct Data
{
    public bool bool_;
    public int int_;
    public string str;
    public byte[] image;
}