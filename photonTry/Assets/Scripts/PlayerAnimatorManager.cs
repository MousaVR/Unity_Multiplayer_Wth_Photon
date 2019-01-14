using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace VRapeutic.CerebralPalsy
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields
        [SerializeField] float directioDampTime=0.25f;
        #endregion

        #region MonoBehaviour Callbacks
        private Animator animator;
        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator) Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }

        // Update is called once per frame
        void Update()
        {
            //important to determine which player the computer control 
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)  return;
            if (!animator) return;
            // deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0) v = 0;
            animator.SetFloat("Speed",h*h+v*v );
            //animator.SetFloat("Direction", h, directioDampTime, Time.deltaTime);
            animator.SetFloat("Direction", h, directioDampTime, Time.deltaTime);
        }
        #endregion
    }
}