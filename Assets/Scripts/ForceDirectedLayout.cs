using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForceDirectedLayout : MonoBehaviour
{
    public float desiredDistance = 1;
    public float connectedNodeForce = 1;
    public float disconnectedNodeForce = 1;
    public float speed = .002f;
    public List<Node> nodes;
    private Vector3 mOffset;
    private float mZCoord;
  

    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<Node>();

        for (int i = 0; i < 20; i++)
        {
            nodes.Add(new Node() { children = nodes.Where(node => Random.value > 0.5f).ToList(), position = Random.insideUnitSphere * 10, velocity = Vector3.zero }); 
            
            Material newMaterial = new Material(Shader.Find("VertexLit"));
            nodes[i].dataPt.GetComponent<Renderer>().material = newMaterial;
            nodes[i].dataPt.gameObject.SetActive(true);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        ApplyForce();

        foreach (var node in nodes)
        {
            node.position += (node.velocity * speed) * Time.deltaTime;
            node.dataPt.transform.position = node.position;
            
        }
        
    }

    private Vector3 GetMouseWorldPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void ApplyForce()
    {
        foreach(var node in nodes)
        {
            var disconnectedNodes = nodes.Except(node.children); 

            foreach (var connectedNode in node.children)
            {
                var difference = node.position - connectedNode.position;
                var distance = difference.magnitude;
                var appliedForce = connectedNodeForce * Mathf.Log10(distance / desiredDistance);
                connectedNode.velocity += appliedForce * Time.deltaTime * difference.normalized;
            }
            foreach (var disconnectedNode in disconnectedNodes)
            {
                var difference = node.position - disconnectedNode.position;
                var distance = difference.magnitude;
                if (distance != 0)
                {
                    var appliedForce = disconnectedNodeForce / Mathf.Pow(distance, 2);
                    disconnectedNode.velocity += appliedForce * Time.deltaTime * difference.normalized;
                }
                
            }
        }
    }
    void OnMouseDown(){
        foreach (var node in nodes)
        {
            mZCoord = Camera.main.WorldToScreenPoint(node.dataPt.transform.position).z;
            mOffset = node.dataPt.transform.position - GetMouseAsWorldPoint();
        }
    }
    private Vector3 GetMouseAsWorldPoint()

    {

        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);

    }

    void OnMouseDrag()

    {

        transform.position = GetMouseAsWorldPoint() + mOffset;

    }


 

    void OnDrawGizmos()
    {
        foreach (var node in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(node.position, 0.5f);
            Gizmos.color = Color.blue;

            foreach (var connectedNode in node.children)
            {
                Gizmos.DrawLine(node.position, connectedNode.position);
            }
        }
    }
}

public class Node
{
    public GameObject dataPt = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    public Vector3 position;
    public Vector3 velocity;
    public List<Node> children;
}