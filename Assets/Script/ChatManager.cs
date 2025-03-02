using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public TMP_InputField chatInput;   // ���ͧ��͡��ͤ���᪷
    public TMP_Text chatDisplay;       // ���ͧ�ʴ���ͤ���᪷
    public TMP_InputField ipInput;     // ���ͧ��͡ IP/DNS
    public TMP_InputField portInput;   // ���ͧ��͡ Port

    private UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    public void StartHost()
    {
        if (ushort.TryParse(portInput.text, out ushort port))
        {
            transport.ConnectionData.Address = "0.0.0.0"; // �ѧ�ء IP
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
            transport.ConnectionData.Address = ipInput.text; // �� IP/DNS ����͡
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

            // Host �����繢�ͤ�������ͧ�ѹ��
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
