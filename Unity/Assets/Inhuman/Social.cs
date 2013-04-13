using UnityEngine;
using System.Collections;

namespace Inhuman
{
    public class Achievement
    {
        public string Name;
        public string Id;
        public int Completed = 0;
    }

    public class Social
    {
        public Achievement[] Achievements;
        public void SetAchievement(string name, int value) { }
        public void GetAchievements() { }
        public void Restore() { }
    }
}