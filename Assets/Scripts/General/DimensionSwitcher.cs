using UnityEngine;
using System.Collections.Generic;

public class DimensionSwitcher : MonoBehaviour
{
    public Transform player;
    private Plane slicingPlane;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;

    // Store the intersection points
    private List<Vector3> intersectionPoints = new List<Vector3>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))  // Trigger dimension switch
        {
            Vector3 forwardDirection = player.forward;
            slicingPlane = new Plane(forwardDirection, player.position);
            Debug.DrawLine(player.position, player.position + forwardDirection, Color.green);
            DrawSlicingPlane(slicingPlane, player.position);
            // Slice the object and generate 2D geometry
            SliceObject(obj1);
            Generate2DPolygonFromIntersections();
            SliceObject(obj2);
            Generate2DPolygonFromIntersections();
            SliceObject(obj3);
            Generate2DPolygonFromIntersections();
        }
    }
    
    // For debug purposes
    void DrawSlicingPlane(Plane slicingPlane, Vector3 planeCenter, float planeSize = 5.0f)
    {
        // Get the normal of the plane
        Vector3 planeNormal = slicingPlane.normal;

        // Draw the normal direction
        Debug.DrawRay(planeCenter, planeNormal * 2.0f, Color.red); // Red line for the normal

        // Find two vectors that are perpendicular to the plane's normal
        Vector3 planeRight = Vector3.Cross(planeNormal, Vector3.up).normalized;
        if (planeRight == Vector3.zero){
            // If planeNormal is pointing straight up or down, choose another axis to cross with
            planeRight = Vector3.Cross(planeNormal, Vector3.forward).normalized;
        }

        Vector3 planeUp = Vector3.Cross(planeNormal, planeRight).normalized;

        // Calculate the four corners of a square on the plane
        Vector3 corner1 = planeCenter + (planeRight * planeSize / 2) + (planeUp * planeSize / 2);
        Vector3 corner2 = planeCenter + (planeRight * planeSize / 2) - (planeUp * planeSize / 2);
        Vector3 corner3 = planeCenter - (planeRight * planeSize / 2) - (planeUp * planeSize / 2);
        Vector3 corner4 = planeCenter - (planeRight * planeSize / 2) + (planeUp * planeSize / 2);

        // Draw the square representing a portion of the plane
        Debug.DrawLine(corner1, corner2, Color.green); // Green for the plane surface
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner4, Color.green);
        Debug.DrawLine(corner4, corner1, Color.green);
    }

    void SliceObject(GameObject obj) {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.mesh;
        Vector3[] localVertices = mesh.vertices; // These are local-space vertices

        // Get the local-to-world matrix, which includes position, rotation, and scale
        Matrix4x4 localToWorld = obj.transform.localToWorldMatrix;

        // Convert local vertices to world space, including scaling
        Vector3[] worldVertices = new Vector3[localVertices.Length];
        for (int i = 0; i < localVertices.Length; i++)
            // Transform the local vertex by the object's local-to-world matrix
            worldVertices[i] = localToWorld.MultiplyPoint3x4(localVertices[i]);

        int[] triangles = mesh.triangles;

        // Clear previous intersection points
        intersectionPoints.Clear();

        // Iterate over all triangles in the mesh
        for (int i = 0; i < triangles.Length; i += 3){
            Vector3 v0 = worldVertices[triangles[i]];
            Vector3 v1 = worldVertices[triangles[i + 1]];
            Vector3 v2 = worldVertices[triangles[i + 2]];

            // Determine the side of the slicing plane for each vertex
            bool v0Above = slicingPlane.GetSide(v0);
            bool v1Above = slicingPlane.GetSide(v1);
            bool v2Above = slicingPlane.GetSide(v2);
            // Debug.Log(v0 + ", " + v1 + ", " + v2);
            // Check if this triangle intersects the slicing plane
            // Find the intersection points on the triangle's edges
            if (v0Above != v1Above){
                Vector3 intersection = FindIntersection(v0, v1);
                intersectionPoints.Add(intersection);
            }
            if (v1Above != v2Above){
                Vector3 intersection = FindIntersection(v1, v2);
                intersectionPoints.Add(intersection);
            }
            if (v2Above != v0Above){
                Vector3 intersection = FindIntersection(v2, v0);
                intersectionPoints.Add(intersection);
            }
        }
    }

    // Function to find the intersection point between two vertices and the slicing plane
    Vector3 FindIntersection(Vector3 v1, Vector3 v2){
        float distance1 = slicingPlane.GetDistanceToPoint(v1);
        float distance2 = slicingPlane.GetDistanceToPoint(v2);

        float t = distance1 / (distance1 - distance2);
        // Debug.Log(Vector3.Lerp(v1, v2, t));
        return Vector3.Lerp(v1, v2, t);  // Interpolating to find the intersection point
    }

    // Generate a 2D polygon from the intersection points
    void Generate2DPolygonFromIntersections(){
        if (intersectionPoints.Count < 3){
            Debug.LogWarning("Not enough points to form a polygon");
            // Debug.Log(intersectionPoints.Count);
            return;
        }
        
        List<Vector3> polygon2D = new List<Vector3>(); // Change 2d or 3d vectors here

        foreach (Vector3 intersectionPoint in intersectionPoints)
            // Convert 3D points into 2D (by discarding Z or using another plane of your choice)
            polygon2D.Add(intersectionPoint);
        
        // Now it is needed to connect these points in a 2D space to create the polygon, triangulate the polygon for rendering in Unity
        TriangulateAndRender2DPolygon(polygon2D);
    }

    // A simple function to triangulate and render the 2D polygon
    void TriangulateAndRender2DPolygon(List<Vector3> polygon2D){
        // Create a new GameObject for the 2D mesh
        GameObject polygonObject = new GameObject("Sliced2DPolygon", typeof(MeshFilter), typeof(MeshRenderer));
        
        // Placeholder material for the polygon (replace with your own material)
        polygonObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));

        // Triangulation: This assumes the polygon is convex and ordered correctly (or you could use a library like LibTessDotNet)
        List<int> triangles = new List<int>();
        for (int i = 0; i < polygon2D.Count; i++)
        {
            if (i != 0 && polygon2D[i] == polygon2D[i - 1])
            {
                // Shift elements one position back starting from the current position
                for (int j = i; j < polygon2D.Count - 1; j++)
                {
                    polygon2D[j] = polygon2D[j + 1];
                }

                // Remove the last element since it's now a duplicate after shifting
                polygon2D.RemoveAt(polygon2D.Count - 1);

                // Decrement 'i' to recheck the current position (since it now contains the next element)
                i--;
            }
        }
        for (int i = 1; i < polygon2D.Count - 1; i++){
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // Convert the 2D points back to 3D for rendering (setting Z = 0))
        Vector3[] vertices3D = new Vector3[polygon2D.Count];
        for (int i = 0; i < polygon2D.Count; i++){
            vertices3D[i] = new Vector3(polygon2D[i].x, polygon2D[i].y, polygon2D[i].z);
            GameObject point = new GameObject("point " + i.ToString());
            point.transform.position = vertices3D[i];
        }
            

        // Create the 2D mesh
        Mesh polygonMesh = new Mesh();
        polygonMesh.vertices = vertices3D;
        polygonMesh.triangles = triangles.ToArray();
        polygonMesh.RecalculateNormals();
        polygonMesh.RecalculateBounds();

        // Assign the mesh to the MeshFilter
        polygonObject.GetComponent<MeshFilter>().mesh = polygonMesh;
    }
}