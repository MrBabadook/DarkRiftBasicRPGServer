using System;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;

namespace DarkRiftRPG
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        public GameObject ServerPlayerPrefab;

        public Dictionary<ushort, GameObject> CurrentPlayers = new Dictionary<ushort, GameObject>();
        List<PlayerPositionInputData> UnprocessedPlayerMovementInput = new List<PlayerPositionInputData>();
        List<PlayerPositionInputData> ProccessedPlayerMovementInput = new List<PlayerPositionInputData>();

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SpawnPlayerOnServer(ushort clientID)
        {
            if (!CurrentPlayers.ContainsKey(clientID))
            {
                GameObject go = Instantiate(ServerPlayerPrefab, ServerPlayerPrefab.transform.position, Quaternion.identity);
                CurrentPlayers.Add(clientID, go);

                PlayerSpawnData data = new PlayerSpawnData(clientID, go.transform.position);
                ServerManager.Instance.ConnectedClients[clientID].SpawnLocalPlayerOnClient(data);

                ServerPlayerController controller = CurrentPlayers[clientID].GetComponent<ServerPlayerController>();
                ServerManager.Instance.SendNewPlayerToOthers(clientID, controller.transform.position);
                ServerManager.Instance.SendOthersToNewPlayer(clientID, CurrentPlayers);
            }
        }

        public void HandlePlayerMovementRequest(ushort clientID, Vector3 playerClickLocation)
        {
            PlayerPositionInputData input = new PlayerPositionInputData(clientID, playerClickLocation);
            UnprocessedPlayerMovementInput.Add(input);
        }

        private void FixedUpdate()
        {
            foreach (PlayerPositionInputData input in UnprocessedPlayerMovementInput)
            {
                ServerPlayerController controller = CurrentPlayers[input.ID].GetComponent<ServerPlayerController>();

                controller.UpdateNavTarget(input.Pos);

                ProccessedPlayerMovementInput.Add(input);
            }

            ProccessedPlayerMovementData proccessedMovement = new ProccessedPlayerMovementData(ProccessedPlayerMovementInput.ToArray());
            ServerManager.Instance.SendToAll(Tags.PlayerMovementUpdate, proccessedMovement);

            UnprocessedPlayerMovementInput.Clear();
            ProccessedPlayerMovementInput.Clear();

        }
    }
}

