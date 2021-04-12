using BS.Projectile;
using BS.BehaviorTrees.Trees;
using BS.BehaviorTrees.Tasks;
using BS.Manager.Cameras;
using BS.Player;
using UnityEngine;

namespace BS.Enemy.Boss{
    [RequireComponent(typeof(BaseBoss))]
    public class BossBehavior : MonoBehaviour{
        protected BaseBoss _boss;
        public bool Invincible = false;			// 무적상태를 체크하는 bool
        public bool DEAD = false;				// 죽었는지 체크하는 bool
        
        public GameObject BulletPrefab;			// 미리 만들어진 총알을 참조용으로 불러오는 변수
        
        Transform PlayerTransform;
        Rigidbody2D PlayerRigid2D;
        PlayerController PlayerScript;
        
        Transform BossTransform;
        SpriteRenderer BossSprite;
        public BulletEraser _eraser;
        
        public GameObject[] Bullet = new GameObject[250];
        public int BulletPool = 0;
        
        bool ATTACK = true;
        int AttackType = 0;
        
        float AlphaInvincible = 0;

        [SerializeField]
        private BehaviorTree _tree;
        
        void Awake(){
            _boss = GetComponent<BaseBoss>();
            if(!_boss.isEnabled){
                _boss.Init();
                _boss.SetMaxHealth(10000);
            }
            
            PlayerTransform = GameObject.Find("BS_Player").GetComponent<Transform>();
            PlayerRigid2D = GameObject.Find("BS_Player").GetComponent<Rigidbody2D>();
            PlayerScript = GameObject.Find("BS_Player").GetComponent<PlayerController>();
            
            BossTransform = GetComponent<Transform>();
            BossSprite = GetComponent<SpriteRenderer>();

            if(_eraser == null){
                Debug.LogWarning("eraser가 할당되지 않았습니다.");
            }

            _tree = new BehaviorTreeBuilder(gameObject)
                        .Selector()
                            .Condition(() => DEAD)
                            .Condition(() => Invincible)
                            .Sequence()
                                .Selector("Attack")
                                    .Sequence("pattern_0")
                                        .Condition(() => AttackType == 0)
                                        .Do("Fire", () => {
                                                Attack_Pattern_0();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_0();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_0();
                                                AttackType = (AttackType + 1) % 5;
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                    .End()
                                    .Sequence("pattern_1")
                                        .Condition(() => AttackType == 1)
                                        .Do("Fire", () => {
                                                Attack_Pattern_1();
                                                AttackType = (AttackType + 1) % 5;
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(1.5f)
                                    .End()
                                    .Sequence("pattern_2")
                                        .Condition(() => AttackType == 2)
                                        .Do("Fire", () => {
                                                Attack_Pattern_2();
                                                AttackType = (AttackType + 1) % 5;
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(3f)
                                    .End()
                                    .Sequence("pattern_3")
                                        .Condition(() => AttackType == 3)
                                        .Do("Fire", () => {
                                                Attack_Pattern_3();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_3();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_3();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_3();
                                                AttackType = (AttackType + 1) % 5;
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                    .End()
                                    .Sequence("pattern_4")
                                        .Condition(() => AttackType == 4)
                                        .Do("Fire", () => {
                                                Attack_Pattern_4();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(1.6f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_4();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(1.6f)
                                        .Do("Fire", () => {
                                                Attack_Pattern_4();
                                                AttackType = (AttackType + 1) % 5;
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(1.6f)
                                    .End()
                                .End()
                            .End()
                    .Build();
        }
        
        // Timers
        
        void Timer_InvincibleCool(){
            Invincible = false;
        }
        
        // Maths
        float Math_2D_Force(float x, float y){
            return Mathf.Sqrt(Mathf.Pow(x,2)+Mathf.Pow(y,2));
        }
        
        
        // Checks
        public void OnDamaged(float damage){    
            if (!Invincible){
                if (_boss.Health > damage){
                    _boss.SetHealth(_boss.Health - damage);
                    AttackType = 0;
                    ATTACK = false;
                    AlphaInvincible = 0f;
                    _eraser.EraserWave(1.2f, 0.75f);
                    Invincible = true;
                    Invoke("Timer_InvincibleCool",3.0f);
                    CameraManager.Instance.ShakeCamera(1f, 0.1f);
                }
                else if (_boss.Health != 0) {
                    _boss.SetHealth(0);
                    DEAD = true;
                    _eraser.EraserWave(3f, 0.25f);
                    CameraManager.Instance.ShakeCamera(2f, 0.2f);
                }
            }
        }
         
        // Gets
        
		// 보스의 위치에서 대상의 위치로 향하는 각도를 반환
        float Get_angle_byPosition(Vector3 Pos){
            
            Vector3 BossPos = BossTransform.position;
            
            return ( Mathf.Atan2(Pos.y-BossPos.y, Pos.x-BossPos.x) * Mathf.Rad2Deg );
        }
        
		// 총알에 부여할 힘의 방향을 좌표 형식으로 반환
        Vector2 Get_Force_Direction(){
            
            Vector3 PlayerPos = PlayerTransform.position;
            Vector3 BossPos = BossTransform.position;
            Vector2 ForceDirection = (PlayerPos - BossPos).normalized;
            
            return ForceDirection;
        }
        
		// 입력받은 각도를 좌표벡터로 변환해 반환
        Vector3 Get_Target_AngleToPos(float angle){
            
            return new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad), 0); 
        }
        
        // 입력받은 상대적 좌표를 일반화된 좌표로 변환하여 반환
        Vector3 Get_Vector3_Direction(Vector3 Pos){
            
            float RangeKey = Math_2D_Force(Pos.x,Pos.y);
            
            Pos = Pos/RangeKey;
            
            Vector3 VectorDirection = new Vector3(Pos.x,Pos.y,0);
            
            return VectorDirection;
        }
        
		// 입력받은 두 좌표간의 거리를 반환
        float Get_Vector3_Range(Vector3 Pos1, Vector3 Pos2){
            
            return Mathf.Sqrt(Mathf.Pow((Pos1.x - Pos2.x),2)+Mathf.Pow((Pos1.y - Pos2.y),2));
        }
        
		// 입력받은 좌표를 기준으로 플레이어를 바라보기 위한 총알의 각도를 반환
		// [REWORK] 픽셀 퍼펙트를 맞추기 힘들다면 각도의 일반화가 필요할 수 있음
        Quaternion Get_toPlayer_rotation(Vector3 Pos){
            
            Vector3 PlayerPos = PlayerTransform.position;
            
            float angle = Mathf.Atan2(PlayerPos.y-Pos.y, PlayerPos.x-Pos.x) * Mathf.Rad2Deg;
            
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            return rotation;
        }
        
        
        
        // Attacks
        
		// 총알을 Instance화 하여 반환
        GameObject Attack_Gen_Bullet(){
            

            BulletPool++;
            
            return Instantiate(BulletPrefab, BossTransform);
        }
        
		// 공격에 필요한 sleep 상태에 있는 총알(Onwork == false)이 존재하면 그것을 반환, 존재하지 않으면 새로운 총알 Instance 반환
        GameObject Attack_CheckandReturn(){
            
            int Key = 0;
            int i = 0;
            
            if (BulletPool != 0){
                for(;i<BulletPool; i++){
                    if (Bullet[i].GetComponent<Bullet>().OnWork == false){
                        Key = 1;
                        break;
                    }
                }
            }
            
            if (Key == 0){
                Bullet[i] = Attack_Gen_Bullet();
            }
            
            return Bullet[i];
        }
        
		// 총알의 위치를 보스의 위치로 초기화하고 공격에 사용하는 데에 쓰이는 함수
		// 인자는 순서대로 발사속도, 목표좌표, 총알의 발사각도, 총알이 발사되기까지 기다리는 틱, 총알의 회전력
        void Attack_SetBullet(float Force,Vector3 Target,Quaternion rotation, float Timer_t, float roll){
            
            GameObject Bullet = Attack_CheckandReturn();
            Vector3 BossPos = BossTransform.position;
            
            Transform BulletTransform = Bullet.GetComponent<Transform>();
            Bullet BulletScript = Bullet.GetComponent<Bullet>();
            
            BulletTransform.position = BossPos;
            BulletTransform.rotation = rotation;
            BulletScript.Vel = Get_Vector3_Direction(Target)*Force;
            BulletScript.OnWork = true;
            BulletScript.Timer = Timer_t;
            BulletScript.Roll = roll;
            BulletScript.Alpha = 0f;			// 총알의 투명도(Alpha)는 발사 후 1으로 초기화됨
        }
        
		// 총알의 위치를 특정 위치로 초기화하고 공격에 사용하는 데에 쓰이는 함수
		// 인자는 순서대로 발사속도, 목표좌표, 총알의 발사각도, 총알이 발사되기까지 기다리는 틱, 총알의 회전력, 발사가 시작될 위치
        void Attack_TeleportBullet(float Force,Vector3 Target,Quaternion rotation, float Timer_t, float roll, Vector3 TPlocation){
            
            GameObject Bullet = Attack_CheckandReturn();
            
            Transform BulletTransform = Bullet.GetComponent<Transform>();
            Bullet BulletScript = Bullet.GetComponent<Bullet>();
            
            BulletTransform.position = TPlocation;
            BulletTransform.rotation = rotation;
            BulletScript.Vel = Get_Vector3_Direction(Target)*Force;
            BulletScript.OnWork = true;
            BulletScript.Timer = Timer_t;
            BulletScript.Roll = roll;
            BulletScript.Alpha = 0f;
        }
        
        void Attack_Pattern_0()
        {
            float randomi = Random.Range(0f,20f);
            float randomj = Random.Range(-1.0f,1.0f);
            randomj = randomj/Mathf.Abs(randomj);
            for(float i = -180; i<180; i+=20){
                Attack_SetBullet(25f, Get_Target_AngleToPos(i+randomi), Quaternion.AngleAxis(i+randomi, Vector3.forward), 0f, -0.25f*randomj);
                Attack_SetBullet(18f, Get_Target_AngleToPos(i-10+randomi), Quaternion.AngleAxis(i-10+randomi, Vector3.forward), 0f, 0.2f*randomj);
            }
            ATTACK = true;
        }
        
        void Attack_Pattern_1(){
            float randomi = Random.Range(0f,9f);

            for(float i = 0; i<360; i+=10){
                Attack_SetBullet(25f, Get_Target_AngleToPos(i+randomi), Quaternion.AngleAxis(i+randomi, Vector3.forward), i, 0f);
                Attack_SetBullet(25f, Get_Target_AngleToPos(120+i+randomi), Quaternion.AngleAxis(120+i+randomi, Vector3.forward), i, 0f);
                Attack_SetBullet(25f, Get_Target_AngleToPos(240+i+randomi), Quaternion.AngleAxis(240+i+randomi, Vector3.forward), i, 0f);
            }
            ATTACK = true;
        }
        
        void Attack_Pattern_2()
        {
            for(float i = 0; i<360; i+=8){
                
                float randomi = Random.Range(0f,50f);
                
                Attack_SetBullet(10f-(i/120f), Get_Target_AngleToPos(i+randomi), Quaternion.AngleAxis(i+randomi, Vector3.forward), i, 0f);
                Attack_SetBullet(10f-(i/120f), Get_Target_AngleToPos(120+i+randomi), Quaternion.AngleAxis(120+i+randomi, Vector3.forward), i, 0f);
                Attack_SetBullet(10f-(i/120f), Get_Target_AngleToPos(240+i+randomi), Quaternion.AngleAxis(240+i+randomi, Vector3.forward), i, 0f);
            }
            ATTACK = true;

        }
        
        void Attack_Pattern_3()
        {
            float Range = Get_Vector3_Range(PlayerTransform.position, BossTransform.position);
            float angle = Get_angle_byPosition(PlayerTransform.position);
            
            Attack_SetBullet( (50f-Range), Get_Target_AngleToPos(angle), Quaternion.AngleAxis(angle, Vector3.forward), 0f, 0f);
            Attack_SetBullet( (50f-Range), Get_Target_AngleToPos(angle+7), Quaternion.AngleAxis(angle+7, Vector3.forward), 0f, 0f);
            Attack_SetBullet( (50f-Range), Get_Target_AngleToPos(angle-7), Quaternion.AngleAxis(angle-7, Vector3.forward), 0f, 0f);
            ATTACK = true;
        }
        
        void Attack_Pattern_4()
        {
            Vector3 PlayerPos = PlayerTransform.position;
            float angle = Get_angle_byPosition(PlayerTransform.position);
            float randomi = Random.Range(40f,50f);
            
            for(float i = 0;i < 360;i += 30){
                Vector3 TargetPos = PlayerPos + Get_Target_AngleToPos(i+randomi)*7f;
                Attack_TeleportBullet(25f, Get_Target_AngleToPos(i+randomi+180), Quaternion.AngleAxis(i+randomi+180, Vector3.forward), 120f, 0f, TargetPos);
            }
            
            ATTACK = true;
        }
        
        
        // Running
        void OnInvincible()
        {
            if (Invincible)
                AlphaInvincible += 0.1f;
            else if (DEAD)
                AlphaInvincible = 1.2f;
            else
                AlphaInvincible = 0f;
        }

        void Rendering()
        {
            OnInvincible();

            float R = BossSprite.color.r;
            float G = BossSprite.color.g;
            float B = BossSprite.color.b;
            
            BossSprite.color = new Color(R, G, B, Mathf.Abs((Mathf.Cos(AlphaInvincible))) );
        }
        
        void Update()
        {
            _tree.Tick();
            
            Rendering();
        }
    }
}