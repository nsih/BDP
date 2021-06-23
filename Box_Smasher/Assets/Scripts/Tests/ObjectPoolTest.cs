using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BS.Utils;
using BS.Projectile;

/// <summary>
/// ObjectPool의 주요 기능 Enqueue, Dequeue를 테스트한다.
/// </summary>

namespace Tests{
    public class ObjectPoolTest{
        
        [Test]
        public void ObjectPoolInstanceTest(){
            ObjectPool instance = ObjectPool.Instance;

            Assert.IsNotNull(instance);
        }

        [Test]
        public void ObjectPoolEnqueueTest(){
            GameObject obj = new GameObject();
            Bullet bullet = obj.AddComponent<Bullet>();

            ObjectPool.Instance.Enqueue(bullet);

            Assert.AreEqual(1, ObjectPool.Instance.Count);
        }

        [Test]
        public void ObjectPoolDequeueTest(){
            Bullet bullet = ObjectPool.Instance.Dequeue();

            Assert.IsNull(bullet);
        }
    }
}
