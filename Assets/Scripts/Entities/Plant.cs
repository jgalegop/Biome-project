using UnityEngine;
using System.Collections;


namespace Entities
{
    public class Plant : LivingBeing
    {
        private readonly float decaySpeed = 1f;

        // energy that provides? 
        public override void BeingEaten()
        {
            Vector3 currentScale = transform.localScale;
            transform.localScale = currentScale - Vector3.one * decaySpeed * Time.deltaTime;
            transform.position += Vector3.up * (GroundYPos + 0.5f * transform.localScale.y - transform.position.y);
            if (transform.localScale.x < 0 ||
                transform.localScale.y < 0 ||
                transform.localScale.z < 0)
            {
                Die();
            }
        }

        public override void Spawn()
        {
            base.Spawn();
            StatisticsManager.PlantCreated();
        }

        public override void Die()
        {
            base.Die();
            StatisticsManager.PlantDestroyed();
        }
    }
}

