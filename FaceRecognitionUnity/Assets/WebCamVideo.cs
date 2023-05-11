using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class WebCamVideo : MonoBehaviour
{
    private Socket socket;
    private WebCamTexture webcamTexture;
    private List<byte[]> frames;
    private bool isFrameOver;
    void Start()
    {
        if (webcamTexture == null)
        {
            // Start the webcam
            webcamTexture = new WebCamTexture();
        }
        GetComponent<Renderer>().material.mainTexture = webcamTexture;
        if (!webcamTexture.isPlaying)
        {
            webcamTexture.Play();
        }
        
    }
    public IEnumerator startRecording()
    {
        int frame = 0;
        while (frame <500)
        {
            // Capture the video frame from the webcam and convert it to a byte array
            Texture2D frameTexture = GetWebcamFrame();
            byte[] frameBytes = frameTexture.EncodeToJPG();
            frames.Add(frameBytes);
            Debug.Log(frameBytes.Length);
            frame++;
        }
        yield return null;
        isFrameOver = true;
    }
    void Update()
    {

    }

    Texture2D GetWebcamFrame()
    {
        // Get the current webcam frame and create a new Texture2D to hold it
        Texture2D frameTexture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        frameTexture.SetPixels(webcamTexture.GetPixels());
        frameTexture.Apply();

        return frameTexture;
    }

    void OnDestroy()
    {
        // Cleanup
        socket.Close();
    }
}
