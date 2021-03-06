﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace VRapeutic.CerebralPalsy
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields
        [Tooltip ("The current Health of our player")]
        public float Health=1f;
        //for instaniate players
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        #endregion

        #region Private Fields
        [Tooltip("The Beams GameObject To control")]
        [SerializeField] GameObject beams;
        bool isFiring;
        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine) PlayerManager.LocalPlayerInstance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
            if (beams == null) Debug.LogError("<Color=Red>Missing</Color> Beams Reference.");
            else beams.SetActive(false);
        }
        void Start()
        {
            //control camera over players
            
        }

        void Update()
        {
            if (photonView.IsMine &&PhotonNetwork.IsConnected==true)ProcessInputs();
            if (beams != null && isFiring != beams.activeSelf)
            beams.SetActive(isFiring); 
        }

        void OnTriggerEnter(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine) return;
            if (!other.name.Contains("Beam")) return;
            Health -= 0.1f;
            CheckHealth();
        }

        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine) return;
            if (!other.name.Contains("Beam")) return;
            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            Health -= 0.1f * Time.deltaTime;
            CheckHealth();
        }
        #endregion

        #region custom
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
                isFiring = true;
            else if (Input.GetButtonUp("Fire1"))
                isFiring = false;
        }

        void CheckHealth()
        {
            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }
        #endregion

        #region IPunObservable implementation
        public  void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)//we append our IsFiring value to the stream of data
            {
                stream.SendNext(isFiring);
                stream.SendNext(Health);
            }
            else
            {
                this.isFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}