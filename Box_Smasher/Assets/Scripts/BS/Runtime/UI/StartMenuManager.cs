using System.Collections;
using System.Collections.Generic;
using BS.Utils;
using UnityEngine;


namespace BS.UI{    
    public class StartMenuManager : MonoBehaviour
    {
        public delegate void OnPlayEvent();
        public event OnPlayEvent OnPlay;

        private CanvasGroup _canvas;

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOutStartMenuLoop(float t){
            float duration = t;

            _canvas.blocksRaycasts = false;
            _canvas.interactable = false;
            while(duration > 0){
                duration -= Time.deltaTime;
                _canvas.alpha = (duration / t);
                yield return null;
            }

            this.OnOff(false);
            _canvas.alpha = 0;
            yield return null;
        }

        public void OnOff(bool signal){
            _canvas.blocksRaycasts = signal;
            _canvas.interactable = signal;
            this.gameObject.SetActive(signal);
        }

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
