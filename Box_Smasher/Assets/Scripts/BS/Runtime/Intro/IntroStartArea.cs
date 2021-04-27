using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Intro{
    public class IntroStartArea : MonoBehaviour
    {
        public delegate void OnEnterEvent(GameObject g);
        public static event OnEnterEvent OnEnter;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag.Equals("Player") && OnEnter != null){
                OnEnter(this.gameObject);
            }
        }
    }
}
