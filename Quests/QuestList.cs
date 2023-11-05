using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using GameDevTV.Inventories;
using RPG.Core;
namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, IJsonSaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>();

        public event Action onUpdate;

        private void Update()
        {
            CompleteObjectivesByPredicates();
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }
            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }
            return null;
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        private void CompleteObjectivesByPredicates()
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.IsComplete()) continue;
                Quest quest = status.GetQuest();
                foreach (var objective in quest.GetObjectives())
                {
                    if (status.IsObjectiveComplete(objective.reference)) continue;
                    if (!objective.usesCondition) continue;
                    if (objective.completionCondition.Check(GetComponents<IPredicateEvaluator>()))
                    {
                        CompleteObjective(quest, objective.reference);
                    }
                }
            }
        }

        

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
            }

            return null;
        }

        public JToken CaptureAsJToken()
        {
            JArray state = new JArray();
            IList<JToken> stateList = state;
            foreach (QuestStatus status in statuses)
            {
                stateList.Add(status.CaptureAsJToken());
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                statuses.Clear();
                IList<JToken> stateList = stateArray;
                foreach (JToken token in stateList)
                {
                    statuses.Add(new QuestStatus(token));
                }
            }
        }
    }

}
