using NUnit.Framework;
using System.Threading;
using game;
using System.Drawing;

namespace Game.Tests
{
    [TestFixture]
    public class GameEngineTests
    {
        private class DummyControl : System.Windows.Forms.Control { }

        [Test]
        public void Start_IncrementsElapsedTime()
        {
            var ctrl = new DummyControl();
            var engine = new GameEngine(ctrl);

            engine.Start();

            // wait a bit for elapsed timer to tick
            Thread.Sleep(1100);
            int t1 = engine.GetElapsedTime();
            Assert.GreaterOrEqual(t1, 1);

            engine.Stop();
        }

        [Test]
        public void ShrinkFrame_EndsWhenTooSmall()
        {
            var ctrl = new DummyControl();
            var engine = new GameEngine(ctrl);
            var rect = new Rectangle(0,0,101,176);
            bool ended = engine.ShrinkFrame(ref rect, 100, 175, 2);
            Assert.IsTrue(ended);
        }

        [Test]
        public void SetElapsedTime_UpdatesAndFiresEvent()
        {
            var ctrl = new DummyControl();
            var engine = new GameEngine(ctrl);
            int fired = -1;
            engine.OnElapsedTimeChanged += s => fired = s;

            engine.SetElapsedTime(42);
            Assert.AreEqual(42, engine.GetElapsedTime());
            Assert.AreEqual(42, fired);
        }
    }
}
