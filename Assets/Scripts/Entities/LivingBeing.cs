using System.Collections;
using System.Collections.Generic;
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

        public virtual void Awake()
        {
            float startingAngle = Random.Range(-180, 180);
            transform.rotation = Quaternion.Euler(0, startingAngle, 0);

            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, 0, pos.z);
        }

        public virtual void Die()
        {
            Destroy(gameObject);
        }

        public virtual void BeingEaten()
        {

        }
    }
}

