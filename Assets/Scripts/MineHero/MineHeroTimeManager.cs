using System;
using UniRx;
using UnityEngine;

namespace Dasverse.Aleo
{
    public class MineHeroTimeManager : MonoBehaviour
    {
        public static MineHeroTimeManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
            }
        }

        [HideInInspector]
        public bool IsGameStart;
        [HideInInspector]
        public bool IsSpecialSpawn;
        [HideInInspector]
        public bool IsBarrierOn;
        [HideInInspector]
        public bool IsDoubleChance;
        [HideInInspector]
        public bool IsEmpCoolDown;
        [HideInInspector]
        public bool IsIgnoreTime;
        [HideInInspector]
        public int MultiComboCount;
        [HideInInspector]
        public long NomalSpwanTime;
        [HideInInspector]
        public long TotalMilliSeconds;
        [HideInInspector]
        public long SpecialTime;
        [HideInInspector]
        public bool IsFinishStart;
        [HideInInspector]
        public long FinishTime;
        [HideInInspector]
        public ReactiveProperty<long> BarrierTimeRp;
        [HideInInspector]
        public ReactiveProperty<long> DoubleTimeRp;
        [HideInInspector]
        public ReactiveProperty<long> IgnoreTimeRp;
        [HideInInspector]
        public ReactiveProperty<long> EmpCoolDownRp;

        private long startTime;
        private long specialSpawnStartTime;
        private long barrierStartTime;
        private long doubleStartTime;
        private long ignoreStartTime;
        private long finishStartTime;

        public void Init()
        {
            IsGameStart = false;
            IsSpecialSpawn = false;
            TotalMilliSeconds = 0;
            MultiComboCount = 0;
            FinishTime = 0;

            BarrierTimeRp = new ReactiveProperty<long>();
            BarrierTimeRp.Subscribe(x =>
            {
                if (x >= 5000)
                {
                    BarrierTimeEnd();
                }
            });

            DoubleTimeRp = new ReactiveProperty<long>();
            DoubleTimeRp.Subscribe(x =>
            {
                if (x >= 5000)
                {
                    DoubleTimeEnd();
                }
            });

            IgnoreTimeRp = new ReactiveProperty<long>();
            IgnoreTimeRp.Subscribe(x =>
            {
                if (x >= 5000)
                {
                    IgnoreTimeEnd();
                }
            });

            EmpCoolDownRp = new ReactiveProperty<long>();
            EmpCoolDownRp.Subscribe(x =>
            {
                if (x >= 2000)
                {
                    EmpCoolDownEnd();
                }
            });
        }

        public void GameRestartInit()
        {
            IsGameStart = false;
            IsFinishStart = false;
            MultiComboCount = 0;
            TotalMilliSeconds = 0;
            SpecialTimeEnd();
        }

        private void Update()
        {
            if (IsGameStart)
            {
                long ticks = TimeUtil.Now.Ticks;
                TotalMilliSeconds = (ticks - startTime) / TimeSpan.TicksPerMillisecond;

                if (IsSpecialSpawn)
                {
                    SpecialTime = (ticks - specialSpawnStartTime) / TimeSpan.TicksPerMillisecond;
                }

                if (IsBarrierOn)
                {
                    BarrierTimeRp.Value = (ticks - barrierStartTime) / TimeSpan.TicksPerMillisecond;
                }

                if (IsDoubleChance)
                {
                    DoubleTimeRp.Value = (ticks - doubleStartTime) / TimeSpan.TicksPerMillisecond;
                }

                if (IsIgnoreTime)
                {
                    IgnoreTimeRp.Value = (ticks - ignoreStartTime) / TimeSpan.TicksPerMillisecond;
                }

                if (IsEmpCoolDown)
                {
                    EmpCoolDownRp.Value = (ticks - ignoreStartTime) / TimeSpan.TicksPerMillisecond;
                }

                if (IsFinishStart)
                {
                    FinishTime = (ticks - finishStartTime) / TimeSpan.TicksPerMillisecond;
                }
            }

        }

        public void GameStart()
        {
            startTime = TimeUtil.Now.Ticks;
            IsGameStart = true;
        }

        public void GameEnd()
        {
            IsGameStart = false;
        }

        public void SpecialTimeStart()
        {
            IsSpecialSpawn = true;
            SpecialTime = 0;
            specialSpawnStartTime = TimeUtil.Now.Ticks;
        }

        public void SpecialTimeEnd()
        {
            IsSpecialSpawn = false;
        }

        public void BarrierTimeStart()
        {
            IsBarrierOn = true;
            barrierStartTime = TimeUtil.Now.Ticks;
        }

        public void BarrierTimeEnd()
        {
            IsBarrierOn = false;
            BarrierTimeRp.SetValueAndForceNotify(0);
        }

        public void DoubleTimeStart()
        {
            IsDoubleChance = true;
            doubleStartTime = TimeUtil.Now.Ticks;
        }

        public void DoubleTimeEnd()
        {
            IsDoubleChance = false;
            DoubleTimeRp.SetValueAndForceNotify(0);
        }

        public void FinishTimeStart()
        {
            IsFinishStart = true;
            finishStartTime = TimeUtil.Now.Ticks;
        }

        public void FinishTimeEnd()
        {
            IsFinishStart = false;
        }

        public void EmpCoolDownStart()
        {   
            IsEmpCoolDown = true;
        }

        public void EmpCoolDownEnd()
        {
            IsEmpCoolDown = false;
            EmpCoolDownRp.SetValueAndForceNotify(0);
        }

        public void IgnoreTimeStart()
        {   
            IsIgnoreTime = true;
            ignoreStartTime = TimeUtil.Now.Ticks;
        }

        public void IgnoreTimeEnd()
        {
            IsIgnoreTime = false;
            IgnoreTimeRp.SetValueAndForceNotify(0);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (IsGameStart)
                {
                    CrystalSpawner.Instance.EndSpawn();
                    CrystalSpawner.Instance.ReturnAllCrystal(false);
                }
            }
            else
            {
                if (IsGameStart)
                {
                    IsGameStart = false;
                    TotalMilliSeconds = (TimeUtil.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;

                    if (IsSpecialSpawn)
                    {
                        SpecialTime = (TimeUtil.Now.Ticks - specialSpawnStartTime) / TimeSpan.TicksPerMillisecond;
                        CrystalSpawner.Instance.SpecialSpawnCountIndexingAsync((int)SpecialTime).Forget();
                    }

                    if (IsBarrierOn)
                        BarrierTimeRp.Value = (TimeUtil.Now.Ticks - barrierStartTime) / TimeSpan.TicksPerMillisecond;

                    if (IsDoubleChance)
                        DoubleTimeRp.Value = (TimeUtil.Now.Ticks - doubleStartTime) / TimeSpan.TicksPerMillisecond;

                    if (IsIgnoreTime)
                        IgnoreTimeRp.Value = (TimeUtil.Now.Ticks - ignoreStartTime) / TimeSpan.TicksPerMillisecond;

                    if (IsFinishStart)
                    {
                        FinishTime = (TimeUtil.Now.Ticks - finishStartTime) / TimeSpan.TicksPerMillisecond;
                        CrystalSpawner.Instance.SpawnCountIndexingAsync((int)FinishTime).Forget();
                    }
                    else
                    {
                        CrystalSpawner.Instance.SpawnCountIndexingAsync((int)TotalMilliSeconds).Forget();
                    }
                }

            }
        }
    }
}

