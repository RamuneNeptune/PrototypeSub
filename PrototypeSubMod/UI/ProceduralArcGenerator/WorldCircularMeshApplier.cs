using UnityEngine;

namespace PrototypeSubMod.UI.ProceduralArcGenerator
{
    public class WorldCircularMeshApplier : CircularMeshApplier
    {
        [SerializeField] private MeshFilter meshFilter;

        private void OnValidate()
        {
            if (!meshFilter) TryGetComponent(out meshFilter);
        }

        public override void UpdateMesh()
        {
            base.UpdateMesh();

            meshFilter.sharedMesh = lastMesh;
        }
    }
}