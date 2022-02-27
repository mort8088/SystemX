// -----------------------------------------------------------------------
// <copyright file="HighScoreManager.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using SystemX.Helpers;
//using GameJoltAPI;

namespace SystemX.HighScore
{
    public enum LeaderboardType { Local, Remote }

    public static class HighScoreManager
    {
        private static Timer UpdateRemote;
        private static string _localFilePath;
        private static string _TableID;

        public static List<HighScoreRec> LocalHighScores { get; set; }
        public static List<HighScoreRec> RemoteHighScores { get; set; }

        public static void Initialize(string tableID)
        {

            UpdateRemote = new System.Timers.Timer();
            UpdateRemote.Elapsed += new ElapsedEventHandler(TimerTrigger);
            UpdateRemote.Interval = 5000;
            UpdateRemote.Enabled = true;

            _TableID = tableID;
            _localFilePath = Path.Combine(PathHelper.RootDirectory, @"Score.dat");
            LocalHighScores = new List<HighScoreRec>();
            RemoteHighScores = new List<HighScoreRec>();

            try
            {
                using (Stream stream = File.Open(_localFilePath, FileMode.Open))
                {
                    if (stream.Length > 0)
                    {
                        BinaryFormatter bin = new BinaryFormatter();

                        LocalHighScores = (List<HighScoreRec>)bin.Deserialize(stream);
                    }
                }
            }
            catch (IOException) { }

            UpdateRemoteScores();
        }

        private static void TimerTrigger(object sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO: Add GameJolt API
            //if (Settings.AllowAPICalls)
            //    UpdateRemoteScores();

        }

        public static void UpdateRemoteScores()
        {
            // TODO: Add GameJolt API
            /*
            ScoresClient sc = new ScoresClient();
            sc.OnFetchComplete += (sender, eventArgs) =>
            {
                if ((eventArgs.Scores == null) || (eventArgs.Scores.Count <= 0)) return;

                RemoteHighScores.Clear();

                foreach (HighScoreRec tmp in eventArgs.Scores.Select(itemScore => new HighScoreRec
                                                                                  {
                                                                                      Score = int.Parse(itemScore.Sort),
                                                                                      Name = itemScore.User,
                                                                                      Date = DateTime.MinValue//.Parse(itemScore.Stored)
                                                                                  }))
                {
                    RemoteHighScores.Add(tmp);
                }
            };
            sc.FetchScores();
            */
            SortRecs();
        }

        public static void Close()
        {
            if (LocalHighScores.Count == 0) return;

            SortRecs();

            try
            {
                using (Stream stream = File.Open(_localFilePath, FileMode.OpenOrCreate))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, LocalHighScores);
                }
            }
            catch (IOException) { }
        }

        public static void AddScore(string name, int score)
        {
            // TODO: Add GameJolt API
            /*
            ScoresClient sc = new ScoresClient();
            sc.AddScores("", score, _TableID);
            */
            LocalHighScores.Add(new HighScoreRec
            {
                Name = name,
                Date = DateTime.Now,
                Score = score
            });

            SortRecs();
        }

        public static void AddScore(int score)
        {
            // TODO: Add GameJolt API
            /*
            AddScore(Settings.Username, score);
            */
        }

        private static void SortRecs()
        {
            LocalHighScores.Sort((s1, s2) => s2.Score.CompareTo(s1.Score));
            RemoteHighScores.Sort((s1, s2) => s2.Score.CompareTo(s1.Score));
        }

        public static HighScoreRec[] GetTop10(LeaderboardType from)
        {
            HighScoreRec[] returnValue = new HighScoreRec[10];

            SortRecs();

            List<HighScoreRec> fromTable;

            switch (from)
            {
                case LeaderboardType.Local:
                    fromTable = LocalHighScores;
                    break;
                case LeaderboardType.Remote:
                    fromTable = RemoteHighScores;
                    break;
                default:
                    fromTable = LocalHighScores;
                    break;
            }

            int i = 0;
            foreach (HighScoreRec highScoreRec in fromTable)
            {
                returnValue[i++] = highScoreRec;

                if (i == 10)
                    break;
            }

            return returnValue;
        }
    }
}