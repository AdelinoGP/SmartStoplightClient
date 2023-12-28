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
public struct StoplightStatusPacket
{
    public string leftStoplightStatus;
    public string rightStoplightStatus;
}

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private float secondsTillUpdate = 0.3f;
    [SerializeField] private string address = "ws://localhost:8001";

    [SerializeField] private CrossroadController crossroadController;

    private static NetworkManager instance;
    private static WebSocket websocket;
    private float keepAlive = 0;
    private float updateTimer = 0;
    private bool connecting = false;

    private bool sendLeftSide = false;
    private int maxRetries = 5;
    public float MsTillUpdate { get => secondsTillUpdate; }
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
        websocket.DispatchMessageQueue();

        if (printCoroutine == null && updateTimer > secondsTillUpdate)
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
    private void OnDestroy()
    {
        websocket?.CancelConnection();
        websocket?.Close();
        websocket = null;
        instance = null;
    }

    private async Task ConnectAsync()
    {
        if (maxRetries < 0 || connecting)
            return;

        maxRetries--;

        if (websocket != null)
        {
            websocket?.CancelConnection();
            websocket?.Close();
            websocket = null;
        }

        connecting = true;
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
        if (instance == null)
            return;
        Debug.Log($"Desconectou do servidor, tentando reconexão");

        connecting = false;
        _ = ConnectAsync();
    }

    private void OnError(string exception)
    {
        if (instance == null)
            return;

        Debug.Log($"Erro na conexão, Erro:{exception}, tentando reconexão");

        connecting = false;
        _ = ConnectAsync();
    }

    private void OnConnect()
    {
        Debug.Log($"Conectou no servidor");
        printCoroutine = StartCoroutine(PrintAndSend());
        connecting = false;
    }

    private void OnMessage(byte[] data)
    {
        string stringData = Encoding.UTF8.GetString(data);
        Debug.Log("Recebeu do servidor: " + stringData);
        StoplightStatusPacket receivedPacket = JsonUtility.FromJson<StoplightStatusPacket>(stringData);

        crossroadController.OnReceiveStoplightData(receivedPacket);
    }

    public void SendCamera(byte[] imageAsBytes)
    {
        //Debug.Log("Enviando imagem, lado esquerdo? " + sendLeftSide);
        string base64ImageRepresentation = Convert.ToBase64String(imageAsBytes);
        CameraPacket clientData = new()
        {
            isLeftSide = sendLeftSide,
            base64Image = base64ImageRepresentation,
        };

        var messageBytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(clientData));

        websocket?.Send(messageBytes);
    }

    private IEnumerator PrintAndSend()
    {
        var camera = sendLeftSide ? crossroadController.cameraLeft : crossroadController.cameraRight;
        var renderTexture = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2);
        camera.targetTexture = renderTexture;

        yield return new WaitForEndOfFrame();

        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAHalf, false);
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;
        // camera.Render();

        renderTexture.Release();

        var bytes = texture.EncodeToPNG();

        SendCamera(bytes);
        Destroy(texture);

        sendLeftSide = !sendLeftSide;
        printCoroutine = null;
    }
}
