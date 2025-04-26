using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.ProceduralArcGenerator;

public class GraphicCircularMeshApplier : Graphic
{
    [SerializeField] private CircularMeshGenerator meshGenerator;
    [SerializeField] private bool updateMesh;

    private Mesh lastArcMesh;
    
    protected override void Start()
    {
        base.Start();
        meshGenerator.GenerateMesh();
        meshGenerator.onGenerateMesh += mesh =>
        {
            lastArcMesh = mesh;
            UpdateGeometry();
        };
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (lastArcMesh == null)
        {
            meshGenerator.GenerateMesh();
            return;
        }
        
        base.OnPopulateMesh(vh);
        vh.Clear();
        
        for (int i = 0; i < lastArcMesh.vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.position = lastArcMesh.vertices[i];
            vert.normal = lastArcMesh.normals[i];
            vh.AddVert(vert);
        }

        for (int i = 0; i < lastArcMesh.triangles.Length; i += 3)
        {
            var index0 = lastArcMesh.triangles[i];
            var index1 = lastArcMesh.triangles[i + 1];
            var index2 = lastArcMesh.triangles[i + 2];
            vh.AddTriangle(index0, index1, index2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!updateMesh) return;

        lastArcMesh = meshGenerator.GenerateMesh();
        UpdateGeometry();
    }
}