﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace VRapeutic.CerebralPalsy
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static GameManager instance;
        public static GameManager Instance {
            get
            {
                if (instance == null)
                {
                    GameObject GameManager = new GameObject();
                    GameManager.AddComponent<GameManager>();                   
                }
                return instance;
            }
        }
        void Awake()
        {
            instance = this;
        }

        #region Photon Callbacks
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
                // called before OnPlayerLeftRoom
                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                LoadArena();
            }
        }

        public override void OnLeftRoom()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        #endregion

        #region Public Methods
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Private Methods
        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        #endregion
    }
}
