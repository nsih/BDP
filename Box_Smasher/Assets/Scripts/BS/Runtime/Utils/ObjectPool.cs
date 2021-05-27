using System.Collections;
using System.Collections.Generic;
using BS.Manager;
using UnityEngine;

namespace BS.Utils{
    public class ObjectPool<T> : BaseManager<ObjectPool<T>> where T : class, new()
    {
        private Queue<T> _pool;
        private int _count;

        #region get
        /// <summary>
        /// 생성된 Obj 갯수
        /// </summary>
        public int Count{
            get{
                return _count;
            }
        }

        /// <summary>
        /// Pool에 들어있는 Obj 갯수
        /// </summary>
        public int PoolCount{
            get{
                return _pool.Count;
            }
        }
        #endregion

        public void Enqueue(T obj){
            if(_pool == null){
                _pool = new Queue<T>();
            }
            _pool.Enqueue(obj);
            _count++;
        }

        public T Dequeue(){
            if(_pool.Count <= 0){
                Debug.LogWarning("Pool의 숫자가 부족합니다.");
            }

            _count--;
            return _pool.Dequeue();
        }
    }
}
