using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BS.Projectile;

namespace Tests{
    public class BulletTestScript{

        [Test]
        public void Bullet_Init_Rigid_Test(){
            var gameObj = new GameObject();
            var rigid = gameObj.AddComponent<Rigidbody2D>();
            var bullet = gameObj.AddComponent<Bullet>();

            float speed = 3.5f;
            bullet.Init(speed);

            // Use the Assert class to test conditions
            Assert.AreEqual(bullet.Speed, speed);
        }

        [Test]
        public void Bullet_Init_Speed_Test(){
            var gameObj = new GameObject();
            var rigid = gameObj.AddComponent<Rigidbody2D>();
            var bullet = gameObj.AddComponent<Bullet>();

            float speed = 3.5f;
            bullet.Init(speed);

            // Use the Assert class to test conditions
            Assert.AreEqual(bullet.Rigid, rigid);
        }
    }
}
