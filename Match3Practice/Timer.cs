using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice
{
    public class Timer
    {
        private readonly List<TimerTask> tasks = new();

        public void Update(float dt)
        {
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                if (tasks[i].Update(dt))
                    tasks.RemoveAt(i);
            }
        }

        public void After(float delay, Action callback)
        {
            tasks.Add(new TimerTask(delay, callback));
        }

        public void Every(float interval, Action callback)
        {
            tasks.Add(new RepeatingTimerTask(interval, callback));
        }

        public void Clear()
        {
            tasks.Clear();
        }
    }

    public class TimerTask
    {
        protected float remaining;
        protected readonly Action callback;

        public TimerTask(float duration, Action callback)
        {
            remaining = duration;
            this.callback = callback;
        }

        public virtual bool Update(float dt)
        {
            remaining -= dt;

            if (remaining <= 0)
            {
                callback?.Invoke();
                return true;
            }

            return false;
        }
    }

    public class RepeatingTimerTask : TimerTask
    {
        private readonly float interval;

        public RepeatingTimerTask(float interval, Action callback)
            : base(interval, callback)
        {
            this.interval = interval;
        }

        public override bool Update(float dt)
        {
            remaining -= dt;

            while (remaining <= 0)
            {
                callback?.Invoke();
                remaining += interval;
            }

            return false;
        }
    }

}
