using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest;
        List<string> completedObjectives = new List<string>();

        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }
        }
        public QuestStatus(JToken objectState)
        {
            if (objectState is JObject state)
            {
                IDictionary<string, JToken> stateDict = state;
                quest = Quest.GetByName(stateDict["questName"].ToObject<string>());
                completedObjectives.Clear();
                if (stateDict["completedObjectives"] is JArray completedState)
                {
                    IList<JToken> completedStateArray = completedState;
                    foreach (JToken objective in completedStateArray)
                    {
                        completedObjectives.Add(objective.ToObject<string>());
                    }
                }
            }
        }
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["questName"] = quest.name;
            JArray completedState = new JArray();
            IList<JToken> completedStateArray = completedState;
            foreach (string objective in completedObjectives)
            {
                completedStateArray.Add(JToken.FromObject(objective));
            }
            stateDict["completedObjectives"] = completedState;
            return state;
        }



    }
}
