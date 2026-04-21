using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace game
{
    public abstract class PowerDownBase
    {
        protected Form3 form;
        protected Button button;
        protected Timer lifeTimer;
        protected Random rnd = new Random();

        public bool Active { get; private set; } = false;

        /// <summary>
        /// Initializes a new power item and attaches its button to the provided form.
        /// </summary>
        /// <param name="form">The Form3 host where the button will be shown.</param>
        public PowerDownBase(Form3 form)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));

            button = new Button();
            button.Size = new Size(60, 40);
            button.BackColor = Color.DarkRed;
            button.ForeColor = Color.White;
            button.Visible = false;
            // generic tag for both power-ups and power-downs
            button.Tag = "PowerItem";
            form.Controls.Add(button);
            button.BringToFront();
        }

        public virtual void Spawn()
        {
            if (Active) return;
            Active = true;

            PlaceRandomly();

            if (button != null)
                button.Visible = true;

            lifeTimer = new Timer();
            lifeTimer.Interval = rnd.Next(10000, 15000);
            // Only destroy when the game is not paused. Use the host form's engine state to check.
            lifeTimer.Tick += (s, e) =>
            {
                try
                {
                    if (form?.Engine != null && form.Engine.IsPaused)
                        return;
                }
                catch { }
                Destroy();
            };
            lifeTimer.Start();
        }

        /// <summary>
        /// Shows the power item at a random position on the form and starts a lifetime timer.
        /// </summary>

        private void PlaceRandomly()
        {
            int x, y;

            do
            {
                x = rnd.Next(0, form.ClientSize.Width - button.Width);
                y = rnd.Next(0, form.ClientSize.Height - button.Height);

                var rect = new Rectangle(x, y, button.Width, button.Height);

                // avoid player
                if (rect.IntersectsWith(form.PlayerBounds))
                    continue;

                // avoid other power item buttons
                bool intersectsOther = form.Controls.OfType<Control>()
                    .Where(c => !ReferenceEquals(c, button) && c.Tag != null && c.Tag.Equals("PowerItem"))
                    .Any(c => rect.IntersectsWith(c.Bounds));

                if (intersectsOther)
                    continue;

                break;
            }
            while (true);

            button.Left = x;
            button.Top = y;
        }

        public void CheckBulletCollision(Panel bullet, Timer bulletTimer)
        {
            if (!Active) return;
            if (button == null || bullet == null) return;

            if (bullet.Bounds.IntersectsWith(button.Bounds))
            {
                // consume bullet immediately to avoid multiple triggers or UI blocking by OnHit
                try
                {
                    if (bulletTimer != null)
                    {
                        bulletTimer.Stop();
                        bulletTimer.Dispose();
                    }
                }
                catch { }

                try
                {
                    bullet.Visible = false;
                }
                catch { }

                // perform effect and remove this power item
                OnHit();
                Destroy();

                try { bullet.Dispose(); } catch { }
            }
        }


        /// <summary>
        /// Called when this item is hit by a bullet. Implement the effect in derived classes.
        /// </summary>
        protected abstract void OnHit();

        public void Destroy()
        {
            if (!Active) return;
            Active = false;

            if (lifeTimer != null)
            {
                lifeTimer.Stop();
                lifeTimer.Dispose();
                lifeTimer = null;
            }

            if (button != null)
            {
                try
                {
                    form.Controls.Remove(button);
                }
                catch { }
                button.Visible = false;
                button.Dispose();
                button = null;
            }
        }
    }
}