using System;
using System.Drawing;
using System.Windows.Forms;

namespace game
{
    /// <summary>
    /// Power-down that sets player's score to zero when hit.
    /// </summary>
    public class PowerDownKill : PowerDownBase
    {
        /// <summary>
        /// Power-down that sets player's score to zero when hit.
        /// </summary>
        public PowerDownKill(Form3 form) : base(form)
        {
            button.Text = "Kill";
            try { button.BackColor = Color.Purple; } catch { }
        }

        protected override void OnHit()
        {
            // set elapsed time to 0
            form.Engine.SetElapsedTime(0);
            // silent effect; no popup
        }
    }
}
