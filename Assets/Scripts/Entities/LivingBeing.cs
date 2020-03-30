using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Abstract class shared by all living entities. It allows these entities to spawn and to die.
    /// It inherits from monobehaviour so that we can attach the species 
    /// (e.g. livingBeing -> animal -> rabbit) to an object.
    /// </summary>
    public abstract class LivingBeing: MonoBehaviour
    {
        public float GroundYPos { get; protected set; }

        public void Awake()
        {
            Spawn();
        }

        public virtual void Spawn()
        {
            float startingAngle = Random.Range(-180, 180);
            transform.rotation = Quaternion.Euler(0, startingAngle, 0);
            GroundYPos = 0.5f;
            transform.position += Vector3.up * (GroundYPos + 0.5f * transform.localScale.y - transform.position.y);
        }

        public virtual void Die()
        {
            Destroy(gameObject);
        }

        public virtual void BeingEaten() { } 
    }
}

