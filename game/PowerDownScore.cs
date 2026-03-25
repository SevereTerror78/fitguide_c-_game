using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    /// <summary>
    /// Power-down that subtracts 10 points when hit.
    /// </summary>
    public class PowerDownScore : PowerDownBase
    {
        /// <summary>
        /// Power-down that subtracts 10 points when hit.
        /// </summary>
        public PowerDownScore(Form3 form) : base(form)
        {
            button.Text = "-10p";
        }

        protected override void OnHit()
        {
            int score = form.Engine.GetElapsedTime();
            int newScore = Math.Max(0, score - 10);
            form.Engine.SetElapsedTime(newScore);
        }

    }
}
