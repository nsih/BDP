using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using BS.Enemy.Boss;
using BS.Manager.Cameras;
using UnityEngine;

namespace BS.Player{
	public class PlayerController : MonoBehaviour
	{
		protected PlayerAnimController _animController;
		protected PlayerPhysicManager _physicManager;
		protected ctw_Effector_behavior _effector;

		protected Rigidbody2D _rigid;
		protected Collider2D _collider;
		public float _maxPower = 2000; /// 최대 차지 파워
		[ProgressBar("Attack Power", 2000, EColor.Green)]
		public float _currentPower = 0; /// 현재 차지 파워
		
		public Animator _bodyAnimator;
		public Animator _faceAnimator;
		public BulletEraser _eraser;
		
		public PhysicsMaterial2D Normal;
		public PhysicsMaterial2D Bouncy;
		
		Camera _mainCamera;
		
		public float _moveDirection;
		public bool DOWN = false;
		public int OnHit = 0;
		[ProgressBar("Health", 3, EColor.Red)]
		public int HP = 3;
		public bool Invincible = false;
		public bool IsCharging = false;
		public bool Dead = false;
		public bool AttackSuccess = false;

		void Awake(){
			_rigid = GetComponent<Rigidbody2D>();
			_collider = GetComponent<CapsuleCollider2D>() as Collider2D;

			// Init animation controller
			_animController = GetComponent<PlayerAnimController>() ?? this.transform.gameObject.AddComponent<PlayerAnimController>();
			_animController.Init(this, _bodyAnimator, _faceAnimator);

			// Init Physic Manager
			_physicManager = GetComponent<PlayerPhysicManager>() ?? this.transform.gameObject.AddComponent<PlayerPhysicManager>();
			_physicManager.Init(this, _rigid);

			_effector = FindObjectOfType<ctw_Effector_behavior>();
			
			if(_eraser == null){
				Debug.LogError("eraser가 할당되어 있지 않습니다.");
			}
			if(_effector == null){
				Debug.LogError("effector가 할당되어 있지 않습니다.");
			}

			_mainCamera = CameraManager.Instance.MainCamera;
		}
		
		// Maths
		
		float Math_2D_Force(float x, float y){
			return Mathf.Sqrt(Mathf.Pow(x,2)+Mathf.Pow(y,2));
		}
		
		float Math_Boss_Damage(){
			return (500f + Mathf.Abs((_currentPower * Math_2D_Force(_rigid.velocity.x, _rigid.velocity.y) / 50f)) );
		}
		
		
		
		// Timers
		void TimerAttackReset(){
			OnHit = 0;
		}
		
		void TimerInvincibleReset(){
			Invincible = false;
		}
		
		// Gets
		Vector2 GetForceDirection(){
			
			Vector3 PlayerPos = this.transform.position;
			Vector3 MouseStaticPos = new Vector3(_mainCamera.ScreenToWorldPoint(Input.mousePosition).x,_mainCamera.ScreenToWorldPoint(Input.mousePosition).y,0);
			Vector3 MousePrivatePos = MouseStaticPos - PlayerPos;
			
			float RangeKey = Math_2D_Force(MousePrivatePos.x,MousePrivatePos.y);
			
			MousePrivatePos = MousePrivatePos/RangeKey;
			
			Vector2 ForceDirection = new Vector2(MousePrivatePos.x,MousePrivatePos.y);
			
			return ForceDirection;
		}
		
		float Get_Angle_byPosition(Vector3 Target, Vector3 Pos){
			return ( Mathf.Atan2(Target.y-Pos.y, Target.x-Pos.x) * Mathf.Rad2Deg );
		}
		
		Vector2 Get_Force_byAngle(float angle){
			return new Vector2( Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad) ); 
		}


		public bool OnAir(){
			return _physicManager._onAir;
		}

		public bool IsMoving(){
			return _physicManager._isMoving;
		}
		
		public bool IsFalling(){
			return _physicManager._isFalling;
		}

		// Checks
		void OnDamage(){
			if (HP > 1){
				HP -= 1;
				Invincible = true;
				_eraser.EraserWave(0.05f);
				_animController.OnHit();
				Invoke("TimerInvincibleReset",1.0f);
				CameraManager.Instance.ShakeCamera(0.5f, 0.05f);
			}
			else if (HP == 1){
				HP = 0;
				Dead = true;
				_animController.Dead();
				CameraManager.Instance.ShakeCamera(1f, 0.1f);
			}
		}
		
		// Effects
		void GenEffect(float angle, float F, float time, int num){
			Vector3 pos = this.transform.position;
			_effector?.Effect_Run(time, pos, Get_Force_byAngle(angle)*F, num);
		}
		
		// Inputs
		
