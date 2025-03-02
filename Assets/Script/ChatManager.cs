using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public TMP_InputField chatInput;   // กล่องกรอกข้อความแชท
    public TMP_Text chatDisplay;       // กล่องแสดงข้อความแชท
    public TMP_InputField ipInput;     // กล่องกรอก IP/DNS
    public TMP_InputField portInput;   // กล่องกรอก Port

    private UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    public void StartHost()
    {
        if (ushort.TryParse(portInput.text, out ushort port))
        {
            transport.ConnectionData.Address = "0.0.0.0"; // ฟังทุก IP
            transport.ConnectionData.Port = port;
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.LogError("Invalid port number!");
        }
    }

    public void StartClient()
    {
        if (ushort.TryParse(portInput.text, out ushort port) && !string.IsNullOrEmpty(ipInput.text))
        {
            transport.ConnectionData.Address = ipInput.text; // ใช้ IP/DNS ที่กรอก
            transport.ConnectionData.Port = port;
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogError("Invalid IP or Port!");
        }
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            string message = chatInput.text;
            chatInput.text = "";

            // Host ควรเห็นข้อความตัวเองทันที
            if (IsHost)
            {
                UpdateChatDisplay($"Host: {message}");
            }

            SendChatToServerRpc(NetworkManager.Singleton.LocalClientId, message);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatToServerRpc(ulong senderId, string message)
    {
        string fullMessage = $"Player {senderId}: {message}";
        ReceiveChatOnClientsClientRpc(fullMessage);
    }

    [ClientRpc]
    private void ReceiveChatOnClientsClientRpc(string message)
    {
        UpdateChatDisplay(message);
    }

    private void UpdateChatDisplay(string message)
    {
        chatDisplay.text += message + "\n";
    }
}
