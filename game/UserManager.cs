using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Alias to avoid conflict with Org.BouncyCastle's BCrypt type
using BCryptNet = BCrypt.Net.BCrypt;


namespace game
{
    public class UserManager
    {
        public int CurrentUserId { get; private set; } = -1;
        public string CurrentUserName { get; private set; } = "";
        public string CurrentUserEmail { get; private set; } = "";

        private readonly string LastLoginPath = Path.Combine(Application.StartupPath, "last_login.txt");

        public bool IsLoggedIn => CurrentUserId > 0;

        /// <summary>
        /// Logs out the current user by resetting stored user id, name and email.
        /// </summary>
        public void Logout()
        {
            CurrentUserId = -1;
            CurrentUserName = "";
            CurrentUserEmail = "";
        }

        /// <summary>
        /// Loads the last saved email from the last_login file. Returns empty string if none.
        /// </summary>
        /// <returns>Last saved email or empty string.</returns>
        public string LoadLastEmail()
        {
            try
            {
                if (File.Exists(LastLoginPath))
                    return File.ReadAllText(LastLoginPath).Trim();
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Saves the provided email to the last_login file. If email is empty, deletes the file.
        /// </summary>
        /// <param name="email">Email to save or empty to clear.</param>
        public void SaveLastEmail(string email)
        {
            try
            {
                // do not create an empty file; if email is empty remove existing file
                if (string.IsNullOrWhiteSpace(email))
                {
                    try { if (File.Exists(LastLoginPath)) File.Delete(LastLoginPath); } catch { }
                }
                else
                {
                    File.WriteAllText(LastLoginPath, email);
                }
            }
            catch { }
        }
        /// <summary>
        /// Attempts to authenticate a user by email and password.
        /// Supports plain-text legacy passwords and bcrypt hashed passwords.
        /// </summary>
        /// <param name="email">User email to lookup.</param>
        /// <param name="password">Password provided by user.</param>
        /// <param name="failureMessage">If login fails, receives a short failure reason.</param>
        /// <returns>True if authentication succeeded; otherwise false.</returns>
        public bool TryLogin(string email, string password, out string failureMessage)
        {
            failureMessage = null;
            try
            {
                var row = DatabaseHelper.GetUserByEmail(email);
                if (row == null)
                {
                    failureMessage = "Email not found.";
                    return false;
                }

                var (id, name, dbPassword) = row.Value;
                bool ok = false;

                // plain equality (legacy plain-text accounts)
                if (dbPassword == password) ok = true;
                // bcrypt verification (modern, secure)
                else
                {
                    try
                    {
                        ok = BCryptNet.Verify(password, dbPassword);
                    }
                    catch { ok = false; }
                }

                if (!ok)
                {
                    failureMessage = "Incorrect password.";
                    return false;
                }

                CurrentUserId = id;
                CurrentUserName = name;
                CurrentUserEmail = email;
                // Do NOT automatically save last email here. UI decides when to remember.
                return true;
            }
            catch (Exception ex)
            {
                failureMessage = "Error while logging in: " + ex.Message;
                return false;
            }
        }
    }
}