		void InputMove(){
			_moveDirection = 0;
			
			if (OnHit == 0){
				
				if (Input.GetKey(KeyCode.A)){
					_moveDirection--;
					_physicManager._isMoving = true;
				}
				else if (Input.GetKey(KeyCode.D)){
					_moveDirection++;
					_physicManager._isMoving = true;
				}
                else
					_physicManager._isMoving = false;
					
				
				if (Mathf.Abs(_rigid.velocity.x) < 15)
				{
					_rigid.velocity = new Vector2(_rigid.velocity.x+1*_moveDirection,_rigid.velocity.y);
				}
				
				if (Input.GetKey(KeyCode.S)){
					DOWN = true;
				}
				else{
					DOWN = false;
				}
				
				if ((Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.Space))&& !_physicManager._onAir ){
					_rigid.velocity = new Vector2(_rigid.velocity.x,40);
					GenEffect(0f, 30f, 1f, 4);
					GenEffect(180f, 30f, 1f, 4);
					_physicManager._onAir = true;
				}


				//Debug.Log(ismoving);
			}
		}
		
		float AngleBetweenTwoPoints(Vector3 a, Vector3 b){
			return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
		}

		void InputAttack(){

			if(Input.GetMouseButtonDown(0) && _physicManager._onAir && (OnHit != 2)){
				_currentPower = 0;
				IsCharging = true;
				OnHit = 1;
			}
			
			if ((Input.GetMouseButton(0)) && _physicManager._onAir && (OnHit != 2)){
				Vector2 pos = this.transform.position;
				Vector2 mouseOnWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

				float angle = AngleBetweenTwoPoints(pos, mouseOnWorld);

				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

				float amount = _maxPower * 0.015f;
				float direction = (mouseOnWorld - pos).x <= 0 ? 1 : -1;

				_currentPower += amount * direction;
			}

			if ((Input.GetMouseButtonUp(0))&&(OnHit == 1)){
				if (Mathf.Abs(_currentPower) >= _maxPower){
					_currentPower = (_currentPower > 0 ? 1 : -1) * _maxPower;
				}

				_rigid.velocity = GetForceDirection() * Mathf.Abs(_currentPower) / 40;
				_collider.sharedMaterial = Bouncy;

				OnHit = 2;
				IsCharging = false;
			}
			
			if (OnHit == 1){
				_rigid.angularDrag = 0.1f;
				_rigid.drag = 2.5f;
				_rigid.gravityScale = 0.5f;
			}
			
			else{
				_rigid.angularDrag = 0.2f;
				_rigid.drag = 0.2f;
				_rigid.gravityScale = 9.8f;
			}
			
			if (OnHit != 2)
				_collider.sharedMaterial = Normal;
		}
		
		
		
		// Running
		
		void Caring(){
			
			if (_physicManager._onAir && (OnHit != 1)){
				_rigid.gravityScale = 9.8f;
			}
			
			if ( !_physicManager._onAir ){
				_rigid.gravityScale = 4.9f;
			}
		}
		
		public void ProcessEffect(Collider2D other){
			switch (other.tag){
				case "Platform":
					ctw_Platform_behavior PlatformScript = other.GetComponent<ctw_Platform_behavior>();
					
					if ((PlatformScript.Trigger == false)&&(_rigid.velocity.y <= 0)){
						if ( _physicManager._onAir ){
							GenEffect(0f, 15f, 1f, 3);
							GenEffect(180f, 15f, 1f, 3);
						}
						AttackSuccess = false;
						IsCharging = false;
					}
					break;
				case "Ground":
					if ( _physicManager._onAir && (_rigid.velocity.y <= -1f)){
						GenEffect(0f, 15f, 1f, 3);
						GenEffect(180f, 15f, 1f, 3);
					}
					AttackSuccess = false;
					IsCharging = false;
					break;
			}
		}

		void OnTriggerStay2D(Collider2D other){
			switch (other.tag){
				case "Bullet":
					if (!Invincible){
						ctw_Bullet_Collider_Script BulletScript = other.GetComponent<ctw_Bullet_Collider_Script>();
						if ((!Dead)&&(BulletScript.OnWork == true)) {
							GenEffect(Get_Angle_byPosition(this.transform.position, other.GetComponent<Transform>().position)+35f, 15f, 1f, 3);
							GenEffect(Get_Angle_byPosition(this.transform.position, other.GetComponent<Transform>().position)-35f, 15f, 1f, 3);
							OnDamage();
						}
						BulletScript.Hitted();
					}
					break;
			}
		}
		
		void OnCollisionEnter2D(Collision2D other){
			if ( (OnHit != 2) && (other.collider.name == "BS_Boss") && (!Invincible) ){
				if (!Dead) {
					GenEffect(Get_Angle_byPosition(this.transform.position, other.collider.GetComponent<Transform>().position), 25f, 2f, 10);
					OnDamage();
				}
			}
			if ( (OnHit == 2) && (other.collider.name == "BS_Boss") ){
				GenEffect(Get_Angle_byPosition(other.collider.GetComponent<Transform>().position, this.transform.position)+60f, 30f, 3f, 8);
				GenEffect(Get_Angle_byPosition(other.collider.GetComponent<Transform>().position, this.transform.position)-60f, 30f, 3f, 8);

				BossBehavior bossScript = other.collider.GetComponent<BossBehavior>();
				bossScript.OnDamaged(Math_Boss_Damage());
				AttackSuccess = true;
				_animController.AttackSuccess();
			}
			
			if (other.collider.tag != "Wall"){
				TimerAttackReset();
			}
		}
		
		private void Update(){
			if (!Dead){
				InputAttack();
				InputMove();
			}
			else {
				OnHit = 0;
				_rigid.angularDrag = 0.2f;
				_rigid.drag = 0.2f;
				_rigid.gravityScale = 9.8f;
				_collider.sharedMaterial = Normal;
			}
			
			Caring();
			if(_animController != null){
				_animController.Render();
			}
		}
	}
}

