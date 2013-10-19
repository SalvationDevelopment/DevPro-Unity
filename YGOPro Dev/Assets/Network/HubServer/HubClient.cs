using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using JsonFx.Json;
using DevPro.Network.Enums;
using DevPro.Network.Data;

namespace DevPro.Network
{
    public class HubClient
    {
        bool m_isConnected;
        private TcpClient m_client;
        private BinaryReader m_reader;
        private DateTime m_pingRequest;
        private Queue<MessageReceived> m_recivedQueue;
        private Queue<byte[]>  m_sendQueue;
	
		private JsonReader m_jsonReader;
		private JsonWriter m_jsonWriter;


        public HubClient()
        {
            m_client = new TcpClient();
            m_recivedQueue = new Queue<MessageReceived>();
            m_sendQueue = new Queue<byte[]>();
			m_jsonReader = new JsonReader();
			m_jsonWriter = new JsonWriter();
        }

        public bool Connect(string address, int port)
        {			
            try
            {
                m_client.Connect(address, port);
                m_reader = new BinaryReader(m_client.GetStream());
                m_isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
				Debug.Log(ex);
                return false;
            }
        }
		
		public void Login(string username, string password)
		{
			SendPacket(DevServerPackets.Login,
				m_jsonWriter.Write(new LoginRequest() 
			{ 
				Username = username, 
				Password =  EncodePassword(password), 
				UID = "Test"
			}));
		}
		
		private string EncodePassword(string password)
        {
            var salt = Encoding.UTF8.GetBytes("&^%£$Ugdsgs:;");
            var userpassword = Encoding.UTF8.GetBytes(password);

            var hmacMD5 = new HMACMD5(salt);
            var saltedHash = hmacMD5.ComputeHash(userpassword);


            //Convert encoded bytes back to a 'readable' string
            return Convert.ToBase64String(saltedHash);
        }
		
        public void Disconnect()
        {
            if (m_isConnected)
            {
                m_isConnected = !m_isConnected;
                if (m_client != null)
                    m_client.Close();
            }
        }
        public void SendPacket(DevServerPackets type, string data)
        {
            SendPacket(type, Encoding.UTF8.GetBytes(data));
        }

        private void SendPacket(DevServerPackets type, byte[] data)
        {
			Debug.Log("Sent: " + type);
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write((byte)type);
            writer.Write((short)(data.Length));
            writer.Write(data);
            lock(m_sendQueue)
                m_sendQueue.Enqueue(stream.ToArray());
        }
        public void SendPacket(DevServerPackets type)
        {
			Debug.Log("Sent: " + type);
            if (type == DevServerPackets.Ping) m_pingRequest = DateTime.Now;
            lock (m_sendQueue)
                m_sendQueue.Enqueue(new[] { (byte)type });
        }
        public void SendMessage(MessageType type, CommandType command, string channel, string message)
        {
            SendPacket(DevServerPackets.ChatMessage, 
                m_jsonWriter.Write(new ChatMessage(type,command,channel,message)));
        }

        private void SendPacket(byte[] packet)
        {
            if (!m_isConnected)
                return;
            try
            {
                try
                {
                    m_client.Client.Send(packet, packet.Length, SocketFlags.None);
                }
                catch (Exception)
                {
                    Disconnect();
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }
        private bool isLargePacket(DevClientPackets packet)
        {
            switch (packet)
            {
                case DevClientPackets.GameList:
                case DevClientPackets.UserList:
                case DevClientPackets.FriendList:
                case DevClientPackets.TeamList:
                case DevClientPackets.ChannelList:
                case DevClientPackets.ChannelUsers:
                    return true;
                default:
                    return false;
            }
        }

        private bool isOneByte(DevClientPackets packet)
        {

            switch (packet)
            {
                case DevClientPackets.LoginFailed:
                case DevClientPackets.RegisterAccept:
                case DevClientPackets.RegisterFailed:
                case DevClientPackets.Pong:
                case DevClientPackets.RefuseDuelRequest:
                    return true;
                default:
                    return false;
            }
        }

        public void HandleSendReceive()
        {
            try
            {
				if (CheckDisconnected())
				{
					Disconnect();
					return;
				}
				//handle incoming
				while(m_client.Available >= 1)
				{
					var packet = (DevClientPackets) m_reader.ReadByte();
					int len = 0;
					byte[] content = null;
					if (!isOneByte(packet))
					{
						if (isLargePacket(packet))
						{
							len = m_reader.ReadInt32();
							content = m_reader.ReadBytes(len);
						}
						else
						{
							len = m_reader.ReadInt16();
							content = m_reader.ReadBytes(len);
						}
					}

					if (len > 0)
					{
						if (content != null)
						{
							var reader = new BinaryReader(new MemoryStream(content));
							lock(m_recivedQueue)
								m_recivedQueue.Enqueue(new MessageReceived(packet, content, reader));
						}
					}
					else
						lock (m_recivedQueue)
							m_recivedQueue.Enqueue(new MessageReceived(packet, null, null));
				}
				//send packet
				while(m_sendQueue.Count > 0)
				{
					byte[] packet;
					packet = m_sendQueue.Dequeue();
					SendPacket(packet);
				}
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        private bool CheckDisconnected()
        {
            return (m_client.Client.Poll(1, SelectMode.SelectRead) && m_client.Available == 0);
        }
		
		public bool HasPacket()
		{
			return m_recivedQueue.Count > 0;
		}
		
		public MessageReceived GetPacket()
		{
			return m_recivedQueue.Dequeue();
		}

        public bool Connected()
        {
            return m_client.Connected;
        }
    }
}
