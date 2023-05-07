using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Mastered.Magisteros.NPC
{
    public class CharacterAwareness : MonoBehaviour
    {
        #region Public and Serialized variables

        [Header("View Cone Attributes")]
        public float viewConeFov = 60f;
        public float viewConeMaxDistance = 5f;
        [SerializeField] bool isATargetInView;
        [SerializeField] bool isViewToTargetClear;
        [SerializeField] Transform closestTargetInView;
        [SerializeField] List<Transform> targetsInView;

        [Header("Proximity Attributes")]
        [SerializeField] LayerMask proximityMask;
        [SerializeField] bool isATargetInProximity;
        public float proximityRange = 10f;
        [SerializeField] Transform closestTargetInProximity;
        [SerializeField] Collider[] targetsInProximity;

        #endregion

        #region Private variables

        Transform characterHead;

        #endregion

        #region Monobehaviour methods

        private void Awake()
        {
            characterHead = transform.Find("Head");
        }

        private void FixedUpdate()
        {
            targetsInProximity = Physics.OverlapSphere(transform.position, proximityRange, proximityMask);

            if(targetsInProximity.Length > 0)
            {
                closestTargetInProximity = FindClosestTargetInProximity(targetsInProximity).transform;

                targetsInView = FindTargetsInView(targetsInProximity);
                closestTargetInView = FindClosestTargetInView(targetsInView);
            }

            isATargetInProximity = targetsInProximity.Length > 0;
            isATargetInView = targetsInView.Count > 0;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                // Proximity gizmos
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, proximityRange);
                Gizmos.color = Color.blue;
                if (isATargetInProximity)
                    Gizmos.DrawLine(transform.position, closestTargetInProximity.position);

                // View cone gizmos
                Gizmos.color = Color.yellow;
                #region Draw view cone
                float totalFOV = viewConeFov;
                float rayRange = viewConeMaxDistance;
                Quaternion leftRayRotation = Quaternion.AngleAxis(-totalFOV, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(totalFOV, Vector3.up);
                Vector3 leftRayDirection = leftRayRotation * transform.forward;
                Vector3 rightRayDirection = rightRayRotation * transform.forward;
                Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
                Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
                Handles.color = Color.yellow;
                UnityEditor.Handles.DrawWireArc(transform.position, transform.up, leftRayDirection, viewConeFov * 2, viewConeMaxDistance);
                #endregion
                Gizmos.color = Color.red;
                if (isATargetInView)
                    Gizmos.DrawLine(transform.position, closestTargetInView.position);
            }
        }

        #endregion

        #region Custom methods

        Collider FindClosestTargetInProximity(Collider[] targets)
        {
            Collider closest = null;

            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (Collider c in targets)
            {
                float dist = Vector3.Distance(c.transform.position, currentPos);
                if (dist < minDist)
                {
                    closest = c;
                    minDist = dist;
                }
            }

            return closest;
        }
        Transform FindClosestTargetInView(List<Transform> targets)
        {
            Transform closest = null;

            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (Transform t in targets)
            {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist < minDist)
                {
                    closest = t;
                    minDist = dist;
                }
            }

            return closest;
        }

        List<Transform> FindTargetsInView(Collider[] targets)
        {
            List<Transform> targetsInView = new List<Transform>();

            foreach(Collider c in targets)
            {
                Vector3 targetDir = c.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);
                float distance = Vector3.Distance(transform.position, c.transform.position);

                if (angle < viewConeFov && distance < viewConeMaxDistance)
                {
                    targetsInView.Add(c.transform);
                }
            }

            return targetsInView;
        }

        #endregion
    }
}
