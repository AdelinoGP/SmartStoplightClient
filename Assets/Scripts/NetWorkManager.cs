using NativeWebSocket;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct CameraPacket
{
    public bool isLeftSide;
    public string base64Image;
}

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private int msTillUpdate = 10000;
    [SerializeField] private string address = "ws://localhost:8001";

    [SerializeField] private CrossroadController crossroadController;

    private static NetworkManager instance;
    private static WebSocket websocket;
    private float keepAlive = 0;
    private float updateTimer = 0;

    private bool sendLeftSide = false;

    public int MsTillUpdate { get => msTillUpdate; }
    private Coroutine printCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        _ = ConnectAsync();
    }

    private void Update()
    {
        if (printCoroutine == null && updateTimer > msTillUpdate)
        {
            printCoroutine = StartCoroutine(PrintAndSend());
            updateTimer = 0;
            return;
        }

        if (keepAlive < Time.time)
        {
            websocket?.Send(Encoding.ASCII.GetBytes("Ping"));
            keepAlive = Time.time + 1;
        }
        updateTimer += Time.deltaTime;
    }
    private void OnDestroy() => websocket?.Close();

    private async Task ConnectAsync()
    {

        websocket = new WebSocket(address);

        websocket.OnOpen += OnConnect;
        websocket.OnError += OnError;
        websocket.OnClose += OnDisconnect;

        websocket.OnMessage += OnMessage;

        // waiting for messages
        await websocket.Connect();
    }

    private void OnDisconnect(WebSocketCloseCode closeCode)
    {
        Debug.Log($"Disconnected to Server");
    }

    private void OnError(string exception)
    {
        Debug.Log($"Error because of Server, Error:{exception}");
    }

    private void OnConnect()
    {
        Debug.Log($"Connected to Server");
        printCoroutine = StartCoroutine(PrintAndSend());
    }

    private void OnMessage(byte[] data)
    {
        Debug.Log($"Data from Server, length:{data.Length}, data:{data}");
    }

    public static void SendCamera(byte[] imageAsBytes)
    {
        string base64ImageRepresentation = Convert.ToBase64String(imageAsBytes) ;
        Debug.Log("Image as string:" + base64ImageRepresentation);
        CameraPacket clientData = new()
        {
            isLeftSide = instance.sendLeftSide,
            base64Image = base64ImageRepresentation,
        };

        var messageBytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(clientData));

        websocket?.Send(messageBytes);
    }

    private IEnumerator PrintAndSend()
    {
        var camera = sendLeftSide ? crossroadController.cameraLeft : crossroadController.cameraRight;
        var renderTexture = RenderTexture.GetTemporary(Screen.width/2 , Screen.height /2);
        camera.targetTexture = renderTexture;

        yield return new WaitForEndOfFrame();

        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAHalf, false);
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;
        camera.Render();

        renderTexture.Release();

        var bytes = texture.EncodeToPNG();

        SendCamera(bytes);
        Destroy(texture);

        instance.printCoroutine = null;
    }
}
