using UnityEngine;

namespace PrototypeSubMod.UI.ProceduralArcGenerator
{
    public class CircularMeshGenerator : MonoBehaviour
    {
        [SerializeField] private int resolution = 1;
        [SerializeField] private float distanceInner;
        [SerializeField] private float distanceOuter;
        [SerializeField, Range(0, 360)] private float angle;

        private void OnValidate()
        {
            resolution = Mathf.Max(resolution, 2);
        }

        public Mesh GenerateMesh()
        {
            var mesh = new Mesh();

            var vertices = new Vector3[(resolution + 1) * 2];
            var uv = new Vector2[vertices.Length];
            var triangles = new int[(vertices.Length - 2) * 3];

            for (int i = 0; i < vertices.Length; i++)
            {
                int normIndex = i % (resolution + 1); // The index along the side of the arc it is on
                bool inside = i > resolution;
                float pointAngle = Mathf.Lerp(0, angle, (float)normIndex / resolution);
                float scalar = inside ? distanceInner : distanceOuter;

                float x = Mathf.Cos(pointAngle * Mathf.Deg2Rad) * scalar;
                float y = Mathf.Sin(pointAngle * Mathf.Deg2Rad) * scalar;
                vertices[i] = new Vector3(x, y, 0);
                uv[i] = inside ? Vector2.zero : Vector2.one;
            }

            int mod = resolution + 1;
            for (int i = 0; i < triangles.Length; i += 6)
            {
                int iModified = i / Mathf.Min(6, mod);

                if (i >= triangles.Length - 7 && resolution <= 3) iModified -= 1;

                int inner1 = iModified + mod;
                int outer1 = iModified;
                int inner2 = iModified + mod + 1;
                int outer2 = iModified + 1;

                triangles[i] = inner1;
                triangles[i + 1] = inner2;
                triangles[i + 2] = outer1;

                triangles[i + 3] = inner2;
                triangles[i + 4] = outer2;
                triangles[i + 5] = outer1;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;
        }

        public void SetTargetAngle(float angle)
        {
            this.angle = angle;
        }
    }
}