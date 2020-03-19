using UnityEngine;


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
            if (transform.localScale.x < 0 ||
                transform.localScale.y < 0 ||
                transform.localScale.z < 0)
            {
                Die();
            }
        }
    }
}

