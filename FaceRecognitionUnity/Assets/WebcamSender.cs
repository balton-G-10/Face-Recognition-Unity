using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using System;
using System.Text;

public class WebcamSender : MonoBehaviour
{
    // UDP settings
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    public RawImage rawImage;
    // Webcam settings
    public int webcamWidth = 640;
    public int webcamHeight = 480;
    public int resizedWidth = 320; // Desired reduced resolution width
    public int resizedHeight = 240; // Desired reduced resolution height
    public int i = 0;
    WebCamTexture webcam;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize UDP client
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000); // or new IPEndPoint(IPAddress.Loopback, 8000);

        // Start webcam
        WebCamDevice[] devices = WebCamTexture.devices;
        webcam = new WebCamTexture(devices[0].name, webcamWidth, webcamHeight);
        webcam.Play();
        string hello = "Hello from Unity!!!";
        byte[] startingBytes = Encoding.UTF8.GetBytes(hello);
        udpClient.Send(startingBytes, startingBytes.Length, remoteEndPoint);

    }

    // Update is called once per frame
    void Update()
    {
        if (i < 31)
        {
            // Get webcam texture
            Texture2D webcamTexture = new Texture2D(webcamWidth, webcamHeight, TextureFormat.RGBA32, false);
            webcamTexture.SetPixels32(webcam.GetPixels32());
            webcamTexture.Apply();

            // Resize webcam texture
            Texture2D resizedTexture = new Texture2D(resizedWidth, resizedHeight);
            rawImage.texture = resizedTexture;
            Graphics.ConvertTexture(webcamTexture, resizedTexture);
            byte[] resizedData = resizedTexture.EncodeToPNG();
            // Send resized data over UDP
            udpClient.Send(resizedData, resizedData.Length, remoteEndPoint);
            i = 32;
        }
    }

    // OnDestroy is called when the script is being destroyed
    void OnDestroy()
    {
        // Close UDP client
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }
    }
}
