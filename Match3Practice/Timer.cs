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

        public TimerChain Chain()
        {
            var chain = new TimerChain();
            tasks.Add(chain);
            return chain;
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

    /// <summary>
    /// A queue of steps (delays + callbacks) that run one after another,
    /// similar to knife.chain in Lua. Build it with Wait/Then/After, then
    /// call Start() to kick it off.
    /// </summary>
    public class TimerChain : TimerTask
    {
        private struct ChainStep
        {
            public float Delay;
            public Action Callback;
        }

        private readonly Queue<ChainStep> steps = new();
        private Action currentCallback;
        private bool started;
        private bool finished;

        // We don't use the base class's duration/callback fields directly,
        // TimerChain manages its own step queue instead.
        public TimerChain() : base(0f, null) { }

        /// <summary>Queue a pause with no callback.</summary>
        public TimerChain Wait(float seconds)
        {
            steps.Enqueue(new ChainStep { Delay = seconds, Callback = null });
            return this;
        }

        /// <summary>Queue a callback that fires as soon as the previous step finishes.</summary>
        public TimerChain Then(Action callback)
        {
            steps.Enqueue(new ChainStep { Delay = 0f, Callback = callback });
            return this;
        }

        /// <summary>Queue a delay followed by a callback (Wait + Then combined).</summary>
        public TimerChain After(float seconds, Action callback)
        {
            steps.Enqueue(new ChainStep { Delay = seconds, Callback = callback });
            return this;
        }

        /// <summary>Repeats everything queued so far an additional `times` times.</summary>
        public TimerChain Repeat(int times)
        {
            var snapshot = steps.ToArray();
            for (int i = 0; i < times; i++)
                foreach (var step in snapshot)
                    steps.Enqueue(step);
            return this;
        }

        /// <summary>Begins running the chain. Call this last.</summary>
        public TimerChain Start()
        {
            if (started) return this;
            started = true;
            AdvanceToNextStep();
            return this;
        }

        private void AdvanceToNextStep()
        {
            if (steps.Count == 0)
            {
                finished = true;
                return;
            }

            var step = steps.Dequeue();
            remaining = step.Delay;
            currentCallback = step.Callback;

            // Zero-delay steps (Then) fire immediately and cascade into
            // the next step within the same Update call.
            if (remaining <= 0f)
            {
                currentCallback?.Invoke();
                AdvanceToNextStep();
            }
        }

        public override bool Update(float dt)
        {
            if (!started) return false;
            if (finished) return true;

            remaining -= dt;

            if (remaining <= 0f)
            {
                currentCallback?.Invoke();
                AdvanceToNextStep();
                return finished;
            }

            return false;
        }
    }
}
