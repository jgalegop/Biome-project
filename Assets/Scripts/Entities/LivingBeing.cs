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
            Debug.Log(gameObject.name + " spawned");

            float startingAngle = Random.Range(-180, 180);
            transform.rotation = Quaternion.Euler(0, startingAngle, 0);
            Debug.Log(gameObject.name + " rotated " + startingAngle);

            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, 1, pos.z);
        }

        public virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}

