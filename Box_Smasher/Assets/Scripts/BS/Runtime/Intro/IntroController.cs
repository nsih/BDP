using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.Events;
using BS.Player;
using BS.UI;
using BS.Manager;
using UnityEngine;

namespace BS.Intro.Manager{
    public class IntroController : BaseManager<IntroController>
    {
        public PlayerController _player;
        public StartMenuManager _startMenu;

        private void Awake()
        {
            if(_startMenu != null){
                _startMenu.OnPlay += OnPlay;
            }else{
                Debug.LogWarning("Intro에 Start Menu가 연결되어있지 않습니다.");
            }
            IntroStartArea.OnEnter += OnEnter;
        }

        private void Start()
        {
            _player.PhysicManager.Freeze();
            _player.AnimController.Off();
        }

        void OnEnter(GameObject obj){
            _player.IsControllable = true;
            IntroStartArea.OnEnter -= OnEnter;
            obj.SetActive(false);
        }

        IEnumerator StartIntroLoop(){
            yield return StartCoroutine(_startMenu.FadeOutStartMenuLoop(1f));
            _player.AnimController.On();
            _player.PhysicManager.UnFreeze();
            _player.PhysicManager.AddForce(Vector2.one * 300);
            yield return null;
        }

        void OnPlay(){
            StartCoroutine(StartIntroLoop());
            _startMenu.OnPlay -= OnPlay;
        }
    }
}
