using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class scrGenerator : MonoBehaviour
{
    private int x = -64;
    private int z = 32;
    private int i = 0;

    private bool debugOK = false;

    private int lastVert = 0;

    public int z_max = -32;
    public int x_max = 64;

    public int z_space = 8;
    public int x_space = 8;

    public float maxHeight = 1f;

    private int tGlob = 0;

    private Vector3[] vertices;

    private Mesh mesh;

    private bool generated = false;
    
    void Start()
    {
        GenerateLevel();
        //RandomTerrain();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (debugOK)
            {
                debugOK = false;
            }
            else
            {
                debugOK = true;
            }
        }
    }

    public void GenerateLevel()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "mapa";

        lastVert = (((x_space * z_space + 1) * (Mathf.Abs(z_max) + 1)) -2) -Mathf.Abs(z_max);

        z_max = Mathf.Abs(z_max) * -1;
        vertices = new Vector3[(Mathf.Abs(x_max) + 1) * (Mathf.Abs(z_max) + 1)];

        while (z > z_max-1)
        {
            while (x < x_max+1)
            {
                vertices[i] = new Vector3(x, Random.Range(0f,maxHeight), z);
                //Debug.Log("Nowy wierzcholek na pozycji: " + x + ", 0, " + z);
                x += x_space;
                i += 1;
            }
            z -= z_space;

            if (x > x_max)
            {
                x = -x_max;
            }
        }
		mesh.vertices = vertices;

        int[] triangles = new int[Mathf.Abs(x_max) * Mathf.Abs(z_max) * 6];
        for (int k = 0, t = 0; k < i; k += 1, t+=3)
        {

            if (k > lastVert)
            {
                break;
            }

            if ((k+1)%(Mathf.Abs(z_max)+1)!=0)
            {
                triangles[t] = k;
                triangles[t + 1] = k + 1;
                triangles[t + 2] = k + 33;
            }
            else
            {
                t += 3;
            }

            tGlob = t;
        }

        for (int k = 0, t = 0; k < i; k += 1, t += 3)
        {
            if ((k + 1) % (Mathf.Abs(z_max) + 1) != 0)
            {
                triangles[t + tGlob] = k + 1;
                triangles[t + 1 + tGlob] = k + (Mathf.Abs(z_max) + 2);
                triangles[t + 2 + tGlob] = k + (Mathf.Abs(z_max) + 1);
            }
            else
            {
                t += 3;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
        
        generated = true;
    }

    /*private void RandomTerrain()
    {
        for(int v=0; v<561; v++)
        {
            mesh.vertices
        }
    }*/

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        if (generated)
        {
            if (debugOK)
            {
                Debug.Log(lastVert);
                Gizmos.color = Color.black;
                for (int i = 0; i < vertices.Length; i++)
                {
                    Handles.Label(new Vector3(vertices[i].x, 2, vertices[i].z), i.ToString());
                    if (i % (Mathf.Abs(z_max) + 1) == 0)
                    {
                        Gizmos.DrawSphere(vertices[i], 0.1f);
                    }
                }
            }
        }
    }
}

/*[CustomEditor(typeof(scrGenerator))]
public class GeneratorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        scrGenerator script = (scrGenerator)target;
        if(GUILayout.Button("Generate map"))
        {
            script.GenerateLevel();
        }
    }
}*/
