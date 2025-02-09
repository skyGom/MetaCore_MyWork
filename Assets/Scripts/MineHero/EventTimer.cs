using System;
using UniRx;
using UnityEngine;

namespace Dasverse.Aleo
{
    public class EventTimer : MonoBehaviour
    {
        private long elapsedTime;
        private long spawnTime;
        private IDisposable timerDispodable;

        public IObservable<long> TimerObservable
        {
            get { return Observable.Interval(TimeSpan.FromMilliseconds(1)); }
        }

        private void OnDestroy()
        {
            DisposeSpawnTimer();
        }

        public void StartTimer()
        {
            timerDispodable = TimerObservable.Subscribe(_ =>
            {
                elapsedTime++;
                spawnTime++;
            });
        }

        public long GetElapsedTime()
        {
            return elapsedTime;
        }

        public long GetSpawnTime()
        {
            return spawnTime;
        }

        public void DisposeSpawnTimer()
        {
            timerDispodable?.Dispose();
            spawnTime = 0;
        }

        public void DisposeElapsedTime()
        {
            timerDispodable?.Dispose();
            elapsedTime = 0;
        }

        public void PauseTimer()
        {
            timerDispodable?.Dispose();
        }
    }
}

