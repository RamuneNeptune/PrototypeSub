using UnityEngine;

namespace PrototypeSubMod.UI.ProceduralArcGenerator
{
    [ExecuteInEditMode]
    public class CircularMeshApplier : MonoBehaviour
    {
        [SerializeField] private CircularMeshGenerator meshGenerator;
        [SerializeField] private bool showVertices;
        [SerializeField] private bool updateMesh;

        protected Mesh lastMesh;
        protected bool dirty;
        protected float lastAssignedAngle = float.PositiveInfinity;

        private void OnValidate()
        {
            dirty = true;
        }

        private void Start()
        {
            UpdateMesh();
        }

        private void Update()
        {
            if (!updateMesh && !dirty) return;
            UpdateMesh();
        }

        public virtual void UpdateMesh()
        {
            if (!meshGenerator) return;

            lastMesh = meshGenerator.GenerateMesh();
        }

        public virtual void SetTargetAngle(float angle)
        {
            if (lastAssignedAngle == angle) return;

            meshGenerator.SetTargetAngle(angle);
            UpdateMesh();

            lastAssignedAngle = angle;
        }

        private void OnDrawGizmosSelected()
        {
            if (!lastMesh || !showVertices) return;

            foreach (var item in lastMesh.vertices)
            {
                Gizmos.DrawSphere(transform.position + item, 0.1f);
            }
        }
    }
}