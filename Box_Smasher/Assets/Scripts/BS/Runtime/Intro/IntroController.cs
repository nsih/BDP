using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace BS.Manager.Opening{
    public class IntroController : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.KeypadEnter)){
                Debug.Log("게임 시작");
            }
        }

        private void DestroySelf(){
            Destroy(this.gameObject);
        }
    }
}
