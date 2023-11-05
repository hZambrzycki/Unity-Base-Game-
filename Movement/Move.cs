using RPG.Core;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Stats;



namespace RPG.Movement
{
    public class Move : MonoBehaviour, IAction, ISaveable, IJsonSaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;
        
        NavMeshAgent navMeshAgent;
        Health health; 

        private void Awake() 
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if(health.IsDead())
            {
            navMeshAgent.enabled = !health.IsDead();
            }
            UpdateAnimator();
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed* Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }
        [System.Serializable]
        struct MoveSaveData
        {
           public Vector3 position;
           public Vector3 rotation;
        }
        public object CaptureState()
        {
            MoveSaveData data = new MoveSaveData();
           // data.position = new SerializableVector3(transform.position);
           // data.rotation = new SerializableVector3(transform.eulerAngles);
        
            return data; 
        }

        public void RestoreState(object state)
        {
            MoveSaveData data = (MoveSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
          
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(transform.position);
        }

        public void RestoreFromJToken(JToken state)
        {
            navMeshAgent.enabled = false;
            transform.position = state.ToObject<Vector3>();
            navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

    }
}

