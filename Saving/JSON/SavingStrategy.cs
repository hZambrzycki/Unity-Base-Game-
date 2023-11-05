using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;


namespace RPG.Saving
{
    public abstract class SavingStrategy : ScriptableObject
    {
        public abstract void SaveToFile(string saveFile, JObject State);

        public abstract JObject LoadFromFile(string saveFile);
        public abstract string GetExtension();

        public string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + GetExtension());
        }
    }
}
