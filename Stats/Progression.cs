using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
        public class Progression : ScriptableObject 
        {
            [SerializeField] ProgressionCharacterClass[] characterClasses = null;

            Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookUpTable = null;

        public float GetStat(Stat stat,CharacterClass characterClass, int level)
        {
            BuildLookUp();

            if (!lookUpTable[characterClass].ContainsKey(stat))
            {
                return 0;
            }


            float[] levels = lookUpTable[characterClass][stat];

            if (levels.Length == 0)
            {
                return 0;
            }
            if (levels.Length < level)
            {
                return levels[levels.Length - 1];
            }
            return levels[level -1];

        }
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookUp();
            float[] levels = lookUpTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookUp()
        {
            if (lookUpTable != null) return;

            lookUpTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookUpTable[progressionClass.characterClass] = statLookupTable;

            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;

        }
        }
}