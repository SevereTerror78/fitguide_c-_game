using System;
using System.Drawing;
using System.Windows.Forms;

namespace game
{
    /// <summary>
    /// Power-up that gives +25 points when hit.
    /// </summary>
    public class PowerUpBig : PowerDownBase
    {
        /// <summary>
        /// Power-up that gives +25 points when hit.
        /// </summary>
        public PowerUpBig(Form3 form) : base(form)
        {
            button.Text = "+25p";
            try { button.BackColor = Color.Gold; } catch { }
        }

        protected override void OnHit()
        {
            int score = form.Engine.GetElapsedTime();
            int newScore = score + 25;
            form.Engine.SetElapsedTime(newScore);
            // silent effect; no popup
        }
    }
}
