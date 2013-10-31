using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DevPro.Game.Network.Enums;
using UnityEngine;

namespace DevPro.Game.Network.Helpers
{
    public class GameConnection
    {
        public bool IsConnected { get; private set; }

        private TcpClient m_client;
        private BinaryReader m_reader;
        private Thread m_thread;

        private Queue<GameClientPacket> m_sendQueue;
        private Queue<GameServerPacket> m_receiveQueue;

        private DateTime m_lastAction;

        public GameConnection(IPAddress address, int port)
        {
            m_sendQueue = new Queue<GameClientPacket>();
            m_receiveQueue = new Queue<GameServerPacket>();
            m_lastAction = DateTime.Now;
			try
			{
            	m_client = new TcpClient(address.ToString(), port);
				Debug.Log ("Connected to duel server.");
			}
			catch
			{
				Debug.Log ("Failed to connect to Duel Server.");
				return;
			}
            IsConnected = true;
            m_reader = new BinaryReader(m_client.GetStream());
            m_thread = new Thread(NetworkTick);
            m_thread.Start();
        }

        public void Send(GameClientPacket packet)
        {
            lock (m_sendQueue)
                m_sendQueue.Enqueue(packet);
        }

        public void Send(CtosMessage message)
        {
            Send(new GameClientPacket(message));
        }

        public void Send(CtosMessage message, byte value)
        {
            GameClientPacket packet = new GameClientPacket(message);
            packet.Write(value);
            Send(packet);
        }

        public void Send(CtosMessage message, int value)
        {
            GameClientPacket packet = new GameClientPacket(message);
            packet.Write(value);
            Send(packet);
        }

        public bool HasPacket()
        {
            lock (m_receiveQueue)
                return m_receiveQueue.Count > 0;
        }

        public GameServerPacket Receive()
        {
            lock (m_receiveQueue)
            {
                if (m_receiveQueue.Count == 0)
                    return null;
                return m_receiveQueue.Dequeue();
            }
        }

        public void Close()
        {
            if (!IsConnected) return;
            IsConnected = false;
            m_client.Close();
        }

        private void NetworkTick()
        {
            try
            {
                int connectionCheckTick = 100;
                while (IsConnected)
                {
                    InternalTick();
                    if (--connectionCheckTick <= 0)
                    {
                        connectionCheckTick = 100;
                        if (!CheckIsConnected())
                            Close();
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void InternalTick()
        {
            lock (m_sendQueue)
            {
                while (m_sendQueue.Count > 0)
                    InternalSend(m_sendQueue.Dequeue());
            }
            while (m_client.Available > 1)
            {
                GameServerPacket packet = InternalReceive();
                lock (m_receiveQueue)
                {
                    m_receiveQueue.Enqueue(packet);
                }
            }
        }

        private void InternalSend(GameClientPacket packet)
        {
            m_lastAction = DateTime.Now;
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            byte[] content = packet.GetContent();
            writer.Write((short)content.Length);
            if (content.Length > 0)
                writer.Write(content);
            byte[] data = ms.ToArray();
            m_client.Client.Send(data);
        }

        private GameServerPacket InternalReceive()
        {
            m_lastAction = DateTime.Now;
            int len = m_reader.ReadInt16();
            GameServerPacket packet = new GameServerPacket(m_reader.ReadBytes(len));
            return packet;
        }

        private bool CheckIsConnected()
        {
            TimeSpan diff = DateTime.Now - m_lastAction;
            if (diff.TotalMinutes > 5)
                return false;
            return true;
        }
    }
}