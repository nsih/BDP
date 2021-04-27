using System.Collections;
using System.Collections.Generic;
using BS.Utils;
using UnityEngine;


namespace BS.UI{    
    public class StartMenuManager : MonoBehaviour
    {
        public delegate void OnPlayEvent();
        public event OnPlayEvent OnPlay;

        public void Play(){
            Debug.Log("게임 시작");
            if(OnPlay != null){
                OnPlay();
            }
        }

        public void Settings(){
            Debug.Log("설정 보기");
        }

        public void Exit(){
            AppHelper.Quit();
        }
    }
}
