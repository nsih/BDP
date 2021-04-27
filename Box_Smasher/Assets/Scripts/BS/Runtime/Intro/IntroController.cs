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

        void OnEnter(GameObject obj){
            _player.IsControllable = true;
            IntroStartArea.OnEnter -= OnEnter;
            obj.SetActive(false);
        }

        void OnPlay(){
            _player.PhysicManager.AddForce(Vector2.one * 300);
            _startMenu.OnPlay -= OnPlay;
        }
    }
}
