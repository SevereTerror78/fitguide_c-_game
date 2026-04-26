using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace game
{
    public class GameEngine
    {
        private readonly Control host;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Timer shootTimer;
        private readonly Random random = new Random();
        private bool ended = false;
        private bool paused = false;
        private int elapsedTime = 0;
        // multiplier to control frame shrink speed (1.0 = normal)
        private double shrinkMultiplier = 1.0;
        private readonly object shrinkLock = new object();

        /// <summary>
        /// Raised when elapsed time (score) changes. Subscribers receive the new seconds value.
        /// </summary>
        public event Action<int> OnElapsedTimeChanged;

        /// <summary>
        /// Raised when the game ends (frame shrank below minimum) and should stop.
        /// </summary>
        public event Action OnGameEnded;

        /// <summary>
        /// Raised when the engine requests a bullet to be shot. Provides origin and target points.
        /// </summary>
        public event Action<Point, Point> OnShootBullet;

        public GameEngine(Control hostForm)
        {
            host = hostForm ?? throw new ArgumentNullException(nameof(hostForm));
        }

        public void Start()
        {
            ended = false;
            elapsedTime = 0;
            StartElapsedTimer();
            StartShootTimer();
        }

        /// <summary>
        /// Pauses the engine timers without ending the game. Elapsed time is preserved.
        /// </summary>
        public void Pause()
        {
            paused = true;
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
                gameTimer = null;
            }

            if (shootTimer != null)
            {
                shootTimer.Stop();
                shootTimer.Dispose();
                shootTimer = null;
            }
        }

        /// <summary>
        /// Resumes the engine timers if the game hasn't ended.
        /// </summary>
        public void Resume()
        {
            paused = false;
            if (ended) return;
            // restart timers only if they are not running
            if (gameTimer == null) StartElapsedTimer();
            if (shootTimer == null) StartShootTimer();
        }

        /// <summary>
        /// Indicates whether the engine is currently paused.
        /// </summary>
        public bool IsPaused => paused;

        public void Stop()
        {
            ended = true;
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
                gameTimer = null;
            }
            if (shootTimer != null)
            {
                shootTimer.Stop();
                shootTimer.Dispose();
                shootTimer = null;
            }
        }

        private void StartElapsedTimer()
        {
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += (s, e) =>
            {
                if (ended) return;
                elapsedTime++;
                OnElapsedTimeChanged?.Invoke(elapsedTime);
            };
            gameTimer.Start();
        }

        private void StartShootTimer()
        {
            shootTimer = new System.Windows.Forms.Timer();
            shootTimer.Interval = random.Next(1000, 2001);
            shootTimer.Tick += (s, e) =>
            {
                if (ended) return;
                var center = new Point(host.ClientSize.Width / 2, host.ClientSize.Height / 2);
                var mouse = host.PointToClient(Cursor.Position);
                OnShootBullet?.Invoke(center, mouse);
                shootTimer.Interval = random.Next(1000, 2001);
            };
            shootTimer.Start();
        }

        public bool ShrinkFrame(ref Rectangle currentBounds, int minWidth = 100, int minHeight = 175, int shrinkAmount = 1)
        {
            if (ended) return true;

            int actualShrink = shrinkAmount;
            lock (shrinkLock)
            {
                try { actualShrink = Math.Max(1, (int)Math.Round(shrinkAmount * shrinkMultiplier)); } catch { actualShrink = shrinkAmount; }
            }

            int newWidth = currentBounds.Width - actualShrink;
            int newHeight = currentBounds.Height - actualShrink;

            // if either dimension reached minimum, end the game
            if (newWidth <= minWidth || newHeight <= minHeight)
            {
                ended = true;
                Stop();
                OnGameEnded?.Invoke();
                return true;
            }

            // if no effective shrink is possible (due to external constraints), also end the game
            if (newWidth >= currentBounds.Width || newHeight >= currentBounds.Height)
            {
                ended = true;
                Stop();
                OnGameEnded?.Invoke();
                return true;
            }

            int centerX = currentBounds.Left + currentBounds.Width / 2;
            int centerY = currentBounds.Top + currentBounds.Height / 2;

            int newLeft = centerX - newWidth / 2;
            int newTop = centerY - newHeight / 2;

            currentBounds = new Rectangle(newLeft, newTop, newWidth, newHeight);
            return false;
        }

        /// <summary>
        /// Temporarily adjusts the shrink multiplier for a duration (milliseconds).
        /// Multiplier > 1.0 makes the frame shrink faster; multiplier &lt; 1.0 slows shrinking.
        /// </summary>
        public void ApplyShrinkMultiplier(double multiplier, int durationMs)
        {
            if (multiplier <= 0) return;
            lock (shrinkLock)
            {
                shrinkMultiplier = multiplier;
            }

            // restore to 1.0 after duration
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(durationMs);
                }
                catch { }
                lock (shrinkLock)
                {
                    shrinkMultiplier = 1.0;
                }
            });
        }

        public int GetElapsedTime() => elapsedTime;
        public void SetElapsedTime(int seconds)
        {
            elapsedTime = Math.Max(0, seconds);
            OnElapsedTimeChanged?.Invoke(elapsedTime);
        }

    }
}