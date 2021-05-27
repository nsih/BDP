using System;
using BS.Player;
using UnityEngine;

namespace BS.Projectile{
	public class Bullet : MonoBehaviour{
		private bool _isEnabled;
		private bool _speed;
		private String _bulletName;
		
		private Rigidbody2D _rigid;


		/// <summary>
		/// Bullet 객체의 초기화를 담당하는 함수
		/// </summary>
		public void Init(){
			
		}

		public void Move(Vector2 dir){

		}
	}
}

