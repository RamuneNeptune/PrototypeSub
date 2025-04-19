using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.ProceduralArcGenerator;

public class GraphicCircularMeshApplier : Graphic
{
    [SerializeField] private CircularMeshGenerator meshGenerator;
    [SerializeField] private bool updateMesh;

    private void OnValidate()
    {
        if (updateMesh)
        {
            
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        var mesh = meshGenerator.GenerateMesh();
        base.OnPopulateMesh(vh);
        vh.Clear();

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.position = mesh.vertices[i];
            vert.normal = mesh.normals[i];
            vh.AddVert(vert);
        }

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            var index0 = mesh.triangles[i];
            var index1 = mesh.triangles[i + 1];
            var index2 = mesh.triangles[i + 2];
            vh.AddTriangle(index0, index1, index2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!updateMesh) return;

        UpdateGeometry();
    }
}