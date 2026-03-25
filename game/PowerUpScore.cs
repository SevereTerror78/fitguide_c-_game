using System;
using System.Drawing;

namespace game
{
    /// <summary>
    /// Power-up that gives +10 points when hit.
    /// </summary>
    public class PowerUpScore : PowerDownBase
    {
        /// <summary>
        /// Power-up that gives +10 points when hit.
        /// </summary>
        public PowerUpScore(Form3 form) : base(form)
        {
            button.Text = "+10p";
            // make it visually distinct
            try { button.BackColor = Color.Green; } catch { }
        }

        protected override void OnHit()
        {
            int score = form.Engine.GetElapsedTime();
            int newScore = score + 10;
            form.Engine.SetElapsedTime(newScore);
        }
    }
}
