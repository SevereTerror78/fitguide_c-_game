using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace game
{
    public class ScoreManager
    {
        private static readonly string DateFormat = "yyyy-MM-dd HH:mm:ss";

        public string GetScoreFilePathForUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) username = "anonymous";
            string fileName = username + "_scores.txt";
            return Path.Combine(Application.StartupPath, fileName);
        }

        /// <summary>
        /// Appends a score entry with timestamp to the user's score file.
        /// </summary>
        /// <param name="username">Username to store score for.</param>
        /// <param name="score">Score value to append.</param>
        public void AppendScore(string username, int score)
        {
            try
            {
                string path = GetScoreFilePathForUser(username);
                string line = $"{score};{DateTime.Now.ToString(DateFormat, CultureInfo.InvariantCulture)}";
                File.AppendAllLines(path, new[] { line });
            }
            catch
            {
            }
        }

        public List<(int score, DateTime time)> ReadAllScores(string username)
        {
            try
            {
                string path = GetScoreFilePathForUser(username);
                if (!File.Exists(path)) return new List<(int, DateTime)>();

                var lines = File.ReadAllLines(path);
                var list = new List<(int, DateTime)>();
                foreach (var l in lines)
                {
                    if (string.IsNullOrWhiteSpace(l)) continue;
                    var parts = l.Split(';');
                    if (parts.Length >= 1 && int.TryParse(parts[0], out int s))
                    {
                        DateTime dt = DateTime.MinValue;
                        if (parts.Length >= 2)
                            DateTime.TryParseExact(parts[1], DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);
                        list.Add((s, dt));
                    }
                }
                return list;
            }
            catch
            {
                return new List<(int, DateTime)>();
            }
        }

        public int GetHighScore(string username)
        {
            var list = ReadAllScores(username);
            return list.Count == 0 ? 0 : list.Max(x => x.score);
        }

        public int GetLastScore(string username)
        {
            var list = ReadAllScores(username);
            return list.Count == 0 ? 0 : list.Last().score;
        }

        public void SyncToDatabaseForUser(string username, int userId)
        {
            try
            {
                var list = ReadAllScores(username);
                if (list.Count == 0) return;
                int last = list.Last().score;
                int high = list.Max(x => x.score);
                DatabaseHelper.UpsertGameScores(userId, last, high);
            }
            catch
            {
            }
        }
    }
}