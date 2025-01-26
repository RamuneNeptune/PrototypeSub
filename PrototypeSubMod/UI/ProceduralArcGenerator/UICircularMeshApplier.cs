using UnityEngine;

namespace PrototypeSubMod.UI.ProceduralArcGenerator
{
    public class UICircularMeshApplier : CircularMeshApplier
    {
        [SerializeField] private CanvasRenderer canvasRend;
        [SerializeField] private Material material;

        private void OnValidate()
        {
            if (!canvasRend) TryGetComponent(out canvasRend);
        }

        public override void UpdateMesh()
        {
            base.UpdateMesh();

            if (!canvasRend) return;

            canvasRend.materialCount = 1;
            canvasRend.SetMaterial(material, 0);
            canvasRend.SetMesh(lastMesh);
        }

        private void OnDisable()
        {
            canvasRend.Clear();
        }

        private void OnEnable()
        {
            UpdateMesh();
        }
    }
}