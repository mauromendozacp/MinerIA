using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IA.Boids
{
    public class Flocking : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Vector3 baseRotation = Vector3.zero;
        [SerializeField] private float maxSpeed = 0f;
        [SerializeField] private float maxForce = 0f;
        [SerializeField] private float checkRadious = 0f;

        [SerializeField] private float separationMultiplayer = 0f;
        [SerializeField] private float cohesionMultiplayer = 0f;
        [SerializeField] private float aligmentMultiplayer = 0f;
        #endregion

        #region PRIVATE_FIELDS
        private Vector2 velocity = Vector2.zero;
        private Vector2 aceleration = Vector2.zero;
        #endregion

        #region PROPERTIES
        private Vector2 Position
        {
            get
            {
                return gameObject.transform.position;
            }
            set
            {
                gameObject.transform.position = value;
            }
        }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
            velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        private void Update()
        {
            Collider2D[] otherColliders = Physics2D.OverlapCircleAll(Position, checkRadious);
            List<Flocking> boids = otherColliders.Select(others => others.GetComponent<Flocking>()).ToList();
            boids.Remove(this);

            if (boids.Any())
                aceleration = Alignment(boids) * aligmentMultiplayer + Separation(boids) * separationMultiplayer + Cohesion(boids) * cohesionMultiplayer;
            else
                aceleration = Vector2.zero;

            velocity += aceleration;
            velocity = LimitMagnitude(velocity, maxSpeed);
            Position += velocity * Time.deltaTime;

            float newAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, newAngle) + baseRotation);
        }
        #endregion

        #region PRIVATE_METHODS
        private Vector2 Alignment(IEnumerable<Flocking> boids)
        {
            Vector2 velocity = Vector2.zero;

            foreach (Flocking boid in boids)
            {
                velocity += boid.velocity;
            }
            velocity /= boids.Count();

            return Steer(velocity.normalized * maxSpeed);
        }

        private Vector2 Cohesion(IEnumerable<Flocking> boids)
        {
            Vector2 sumPositions = Vector2.zero;

            foreach (Flocking boid in boids)
            {
                sumPositions += boid.Position;
            }

            Vector2 average = sumPositions / boids.Count();
            Vector2 direction = average - Position;

            return Steer(direction.normalized * maxSpeed);
        }

        private Vector2 Separation(IEnumerable<Flocking> boids)
        {
            Vector2 direction = Vector2.zero;
            boids = boids.Where(o => DistanceTo(o) <= checkRadious / 2);

            foreach (var boid in boids)
            {
                Vector2 difference = Position - boid.Position;
                direction += difference.normalized / difference.magnitude;
            }
            direction /= boids.Count();

            return Steer(direction.normalized * maxSpeed);
        }

        private Vector2 Steer(Vector2 desired)
        {
            Vector2 steer = desired - velocity;

            return LimitMagnitude(steer, maxForce);
        }

        private float DistanceTo(Flocking boid)
        {
            return Vector3.Distance(boid.transform.position, Position);
        }

        private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
        {
            if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
            {
                baseVector = baseVector.normalized * maxMagnitude;
            }

            return baseVector;
        }
        #endregion
    }
}