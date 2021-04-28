using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Manager{
    /// <summary>
    /// 매니저 스크립트를 위한 싱글톤 클래스
    /// </summary>
    /// <typeparam name="T">Class Type</typeparam>
    public class BaseManager<T> : MonoBehaviour where T : class, new()
    {
        protected static T _instance;

        public static T Instance {
            get{
                if(_instance == null){
                    _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if(_instance == null){
                        GameObject container = new GameObject();
                        container.name = string.Format("{0}", typeof(T).Name);
                        _instance = container.AddComponent(typeof(T)) as T;
                    }
                }

                return _instance;
            }
        }
    }
}

