using NUnit.Framework;
using game;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

// code to run the test: dotnet test tests\Game.Tests\Game.Tests.csproj

namespace Game.Tests
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class PowerItemTests
    {
        private class DummyForm : Form { }

        [Test]
        public void PowerUpScore_IncreasesScore()
        {
            // create a real Form3 instance (requires STA)
            var form = (Form3)System.Activator.CreateInstance(typeof(Form3), new object[] { "testuser", new UserManager(), new ScoreManager() });

            try
            {
                // create power-up and spawn
                var pu = new PowerUpScore(form);
                pu.Spawn();

                // ensure engine has a known base score
                form.Engine.SetElapsedTime(10);

                // find the spawned button on the form and create a bullet overlapping it
                Button btn = null;
                foreach (Control c in form.Controls)
                {
                    var b = c as Button;
                    if (b != null && b.Tag != null && b.Tag.Equals("PowerItem") && b.Text == "+10p") { btn = b; break; }
                }
                Assert.IsNotNull(btn, "PowerUp button not found on form after spawn.");

                Panel bullet = new Panel { Size = new Size(10, 10), BackColor = Color.Red, Location = new Point(btn.Left + 1, btn.Top + 1) };
                var movementTimer = new System.Windows.Forms.Timer { Interval = 15 };

                // trigger collision
                pu.CheckBulletCollision(bullet, movementTimer);

                // verify score increased by 10
                Assert.AreEqual(20, form.Engine.GetElapsedTime());
            }
            finally
            {
                form?.Engine?.Stop();
                try { form?.Close(); } catch { }
            }
        }
    }
}
