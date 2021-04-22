using System.Collections;
using System.Collections.Generic;
using System;
using DarkRift.Server;
using DarkRift;

namespace DarkRiftRPG
{
    public class ConnectedClient
    {
        public ushort ClientID;
        public IClient Client;
        public ConnectedClient(IClient client)
        {
            Client = client;
            ClientID = client.ID;

            Client.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;
            using (Message m = e.GetMessage())
            {
                switch ((Tags)m.Tag)
                {
                    case Tags.SpawnLocalPlayerRequest:
                        OnSpawnLocalPlayerRequest();
                        break;
                    case Tags.PlayerMovementRequest:
                        OnPlayerMovementRequest(m.Deserialize<PlayerMovementRequestData>());
                        break;
                }
            }
        }

        private void OnPlayerMovementRequest(PlayerMovementRequestData data)
        {
            PlayerManager.Instance.HandlePlayerMovementRequest(ClientID, data.PlayerClickLocation);
        }

        private void OnSpawnLocalPlayerRequest()
        {
            PlayerManager.Instance.SpawnPlayerOnServer(ClientID);
        }

        public void SpawnLocalPlayerOnClient(PlayerSpawnData data)
        {
            ServerManager.Instance.SendToClient(data.ID, Tags.SpawnPlayer, data);
        }

        public void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= OnMessage;
        }
    }
}

