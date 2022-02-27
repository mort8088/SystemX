using System;

namespace SystemX.HighScore {
    [Serializable]
    public struct HighScoreRec {
        public DateTime Date;
        public string Name;
        public int Score;

        public string DataAsString() {
            return string.Format("{0},{1},{2}", Name, Score, Date);
        }
    }
}