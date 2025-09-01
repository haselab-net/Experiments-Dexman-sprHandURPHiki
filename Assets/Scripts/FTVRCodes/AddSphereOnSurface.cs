using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSphereOnSurface: MonoBehaviour
{
    // Start is called before the first frame update
    Mesh mesh1;
    public GameObject shpere, shpere1, booleanSphere1, booleanSphere2;
    public Material MaterialRed;
    void Start()
    {

		mesh1 = GetComponent<MeshFilter>().mesh;
 
		Vector3[] vertices = mesh1.vertices;
 
		// foreach(Vector3 vertex in vertices)
		// {
		// 	Debug.Log( transform.TransformPoint(vertex));
		// }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] vertices = mesh1.vertices;
        for(int i = 0;i < vertices.Length;i++){
           vertices[i] = transform.TransformPoint(vertices[i]);
        }
        
        //print(GetNearestVertex(vertices, shpere));
        
        booleanSphere1.transform.position = GetNearestVertex(vertices, shpere);
        booleanSphere2.transform.position = GetNearestVertex(vertices, shpere1);
        // DrawLine(shpere.transform.position, GetNearestVertex(vertices, shpere), Color.red);
        // DrawLine(shpere1.transform.position, GetNearestVertex(vertices, shpere1), Color.red);

    }

    Vector3 GetNearestVertex(Vector3[] vertices, GameObject objectInput){
        if(vertices!=null&&vertices.Length>0){
            Vector3 targetTemp = vertices.Length>0? vertices[0]:new Vector3(0, 0, 0); 
            float dis = Vector3.Distance(objectInput.transform.position,vertices[0]);
            float disTemp;
            for(int i=1;i<vertices.Length;i++){
                disTemp = Vector3.Distance(objectInput.transform.position,vertices[i]);
                if(disTemp<dis){
                    targetTemp = vertices[i];
                    dis = disTemp;
                }
            }
            return targetTemp;
        }else{
            return new Vector3(0, 0, 0);
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.02f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = MaterialRed;
        // lr.startColor = color;
        // lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
