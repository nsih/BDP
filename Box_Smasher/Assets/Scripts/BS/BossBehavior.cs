using System.Collections;
using System.Collections.Generic;
using BS.BehaviorTrees.Trees;
using BS.BehaviorTrees.Tasks;
using UnityEngine;

namespace BS.Enemy.Boss{
    public class BossBehavior : MonoBehaviour{
        public float DamageForce;
        public float MaxHP;
        public float HP;
        public float LastHP;
        public bool Invincible = false;
        public bool DEAD = false;
        
        public GameObject BulletPrefab;
        
        Transform PlayerTransform;
        Rigidbody2D PlayerRigid2D;
        ctw_Player_behavior PlayerScript;
        
        ctw_Camera_behavior CameraScript;
        
        Transform BossTransform;
        SpriteRenderer BossSprite;
        ctw_Eraser_behavior Eraser;
        
        public GameObject[] Bullet = new GameObject[250];
        public int BulletPool = 0;
        
        bool ATTACK = true;
        int AttackType = 0;
        
        float AlphaInvincible = 0;

        [SerializeField]
        private BehaviorTree _tree;
        
        void Awake(){
            MaxHP = 10000;
            HP = 10000;
            LastHP = 10000;
            
            PlayerTransform = GameObject.Find("ctw_Player").GetComponent<Transform>();
            PlayerRigid2D = GameObject.Find("ctw_Player").GetComponent<Rigidbody2D>();
            PlayerScript = GameObject.Find("ctw_Player").GetComponent<ctw_Player_behavior>();
            
            CameraScript = GameObject.Find("ctw_Main Camera").GetComponent<ctw_Camera_behavior>();
            
            BossTransform = GetComponent<Transform>();
            BossSprite = GetComponent<SpriteRenderer>();
            Eraser = GameObject.Find("ctw_Eraser_Boss").GetComponent<ctw_Eraser_behavior>();

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
        public void OnDamaged(float Damage){    
            if (!Invincible){
                if (HP > Damage){
                    LastHP = HP;
                    HP -= Damage;
                    AttackType = 0;
                    ATTACK = false;
                    AlphaInvincible = 0f;
                    Eraser.Alpha = 1f;
                    Invincible = true;
                    Invoke("Timer_InvincibleCool",3.0f);
                    CameraScript.CamShake = 1f;
                }
                else if (HP != 0) {
                    LastHP = HP;
                    HP = 0;
                    DEAD = true;
                    Eraser.Alpha = 1f;
                    CameraScript.CamShake = 2f;
                }
            }
        }
         
        // Gets
        
        float Get_angle_byPosition(Vector3 Pos){
            
            Vector3 BossPos = BossTransform.position;
            
            return ( Mathf.Atan2(Pos.y-BossPos.y, Pos.x-BossPos.x) * Mathf.Rad2Deg );
        }
        
        Vector2 Get_Force_Direction(){
            
            Vector3 PlayerPos = PlayerTransform.position;
            Vector3 BossPos = BossTransform.position;
            Vector2 ForceDirection = (PlayerPos - BossPos).normalized;
            
            return ForceDirection;
        }
        
        Vector3 Get_Target_AngleToPos(float angle){
            
            return new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad), 0); 
        }
        
        
        Vector3 Get_Vector3_Direction(Vector3 Pos){
            
            float RangeKey = Math_2D_Force(Pos.x,Pos.y);
            
            Pos = Pos/RangeKey;
            
            Vector3 VectorDirection = new Vector3(Pos.x,Pos.y,0);
            
            return VectorDirection;
        }
        
        float Get_Vector3_Range(Vector3 Pos1, Vector3 Pos2){
            
            return Mathf.Sqrt(Mathf.Pow((Pos1.x - Pos2.x),2)+Mathf.Pow((Pos1.y - Pos2.y),2));
        }
        
        Quaternion Get_toPlayer_rotation(Vector3 Pos){
            
            Vector3 PlayerPos = PlayerTransform.position;
            
            float angle = Mathf.Atan2(PlayerPos.y-Pos.y, PlayerPos.x-Pos.x) * Mathf.Rad2Deg;
            
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            return rotation;
        }
        
        
        
        // Attacks
        
        GameObject Attack_Gen_Bullet(){
            

            BulletPool++;
            
            return Instantiate(BulletPrefab, BossTransform);
        }
        
        GameObject Attack_CheckandReturn(){
            
            int Key = 0;
            int i = 0;
            
            if (BulletPool != 0){
                for(;i<BulletPool; i++){
                    if (Bullet[i].GetComponent<ctw_Bullet_behavior>().OnWork == false){
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
        
        void Attack_SetBullet(float Force,Vector3 Target,Quaternion rotation, float Timer_t, float roll){
            
            GameObject Bullet = Attack_CheckandReturn();
            Vector3 BossPos = BossTransform.position;
            
            Transform BulletTransform = Bullet.GetComponent<Transform>();
            ctw_Bullet_behavior BulletScript = Bullet.GetComponent<ctw_Bullet_behavior>();
            
            BulletTransform.position = BossPos;
            BulletTransform.rotation = rotation;
            BulletScript.Vel = Get_Vector3_Direction(Target)*Force;
            BulletScript.OnWork = true;
            BulletScript.Timer = Timer_t;
            BulletScript.Roll = roll;
            BulletScript.Alpha = 0f;
        }
        
        void Attack_TeleportBullet(float Force,Vector3 Target,Quaternion rotation, float Timer_t, float roll, Vector3 TPlocation){
            
            GameObject Bullet = Attack_CheckandReturn();
            
            Transform BulletTransform = Bullet.GetComponent<Transform>();
            ctw_Bullet_behavior BulletScript = Bullet.GetComponent<ctw_Bullet_behavior>();
            
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
            
            // Rendering();
        }
    }
}