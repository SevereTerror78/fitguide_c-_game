using System;
using System.Drawing;
using System.Windows.Forms;

namespace game
{
    public class OverlayControl : Control
    {
        private Button playerButton;
        private PointF target = new PointF(0, 0);
        private Timer timer;
        public float BarrelLength { get; set; } = 150f;

        public OverlayControl(Button player)
        {
            playerButton = player;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.Dock = DockStyle.Fill;
            this.Enabled = true;

            timer = new Timer();
            timer.Interval = 15;
            timer.Tick += (s, e) =>
            {
                try
                {
                    var p = this.PointToClient(Cursor.Position);
                    target = new PointF(p.X, p.Y);
                    this.Invalidate();
                }
                catch { }
            };
            timer.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // Make control transparent to mouse events (click-through)
                const int WS_EX_TRANSPARENT = 0x20;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                if (playerButton == null || !playerButton.Visible) return;

                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float sx = playerButton.Left + playerButton.Width / 2f;
                float sy = playerButton.Top + playerButton.Height / 2f;

                float dx = target.X - sx;
                float dy = target.Y - sy;
                float len = (float)Math.Sqrt(dx * dx + dy * dy);
                if (len < 1) len = 1;
                float ex = sx + dx / len * BarrelLength;
                float ey = sy + dy / len * BarrelLength;

                using (var pen = new Pen(Color.Gray, 8))
                using (var penInner = new Pen(Color.DarkGray, 2))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    penInner.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    penInner.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawLine(pen, sx, sy, ex, ey);
                    g.DrawLine(penInner, sx, sy, ex, ey);
                }
            }
            catch { }
        }

        protected override void Dispose(bool disposing)
        {
            try { if (disposing) timer?.Stop(); } catch { }
            try { if (disposing) timer?.Dispose(); } catch { }
            base.Dispose(disposing);
        }
    }
}
