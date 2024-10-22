using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DimensionSwitcher : MonoBehaviour {
    public Transform player;
    private Plane _slicingPlane;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    // Store the intersection points
    private List<Vector3> _intersectionPoints = new List<Vector3>();

    void Update() {
        // Trigger dimension switch
        if (Input.GetKeyDown(KeyCode.T)){
            Vector3 forwardDirection = player.forward;
            _slicingPlane = new Plane(forwardDirection, player.position);
            Debug.DrawLine(player.position, player.position + forwardDirection, Color.green);
            DrawSlicingPlane(_slicingPlane, player.position);
            // Slice the object and generate 2D geometry
            SliceObject(obj1);
            Generate2DPolygonFromIntersections(_intersectionPoints);
            SliceObject(obj2);
            Generate2DPolygonFromIntersections(_intersectionPoints);
            SliceObject(obj3);
            Generate2DPolygonFromIntersections(_intersectionPoints);
        }
    }
    
    // For debug purposes
    void DrawSlicingPlane(Plane slicingPlane, Vector3 planeCenter, float planeSize = 5.0f){
        // Get the normal of the plane
        Vector3 planeNormal = slicingPlane.normal;
        // Draw the normal direction
        Debug.DrawRay(planeCenter, planeNormal * 2.0f, Color.red); // Red line for the normal
        // Find two vectors that are perpendicular to the plane's normal
        Vector3 planeRight = Vector3.Cross(planeNormal, Vector3.up).normalized;
        if (planeRight == Vector3.zero)
            // If planeNormal is pointing straight up or down, choose another axis to cross with
            planeRight = Vector3.Cross(planeNormal, Vector3.forward).normalized;
        
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
        if (!meshFilter) return;
        
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
        _intersectionPoints.Clear();

        // Iterate over all triangles in the mesh
        for (int i = 0; i < triangles.Length; i += 3){
            Vector3 v0 = worldVertices[triangles[i]];
            Vector3 v1 = worldVertices[triangles[i + 1]];
            Vector3 v2 = worldVertices[triangles[i + 2]];
            
            // Determine the side of the slicing plane for each vertex
            bool v0Above = _slicingPlane.GetSide(v0);
            bool v1Above = _slicingPlane.GetSide(v1);
            bool v2Above = _slicingPlane.GetSide(v2);
            
            // Check if this triangle intersects the slicing plane
            // Find the intersection points on the triangle's edges
            if (v0Above != v1Above){
                Vector3 intersection = FindIntersection(v0, v1);
                _intersectionPoints.Add(intersection);
            }
            if (v1Above != v2Above){
                Vector3 intersection = FindIntersection(v1, v2);
                _intersectionPoints.Add(intersection);
            }
            if (v2Above != v0Above){
                Vector3 intersection = FindIntersection(v2, v0);
                _intersectionPoints.Add(intersection);
            }
        }
    }

    // Function to find the intersection point between two vertices and the slicing plane
    Vector3 FindIntersection(Vector3 v1, Vector3 v2){
        float distance1 = _slicingPlane.GetDistanceToPoint(v1);
        float distance2 = _slicingPlane.GetDistanceToPoint(v2);
        float t = distance1 / (distance1 - distance2);
        return Vector3.Lerp(v1, v2, t);  // Interpolating to find the intersection point
    }
    
    // It is needed to connect the intersection points in a 2D space to create the polygon, triangulate the polygon for rendering in Unity
    void Generate2DPolygonFromIntersections(List<Vector3> polygon2D){
        if (_intersectionPoints.Count < 3) return; // There is nothing to generate since we need 3 points for at least one triangle
        // Create a new GameObject for the 2D mesh
        GameObject polygonObject = new GameObject("Sliced2DPolygon", typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D));
        // Placeholder material for the polygon
        polygonObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));

        CleanupVertices(ref polygon2D);
        SortVerticesClockwise(ref polygon2D);
        List<int> triangles = new List<int>();
        for (int i = 1; i < polygon2D.Count - 1; i++){
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        
        // Creating the 2D mesh
        Mesh polygonMesh = new Mesh {
            vertices = polygon2D.ToArray(),
            triangles = triangles.ToArray()
        };
        polygonMesh.RecalculateNormals();
        polygonMesh.RecalculateBounds();

        // Assign the created mesh to the MeshFilter
        polygonObject.GetComponent<MeshFilter>().mesh = polygonMesh;
    }

    void CleanupVertices(ref List<Vector3> polygon2D) {
        // Triangulation (assumes the polygon is convex and ordered correctly)
        for (int i = 0; i < polygon2D.Count; i++) {
            if (i < 2) continue; // We can't do any math until we are deeper in the list
            // Calculate the area of the triangle formed by the points, if it is zero, the points are collinear
            float area = polygon2D[i-2].x * (polygon2D[i-1].y - polygon2D[i].y) +
                         polygon2D[i-1].x * (polygon2D[i].y - polygon2D[i-2].y) +
                         polygon2D[i].x * (polygon2D[i-2].y - polygon2D[i-1].y);
            if (!Mathf.Approximately(area, 0.0f)) continue; // We don't need to remove one of the point if they are not collinear
            // Calculate distances between each pair of points
            float d1 = Vector3.Distance(polygon2D[i-2], polygon2D[i-1]);
            float d2 = Vector3.Distance(polygon2D[i-1], polygon2D[i]);
            float d3 = Vector3.Distance(polygon2D[i-2], polygon2D[i]);

            // Check which point is in the middle
            if (Mathf.Approximately(d1 + d2, d3))
                // polygon2D[i-1] is in the middle
                for (int j = i-1; j < polygon2D.Count - 1; j++) polygon2D[j] = polygon2D[j + 1];
            else if (Mathf.Approximately(d1 + d3, d2))
                // polygon2D[i-2] is in the middle
                for (int j = i-2; j < polygon2D.Count - 1; j++) polygon2D[j] = polygon2D[j + 1];
            else
                // polygon2D[i] is in the middle
                for (int j = i; j < polygon2D.Count - 1; j++) polygon2D[j] = polygon2D[j + 1];
            
            // Remove the last element since it's now a duplicate after shifting
            polygon2D.RemoveAt(polygon2D.Count - 1);
            // Decrement 'i' to recheck the current position (since it now contains the next element)
            i--;
        }
        // Getting rid of duplicates and sorting the vertices to create triangles more effectively
        polygon2D = polygon2D.Distinct().ToList();
    }
    
    //After slicing vertices can be created randomly, so in order to create triangles correctly the vertices have to be sorted clockwise
    void SortVerticesClockwise(ref List<Vector3> vertices){
        // Calculating the centroid (center point) of the polygon
        Vector3 centroid = Vector3.zero;
        foreach (var vertex in vertices) centroid += vertex;
        centroid /= vertices.Count;

        // Sorting vertices based on their angle from the centroid
        vertices.Sort((a, b) => {
            float angleA = Mathf.Atan2(a.y - centroid.y, a.x - centroid.x);
            float angleB = Mathf.Atan2(b.y - centroid.y, b.x - centroid.x);
            return angleA.CompareTo(angleB);
        });
    }
}