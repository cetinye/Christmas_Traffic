using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Christmas_Traffic
{
    public class PathCreator : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };
        public List<Vector3> points = new List<Vector3>();
        public bool drawable;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void DrawLine()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.transform.TryGetComponent<Collider>(out Collider collider) && collider.CompareTag("Plane"))
                {
                    drawable = true;
                    points.Clear();
                }
                else
                    drawable = false;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && drawable)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    if (drawable && hitInfo.collider.CompareTag("Plane"))
                    {
                        if (DistanceToLastPoint(hitInfo.point) > 1)
                        {
                            points.Add(new Vector3(hitInfo.point.x, hitInfo.point.y, 0));

                            lineRenderer.positionCount = points.Count;
                            lineRenderer.SetPositions(points.ToArray());
                        }
                    }
                }
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnNewPathCreated(points);
            }
        }

        private float DistanceToLastPoint(Vector2 point)
        {
            if (!points.Any())
                return Mathf.Infinity;

            return Vector2.Distance(points.Last(), point);
        }
    }
}