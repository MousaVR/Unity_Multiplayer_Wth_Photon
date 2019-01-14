using System.Collections;
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
        #endregion
        
        #region Private Fields
        [Tooltip("The Beams GameObject To control")]
        [SerializeField] GameObject beams;
        bool isFiring;
        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            if (beams == null) Debug.LogError("<Color=Red>Missing</Color> Beams Reference.");
            else beams.SetActive(false);
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
                stream.SendNext(isFiring);
            else
                this.isFiring =(bool)stream.ReceiveNext();
        }

        #endregion
    }
}