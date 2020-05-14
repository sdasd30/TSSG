using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderExtrude : MonoBehaviour
{
    
    public GameObject WallObjPrefab;
    public bool DisplayDebugMesh = false;

    private bool m_isTemporaryTile = false;
    private bool m_wallsCreated = false;
    // Start is called before the first frame update

    void Start()
    {
        if (m_isTemporaryTile)
            return;
        GameObject newObj = Instantiate(gameObject,transform.parent);
        newObj.GetComponent<ColliderExtrude>().m_isTemporaryTile = true;
        newObj.transform.rotation = Quaternion.identity;
        
        
    }

    private void Update()
    {
        if (!m_isTemporaryTile)
            return;
        if (m_wallsCreated)
        {
            Destroy(gameObject);
            return;
        }
        createMesh();
    }

    private void createMesh()
    {
        CompositeCollider2D m_compositeCollider = GetComponent<CompositeCollider2D>();
        if (m_compositeCollider.pathCount == 0)
            return;
        for (int p = 0; p < m_compositeCollider.pathCount; p++)
        {
            Vector2[] pathVerts = new Vector2[m_compositeCollider.GetPathPointCount(p)];
            m_compositeCollider.GetPath(p, pathVerts);
            List<Vector3> verts = new List<Vector3>();
            foreach (Vector2 v in pathVerts)
                verts.Add(new Vector3(v.x, v.y, 0f));

            verts.Reverse();
            int N = verts.Count;
            List<Vector3> verticesList = new List<Vector3>(verts);
            List<Vector3> verticesExtrudedList = new List<Vector3>();
            List<int> indices = new List<int>();

            for (int i = 0; i < verticesList.Count; i++)
            {
                verticesExtrudedList.Add(new Vector3(verticesList[i].x, verticesList[i].y, 50));
            }

            //add the extruded parts to the end of verteceslist
            verticesList.AddRange(verticesExtrudedList);

            for (int i = 0; i < N; i++)
            {

                int i1 = i;
                int i2 = (i1 + 1) % N;
                int i3 = i1 + N;
                int i4 = i2 + N;

                indices.Add(i1);
                indices.Add(i3);
                indices.Add(i4);

                indices.Add(i1);
                indices.Add(i4);
                indices.Add(i2);

            }

            Mesh m = new Mesh();

            m.vertices = verticesList.ToArray();
            m.triangles = indices.ToArray();

            m.RecalculateNormals();
            m.RecalculateBounds();
            m.Optimize();
            GameObject WallObj = Instantiate(WallObjPrefab, transform.position, Quaternion.identity);
            if (DisplayDebugMesh)
                WallObj.AddComponent(typeof(MeshRenderer));
            MeshFilter filter = WallObj.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = m;
            WallObj.AddComponent<MeshCollider>();
            WallObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            Vector3 pos = WallObj.transform.position;
            WallObj.transform.position = new Vector3(pos.x, pos.y + 10f, pos.z);
        }
        m_wallsCreated = true;
    }
}
