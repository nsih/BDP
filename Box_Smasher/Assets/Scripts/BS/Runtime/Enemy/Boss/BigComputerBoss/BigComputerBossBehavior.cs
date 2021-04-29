using System;
using BS.BehaviorTrees.Trees;
using BS.BehaviorTrees.Tasks;
using UnityEngine;

namespace BS.Enemy.Boss{

    public class BigComputerBossBehavior : BaseBossBehavior
    {
        [Flags]
        enum AttackType {
            DRONE,
            SHOTGUN,
            LASER,
            EXPLOSION_CORE,
            SMASH
        }

        const int MAX_HEALTH = 10000;
        private AttackType _attackType;

        public override void Init()
        {
            base.Init();
            _boss.SetMaxHealth(MAX_HEALTH);
            
            #region behavior tree
            _tree = new BehaviorTreeBuilder(gameObject)
                        .Selector()
                            .Condition(() => _boss.IsDead)
                            .Condition(() => _boss.IsInvincible)
                            .Sequence()
                                .Selector("Attack")
                                    .Sequence("Drone")
                                        .Condition(() => _attackType == AttackType.DRONE)
                                        .Do("Drone Attack", () => {
                                                DroneAttack();
                                                return TaskStatus.Success;
                                            })
                                        .WaitTime(0.8f)
                                    .End()
                                    .Sequence("Shotgun")
                                        .Condition(() => _attackType == AttackType.SHOTGUN)
                                        .Do("Fire Shotgun", () => {
                                                FireShotgun();
                                                return TaskStatus.Success;
                                            })
                                        .WaitTime(1.5f)
                                    .End()
                                    .Sequence("Laser")
                                        .Condition(() => _attackType == AttackType.LASER)
                                        .Do("Fire Laser", () => {
                                                Laser(Vector2.right);
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(3f)
                                    .End()
                                    .Sequence("ExplosionCore")
                                        .Condition(() => _attackType == AttackType.EXPLOSION_CORE)
                                        .Do("Fire Explosion Core", () => {
                                                FireExplosionCore();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(0.8f)
                                    .End()
                                    .Sequence("Smash")
                                        .Condition(() => _attackType == AttackType.SMASH)
                                        .Do("Smash Player", () => {
                                                Smash();
                                                return TaskStatus.Success; 
                                            })
                                        .WaitTime(1.6f)
                                    .End()
                                .End()
                                .Do("Choose Random Attack Type", () => {
                                    _attackType = BS.Utils.Random.RandomEnum<AttackType>();
                                    return TaskStatus.Success;
                                })
                            .End()
                    .Build();
            #endregion
        }

        #region 공격패턴 1단계
        private void DroneAttack(){
            Debug.Log("드론 공격");
        }

        private void FireShotgun(){
            Debug.Log("샷건 발사");
        }

        private void Laser(Vector2 axis){
            Debug.Log("레이저 공격");
        }

        private void FireExplosionCore(){
            Debug.Log("폭발 코어 공격");
        }   

        private void Smash(){
            Debug.Log("스메쉬");
        }
        #endregion
    }
}
