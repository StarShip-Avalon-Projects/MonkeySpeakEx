using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Monkeyspeak.Libraries
{
    /// <summary>
    /// Reciporcating Timers for Monkey Speak
    /// </summary>
    public class Timers : AbstractBaseLibrary
    {
        #region Private Fields

        private static object lck = new object();
        private static Dictionary<double, TimerInfo> timers = new Dictionary<double, TimerInfo>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Default Timer Library. Call static method Timers.DestroyTimers()
        /// when your application closes.
        /// </summary>
        public Timers()
        {
            // (0:300) When timer # goes off,
            Add(new Trigger(TriggerCategory.Cause, 300), WhenTimerGoesOff,
                "(0:300) When timer # goes off,");

            Add(new Trigger(TriggerCategory.Cause, 301), WhenAnyTimerGoesOff,
                "(0:301) When any timer goes off,");

            // (1:300) and timer # is running,
            Add(new Trigger(TriggerCategory.Condition, 300), AndTimerIsRunning,
                "(1:300) and timer # is running,");
            // (1:301) and timer # is not running,
            Add(new Trigger(TriggerCategory.Condition, 301), AndTimerIsNotRunning,
                "(1:301) and timer # is not running,");

            // (5:300) create timer # to go off every # second(s).
            Add(new Trigger(TriggerCategory.Effect, 300), CreateTimer,
                "(5:300) create timer # to go off every # second(s).");

            // (5:301) stop timer #.
            Add(new Trigger(TriggerCategory.Effect, 301), StopTimer,
                "(5:301) stop timer #.");
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Closes and removes all Timers
        /// </summary>
        public static void DestroyTimers()
        {
            List<double> keys = new List<double>(timers.Keys);
            lock (lck)
            {
                foreach (double key in keys)
                {
                    /*
                     * MSDN A Dictionary<TKey, TValue> can support multiple readers
                     * concurrently, as long as the collection is not modified. Even
                     * so, enumerating through a collection is intrinsically not a
                     * thread-safe procedure. In the rare case where an enumeration
                     * contends with write accesses, the collection must be locked
                     * during the entire enumeration. To allow the collection to be
                     * accessed by multiple threads for reading and writing, you must
                     * implement your own synchronization.
                     */

                    if (timers[key].Timer != null)
                        timers[key].Timer.Dispose();
                    timers.Remove(key);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// (1:301) and timer # is not running,
        /// </summary>
        /// <param name="reader">
        /// </param>
        /// <returns>
        /// </returns>
        private bool AndTimerIsNotRunning(TriggerReader reader)
        {
            bool test = AndTimerIsRunning(reader) == false;
            return test;
        }

        /// <summary>
        /// (1:300) and timer # is running,"
        /// </summary>
        /// <param name="reader">
        /// </param>
        /// <returns>
        /// </returns>
        private bool AndTimerIsRunning(TriggerReader reader)
        {
            TimerInfo timerInfo;
            if (TryGetTimerFrom(reader, out timerInfo) == false)
                return false;
            bool test = timerInfo.Timer != null;

            return test;
        }

        /// <summary>
        /// (5:300) create timer # to go off every # second(s).
        /// </summary>
        /// <param name="reader">
        /// </param>
        /// <returns>
        /// </returns>
        private bool CreateTimer(TriggerReader reader)
        {
            TimerInfo timerInfo = new TimerInfo();
            double idx = new double();
            if (reader.PeekVariable())
            {
                Variable var = reader.ReadVariable();
                if (var.Value != null && var.Value is double)
                    idx = (double)var.Value;
            }
            else if (reader.PeekNumber())
            {
                idx = reader.ReadNumber();
            }

            double num = double.NaN;
            if (reader.PeekVariable())
            {
                Variable var = reader.ReadVariable();
                if (var.Value.GetType() == typeof(double))
                    num = Convert.ToDouble(var.Value);
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            if (double.IsNaN(num)) return false;

            lock (lck)
            {
                if (timerInfo.Timer != null) timerInfo.Timer.Dispose();
                timerInfo = new TimerInfo(reader.Page, num, idx);
                timerInfo.Timer.InitializeLifetimeService();
                if (timers.Keys.Contains(idx) == true)
                {
                    Console.WriteLine("New Timer Disposing old timer " + idx.ToString());
                    timers[idx].Timer.Dispose();
                    timers[idx] = timerInfo;
                }
                else
                    timers.Add(idx, timerInfo);
            }

            return true;
        }

        /// <summary>
        /// (5:301) stop timer #.
        /// </summary>
        /// <param name="reader">
        /// </param>
        /// <returns>
        /// </returns>
        private bool StopTimer(TriggerReader reader)
        {
            // Does NOT destroy the Timer.
            //Now it Does! TryGetTimerFrom(reader) uses Dictionary.ContainsKey
            TimerInfo timerInfo = new TimerInfo();
            double num = 0;
            if (reader.PeekVariable())
            {
                Variable var = reader.ReadVariable();
                if (Double.TryParse(var.Value.ToString(), out num) == false)
                    num = 0;
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }
            try
            {
                lock (lck)
                {
                    if (timers.ContainsKey(num))
                    {
                        System.Diagnostics.Debug.Print("Stop Timer " + num.ToString());
                        timers[num].Timer.Dispose();
                        timers.Remove(num);
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
            return true;
        }

        private bool TryGetTimerFrom(TriggerReader reader, out TimerInfo timerInfo)
        {
            double num = double.NaN;
            if (reader.PeekVariable())
            {
                Variable var = reader.ReadVariable();
                num = 0;
                double.TryParse(var.Value.ToString(), out num);

            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            if (double.IsNaN(num) == false)
            {
                
                if (timers.ContainsKey(num) == false)
                {
                    // Don't add a timer to the Dictionary if it don't
                    // exist. Just return a blank timer
                    // - Gerolkae
                    timerInfo = new TimerInfo();
                    return false;
                }
                else
                {
                    timerInfo = timers[num];
                    return true;
                }
            }
            timerInfo = null;
            return false;
        }

        /// <summary>
        /// (0:301) When any timer goes off,
        /// </summary>
        /// <param name="reader">
        /// <see cref="TriggerReader"/>
        /// </param>
        /// <returns>
        /// when any timer goes off
        /// </returns>
        private bool WhenAnyTimerGoesOff(TriggerReader reader)
        {
            return true;
        }

        /// <summary>
        /// (0:300) When timer # goes off,
        /// </summary>
        /// <param name="reader">
        /// <see cref="TriggerReader"/>
        /// </param>
        /// <returns>
        /// when timer # goes off
        /// </returns>
        private bool WhenTimerGoesOff(TriggerReader reader)
        {
            TimerInfo timerInfo = null;
            lock (lck)
            {
                if (TryGetTimerFrom(reader, out timerInfo) == true)
                {
                    // Make sure only the Current Timer triggers
                    // ~Gerolkae
                    //System.Diagnostics.Debug.Print(timerInfo.ID.ToString() + "Timer Triggered");
                    return timerInfo == TimerInfo.CurrentTimer;
                }

                return false;
            }
        }

        #endregion Private Methods
    }

    /// <summary>
    /// A TimerInfo object contains Timer and Page Owner. Timer is not
    /// started from a TimerInfo constructor.
    /// </summary>
    internal class TimerInfo
    {
        #region Public Fields

        public static TimerInfo CurrentTimer;

        #endregion Public Fields

        #region Private Fields

        private object _timerLock = new object();
        private double id;
        private double interval = 0;
        private object lck = new object();
        private Page owner;
        private Timer timer;

        #endregion Private Fields

        #region Public Constructors

        public TimerInfo()
        {
            timer = new Timer(Timer_Elapsed);
        }

        /// <summary>
        /// T
        /// </summary>
        /// <param name="owner">
        /// Page owner
        /// </param>
        /// <param name="interval">
        /// cycle time in seconds
        /// </param>
        /// <param name="Id">
        /// MonkeySpeak ID if the timer
        /// </param>
        public TimerInfo(Page owner, double interval, double Id)
        {
            id = Id;
            this.owner = owner;
            this.interval = interval;
            timer = new Timer(Timer_Elapsed, this, TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(interval));
        }

        #endregion Public Constructors

        #region Public Properties

        public double ID
        {
            get { return id; }
            set { id = value; }
        }

        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public Page Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public Timer Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        private void Timer_Elapsed(object sender)
        {
            // Lets Capture the Current Triggering Timer ~Gerolkae
            if (!Monitor.TryEnter(_timerLock))
            {
                // something has the lock. Probably shutting down.
                return;
            }
            try
            {
                CurrentTimer = (TimerInfo)sender;
                owner.Execute(300, 301);
                CurrentTimer = null;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.Print(Ex.Message);
            }
            finally
            {
                Monitor.Exit(_timerLock);
            }
        }

        #endregion Private Methods

        public static bool operator ==(TimerInfo timer1, TimerInfo timer2)
        {
            if (ReferenceEquals(timer1, null))
            {
                return ReferenceEquals(timer2, null);
            }

            return timer1.id == timer2.id;
        }

        public static bool operator !=(TimerInfo timer1, TimerInfo timer2)
        {
            if (!ReferenceEquals(timer1, null))
            {
                return !ReferenceEquals(timer2, null);
            }

             return timer1.id != timer2.id;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;
            TimerInfo o = (TimerInfo)obj;
            return ID == o.id;
        }
        public override int GetHashCode()
        {
            return (int)id;
        }
    }
}