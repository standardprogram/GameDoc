using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using LitJson;

public class NetMgr : MonoBehaviour {

	public static NetMgr Instance = null;
	public string host = "127.0.0.1";
	public int port = 55555;

	private Socket _tcpSock;

	void Awake(){
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		_tcpSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		IPAddress ip = IPAddress.Parse (host);
		IPEndPoint ipe = new IPEndPoint (ip, port);
		_tcpSock.Connect (ipe);
	}

	private void unPackHead(byte[] head, out int commandId, out int size){
		int head0 = head [0];
		int head1 = head [1];
		int head2 = head [2];
		int head3 = head [3];
		int protoVersion = head [4];
		int serverVersion = System.BitConverter.ToInt32 (head, 5);
		// todo check the head, version
		System.Array.Reverse(head, 9, 4);
		size = System.BitConverter.ToInt32(head, 9);

		System.Array.Reverse(head, 13, 4);
		commandId = System.BitConverter.ToInt32(head, 13);
	}
	
	// Update is called once per frame
	void Update () {
		if (_tcpSock.Connected && _tcpSock.Poll(1, SelectMode.SelectRead))
		{
			int commandId, size;

			byte[] head = new byte[17];
			int len = _tcpSock.Receive(head, 0, head.Length, SocketFlags.None);

			unPackHead(head, out commandId, out size);


			byte[] msg = new byte[size-4];
			_tcpSock.Receive(msg, 0, msg.Length, SocketFlags.None);

			JsonReader jsonR = new JsonReader(System.Text.Encoding.Default.GetString(msg));
			JsonData jData = JsonMapper.ToObject(jsonR);

			if((int)jData["status"] == 1){
				Debug.Log(jData["data"]);
			}
		}
	}

	private byte[] packMsg(byte[] msg, int commandId){
		byte[] head = new byte[17];
		head [0] = 0;
		head [1] = 0;
		head [2] = 0;
		head [3] = 0;
		head [4] = 0; // protoVersion
		Array.Copy (System.BitConverter.GetBytes (0), 0, head, 5, 4); // serverVersion

		byte[] lengthByte = new byte[4];
		lengthByte = System.BitConverter.GetBytes (msg.Length + 4);
		Array.Reverse (lengthByte);
		Array.Copy (lengthByte, 0, head, 9, 4);

		byte[] commandIdByte = new byte[4];
		commandIdByte = System.BitConverter.GetBytes (commandId);
		Array.Reverse (commandIdByte);
		Array.Copy (commandIdByte, 0, head, 13, 4);

		byte[] ret = new byte[head.Length + msg.Length];
		Array.Copy (head, 0, ret, 0, head.Length);
		Array.Copy (msg, 0, ret, head.Length, msg.Length);
		return ret;
	}

	public void SendMsg(byte[] msg, int commandId){
		byte[] data = packMsg (msg, commandId);
		_tcpSock.Send(data, data.Length, 0);//发送信息
	}
}