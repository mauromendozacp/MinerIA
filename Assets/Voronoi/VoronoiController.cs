using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.Voronoi
{
    public class VoronoiController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private List<Transform> points = null;
        [SerializeField] private Vector2Int limit = Vector2Int.zero;

        [Header("Gizmos Settings"), Space]
        [SerializeField] private Color limitColor = Color.white;
        [SerializeField] private Color pointLineColor = Color.white;
        #endregion

        #region PRIVATE_FIELDS
        private List<(Vector2, Vector2)> pointLines = new List<(Vector2, Vector2)>();
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            InitPointLines();

            foreach (Transform point in points)
            {
                foreach (Transform point2 in points)
                {
                    if (!points.Contains(point))
                    {
                        Vector3 bisectriz = point.position - point2.position;
                        bisectriz = -bisectriz;


                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            DrawLimits();
            DrawPointLines();
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitPointLines()
        {
            foreach (Transform point in points)
            {
                foreach (Transform point2 in points)
                {
                    pointLines.Add((point.transform.position, point2.transform.position));
                }
            }
        }

        private void DrawLimits()
        {
            Gizmos.color = limitColor;

            Gizmos.DrawLine(Vector3.zero, new Vector3(0f, limit.y));
            Gizmos.DrawLine(Vector3.zero, new Vector3(limit.x, 0f));
            Gizmos.DrawLine(new Vector3(limit.x, 0f), new Vector3(limit.x, limit.y));
            Gizmos.DrawLine(new Vector3(0f, limit.y), new Vector3(limit.x, limit.y));
        }

        private void DrawPointLines()
        {
            Gizmos.color = pointLineColor;

            for (int i = 0; i < pointLines.Count; i++)
            {
                Gizmos.DrawLine(pointLines[i].Item1, pointLines[i].Item2);
            }
        }
        #endregion
    }
}
