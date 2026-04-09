using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using game;

namespace Game.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class FormTests
    {
        [Test]
        public void Form1_ShouldContainMenuPanel_AfterConstruction()
        {
            using (var f = new Form1())
            {
                var menu = f.Controls.Find("menu", true).FirstOrDefault();
                Assert.IsNotNull(menu, "Menu control should exist on Form1 after construction");
                Assert.IsTrue(menu.Visible, "Menu control should be visible");
            }
        }

        [Test]
        public void Form2_ShouldCreateEmailAndPasswordFields()
        {
            var um = new UserManager();
            using (var f = new Form2(um))
            {
                var email = f.Controls.Find("email", true).FirstOrDefault() as TextBox;
                var password = f.Controls.Find("password", true).FirstOrDefault() as TextBox;

                Assert.IsNotNull(email, "Email textbox should be present in Form2");
                Assert.IsNotNull(password, "Password textbox should be present in Form2");
                Assert.IsTrue(password.UseSystemPasswordChar, "Password textbox should mask input");
            }
        }
    }
}
