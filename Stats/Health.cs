using RPG.Core;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Stats
{
  public class Health : MonoBehaviour, IJsonSaveable
  {
    [SerializeField] float regenerationPercentage = 70;
    [SerializeField] TakeDamageEvent takeDamage;
    public UnityEvent onDie;

    [System.Serializable]
    public class TakeDamageEvent : UnityEvent<float>
    {
    }

    LazyValue<float> healthPoints;

    bool wasDeadLastFrame = false;

    private void Awake() 
    {
      healthPoints = new LazyValue<float>(GetInitialHealth);
    }
    private void Start()
    {
      healthPoints.ForceInit();
    }
    private void OnEnable() 
    {
      GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
    }
    private void OnDisable()
    {
      GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
    }
    private float GetInitialHealth()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }
    private void RegenerateHealth()
    {
     float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
     healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
    }
    public bool IsDead()
    {
      return healthPoints.value <= 0;
    }
    public void TakeDamage(GameObject instigator, float damage)
    {
      print(gameObject.name + "took damage: " + damage);
      healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

      if (IsDead())
      {
          onDie.Invoke();
          AwardExperience(instigator);
        }
      else 
      {
        takeDamage.Invoke(damage);
      }
      UpdateState();
    }
    public void Heal(float healthToRestore)
    {
      healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
    }
    
    public float GetHealthPoints()
    {
      return healthPoints.value;
    }
    public float GetMaxHealthPoints()
    {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }
    public float GetPercentage()
    {
      return 100 * GetFraction();
    }
    public float GetFraction()
    {
     return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
    }
    private void UpdateState()
    {
      Animator animator = GetComponent<Animator>();
      if (!wasDeadLastFrame && IsDead())
      {
        animator.SetTrigger("die");
        GetComponent<ActionScheduler>().CancelCurrentAction();
      }
      if (wasDeadLastFrame && !IsDead())
      {
        animator.Rebind();
      }

      wasDeadLastFrame = IsDead();
    }
    private void AwardExperience(GameObject instigator)
    {
      Experience experience = instigator.GetComponent<Experience>();
      if(experience == null) return;
      UpdateState();
      experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
    }
   
    public JToken CaptureAsJToken()
    {
      return JToken.FromObject(healthPoints.value);
    }

    public void RestoreFromJToken(JToken state)
    {
      healthPoints.value = state.ToObject<float>();
    }
  }
}
