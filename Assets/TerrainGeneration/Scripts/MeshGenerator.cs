using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(
            float[,] heightMap,
            float heightMultiplier,
            AnimationCurve heightCurve,
            int levelOfDetail)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float topLeftX = (width - 1) * -0.5f;
            float topLeftZ = (height - 1) * 0.5f;

            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int vertexPerLine = (width - 1) / meshSimplificationIncrement + 1;

            var meshData = new MeshData(vertexPerLine, vertexPerLine);

            var vertexIndex = 0;
            for (var y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (var x = 0; x < width; x += meshSimplificationIncrement)
                {
                    meshData.vertices[vertexIndex] =
                        new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier,
                            topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(
                            vertexIndex,
                            vertexIndex + vertexPerLine + 1,
                            vertexIndex + vertexPerLine);
                        meshData.AddTriangle(
                            vertexIndex + vertexPerLine + 1,
                            vertexIndex,
                            vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        private int _triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[_triangleIndex] = a;
            triangles[_triangleIndex + 1] = b;
            triangles[_triangleIndex + 2] = c;

            _triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}