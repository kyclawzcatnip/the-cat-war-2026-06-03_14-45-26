using UnityEngine;

namespace CatWar
{
    public class Projectile : MonoBehaviour
    {
        private Vector3 direction;
        private float speed = 10f;
        private float damage = 10f;
        private LayerMask hitMask;

        public void Setup(Vector3 startPos, Vector3 targetDir, float projectileSpeed, float projDamage, LayerMask mask)
        {
            transform.position = startPos;
            direction = targetDir.normalized;
            speed = projectileSpeed;
            damage = projDamage;
            hitMask = mask;

            // Rotate sprite towards direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Clean up if it flies off-screen
            if (Mathf.Abs(transform.position.x) > 30f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if collider is in the target layer mask
            if ((hitMask.value & (1 << collision.gameObject.layer)) != 0)
            {
                // Deal damage to unit
                Unit unit = collision.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.TakeDamage(damage);
                }
                else
                {
                    // Or deal damage to base
                    Base baseObj = collision.GetComponent<Base>();
                    if (baseObj != null)
                    {
                        baseObj.TakeDamage(damage);
                    }
                }

                // Self destruct
                Destroy(gameObject);
            }
        }
    }
}
