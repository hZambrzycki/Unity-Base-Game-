using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float experiencePoints = 0;

        public Action onExperienceGained;

        private void Update() 
        {
            if(Input.GetKey(KeyCode.E))
            {
                GainExperience(Time.deltaTime*1000);
            }    
        }
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }

        public float GetPoints()
        {
            return experiencePoints;
        }
    }
}
