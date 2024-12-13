using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DimensionSwitcher : MonoBehaviour {
    public Transform player;
    private Plane _slicingPlane; // The plane that is used for slicing 3D objects when switching from 3D to 2D
    public GameObject[] slicableObjects = new GameObject[1]; //All the objects that will be sliced after dimension switch, can be modified in th editor
    public List<GameObject> slicedObjects;
    private List<Vector3> _intersectionPoints = new List<Vector3>(); // Store the intersection points
    private List<Vector3> _intersectionPoints3D = new List<Vector3>(); // List to store intersection points in 3D space before projecting
    private String _tagOfSlicedObject;
    private Vector3 _locationOfSlicedObject, _centroidOfSlicedObject;
    private Vector3 _centroid3D = Vector3.zero;
    public Vector3 planeRight;
    public Sprite topGroundSprite, bottomGroundSprite, acidSprite;

    public Vector3 Slice3DWorld() {
        // When switching to 2D all "slicable" objects need to be sliced, since 3D objects can't be used for 2D world
        Vector3 forwardDirection = player.forward; // This is the normal of the slicing plane
        // Slicing plane is used to check for intersections with "slicable" objects
        _slicingPlane = new Plane(forwardDirection, player.position);
        // Slice the objects and generate 2D geometry
        foreach (GameObject objectToSlice in slicableObjects){
            // Perform the intersection check
            if (!IsObjectIntersectingPlane(objectToSlice, _slicingPlane))
                continue; // Skip this object if it doesn't intersect the plane
            
            // Getting the tag and location to later assign them to the newly created object
            _tagOfSlicedObject = objectToSlice.tag;
            _locationOfSlicedObject = objectToSlice.transform.position;
            
            // Interactable objects aren't sliced, they have their counterpart (connected with TransitionablePar) moved to a new position
            if (objectToSlice.CompareTag("Interactable")) {
                GameObject counterpart = objectToSlice.GetComponent<TransitionablePair>().target.transform.gameObject; 
                counterpart.SetActive(true);
                AdjustPosition(counterpart, _locationOfSlicedObject); 
            }
            // Other objects are sliced
            else {
                SliceObject(objectToSlice); 
                Create2DObject(_intersectionPoints, objectToSlice);
            }
        }
        return planeRight;
    }
    
    private bool IsObjectIntersectingPlane(GameObject obj, Plane slicingPlane) {
        Bounds bounds; // The object must have either a renderer or a collider for bounds
        if(obj.GetComponent<Renderer>())
            bounds = obj.GetComponent<Renderer>().bounds;
        else if(obj.GetComponent<Collider>())
            bounds = obj.GetComponent<Collider>().bounds;
        else {
            Debug.LogError($"Cannot check intersection for {obj.name}");
            return false;
        }
        
        Vector3[] corners = {
            bounds.min, bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };

        bool anyAbove = false, anyBelow = false;
        foreach (var corner in corners) {
            if (slicingPlane.GetSide(corner)) anyAbove = true;
            else anyBelow = true;
            // If we find corners on both sides, the bounds intersect the plane
            if (anyAbove && anyBelow) return true;
        }
        // If all corners are on one side, there's no intersection
        return false;
    }

    public void Clean2DWorld() {
        // Removing all the 2D objects that were created by slicing
        foreach (GameObject createdObject in slicedObjects)
            Destroy(createdObject);
        slicedObjects.Clear(); 
    }

    private void SliceObject(GameObject obj) {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (!meshFilter) 
            return; // The object cannot be sliced if it has no mesh
        Mesh mesh = meshFilter.mesh;
        
        // All 3D objects are children of some other object, so all vertices need to be translated to world-space from local-space
        Vector3[] worldVertices = TransformFromLocalToWorld(obj, mesh);
        
        int[] triangles = mesh.triangles;
        // Clear previous intersection points
        _intersectionPoints.Clear(); 
        _intersectionPoints3D.Clear();
        
        // Defining the slicing plane's local coordinate system (planeRight and planeUp)
        // If needed to stop projecting slices, this block can be commented out
        Vector3 planeNormal = _slicingPlane.normal;
        planeRight = Vector3.Cross(planeNormal, Vector3.up).normalized;
        Vector3 planeUp = Vector3.Cross(planeNormal, planeRight).normalized;
        
        // Iterate over all triangles in the mesh
        for (int i = 0; i < triangles.Length; i += 3){
            // Getting each vertex of the triangle separately
            Vector3 v0 = worldVertices[triangles[i]];
            Vector3 v1 = worldVertices[triangles[i + 1]];
            Vector3 v2 = worldVertices[triangles[i + 2]];
            
            // Determine the side of the slicing plane for each vertex
            bool v0Above = _slicingPlane.GetSide(v0);
            bool v1Above = _slicingPlane.GetSide(v1);
            bool v2Above = _slicingPlane.GetSide(v2);
            
            // Checking if this triangle intersects the slicing plane & finding the intersection points on the triangle's edges
            // If needed to stop projecting slices, (ProjectTo2D(intersection, planeRight, planeUp)) can be switched to FindIntersection(x, x)
            if (v0Above != v1Above) {
                Vector3 intersection = FindIntersection(v0, v1);
                _intersectionPoints3D.Add(intersection);
                _intersectionPoints.Add(ProjectTo2D(intersection, planeUp));
            }
            if (v1Above != v2Above) {
                Vector3 intersection = FindIntersection(v1, v2);
                _intersectionPoints3D.Add(intersection);
                _intersectionPoints.Add(ProjectTo2D(intersection, planeUp));
            }
            if (v2Above != v0Above) {
                Vector3 intersection = FindIntersection(v2, v0);
                _intersectionPoints3D.Add(intersection);
                _intersectionPoints.Add(ProjectTo2D(intersection, planeUp));
            }
        }
    }

    // Function to transform all vertices of a mesh from local-space to world-space
    private Vector3[] TransformFromLocalToWorld(GameObject obj, Mesh mesh) {
        Vector3[] localVertices = mesh.vertices; // These are local-space vertices
        Matrix4x4 localToWorld = obj.transform.localToWorldMatrix; // Get the local-to-world matrix, which includes position, rotation, and scale
        Vector3[] worldVertices = new Vector3[localVertices.Length]; // Convert local vertices to world space, including scaling
        for (int i = 0; i < localVertices.Length; i++) // Transform the local vertex by the object's local-to-world matrix
            worldVertices[i] = localToWorld.MultiplyPoint3x4(localVertices[i]);
        return worldVertices;
    }

    // Function to find the intersection point between two vertices and the slicing plane
    private Vector3 FindIntersection(Vector3 v1, Vector3 v2){
        float distance1 = _slicingPlane.GetDistanceToPoint(v1);
        float distance2 = _slicingPlane.GetDistanceToPoint(v2);
        float t = distance1 / (distance1 - distance2);
        return Vector3.Lerp(v1, v2, t);  // Interpolating to find the intersection point
    }
    
    // A new 2D object is created by correctly connecting the sorted intersection points in a 2D space and adding a sprite
    private void Create2DObject(List<Vector3> polygon2D, GameObject objectToSlice) {
        if (polygon2D.Count < 3) 
            return; // We need at least 3 points to create a polygon
        // Create a new GameObject for the 2D polygon
        GameObject polygonObject = new GameObject($"Sliced_{objectToSlice.name}", typeof(PolygonCollider2D), typeof(SpriteRenderer));
        slicedObjects.Add(polygonObject); // Keeping count of all created objects for cleanup later
        polygonObject.tag = _tagOfSlicedObject; // The new object needs a matching tag (for example, to be able to kill the player or count as ground)
        
        // Clean up and sort vertices to ensure correct triangulation
        CleanupVertices(ref polygon2D);
        
        // Calculating the centroid (center point) of the polygon
        Vector3 centroid = Vector3.zero;
        foreach (var vertex in polygon2D) 
            centroid += vertex;
        centroid /= polygon2D.Count;
        
        SortVerticesClockwise(ref polygon2D, centroid);
        
        // Calculating the centroid (center point) of the 3D intersection points (used to correctly place the object later)
        _centroid3D = Vector3.zero;
        foreach (var point in _intersectionPoints3D)
            _centroid3D += point;
        _centroid3D /= _intersectionPoints3D.Count;
        Debug.Log($"Centroid of the polygon is: {_centroid3D} for {objectToSlice.name}");

        // Create a 2D collider using the 2D vertices
        Vector2[] colliderPoints = new Vector2[polygon2D.Count];
        PolygonCollider2D polygonCollider = polygonObject.GetComponent<PolygonCollider2D>();
        
        // The collider needs to be centered relative to the center of the game object
        for (int i = 0; i < polygon2D.Count; i++) 
            colliderPoints[i] = polygon2D[i] - centroid;
        polygonCollider.points = colliderPoints;
        
        SetUpSpriteRenderer(polygonObject, polygonCollider); // Set up the SpriteRenderer
        AdjustPosition(polygonObject, _centroid3D); // Placing the new object in the 2D world
    }

    // The sliced object's 3D position needs to be projected onto the slicing plane
    private void AdjustPosition(GameObject polygonObject, Vector3 centerPoint) {
        // Adjust position based on the object's z position relative to the slicing plane to retain 3D spacing
        Vector3 zOffset = (centerPoint - player.position).z * _slicingPlane.normal;
        if (Vector3.Dot(planeRight, Vector3.right) > 0) { // If plane right is not facing the same direction as x-axis, world needs to be mirrored
            float xOffset = centerPoint.x - player.transform.position.x; // This is the distance from the sliced object to the plane origin
            polygonObject.transform.position = new Vector3(centerPoint.x-xOffset*2-zOffset.x, _locationOfSlicedObject.y, 2f);
        }
        else
            polygonObject.transform.position = new Vector3(centerPoint.x-zOffset.x, _locationOfSlicedObject.y, 2f); // +zOffset or -zOffset flips the 2D world
    }

    private void CleanupVertices(ref List<Vector3> polygon2D) {
        for (int i = 2; i < polygon2D.Count; i++) {
            // Calculate the area of the triangle formed by the points, if it is zero, the points are collinear
            float area = polygon2D[i-2].x * (polygon2D[i-1].y - polygon2D[i].y) +
                         polygon2D[i-1].x * (polygon2D[i].y - polygon2D[i-2].y) +
                         polygon2D[i].x * (polygon2D[i-2].y - polygon2D[i-1].y);
            
            if (area != 0f) 
                continue; // We don't need to remove one of the points if they are not collinear
            
            // Calculate distances between each pair of points
            float d1 = Vector3.Distance(polygon2D[i-2], polygon2D[i-1]);
            float d2 = Vector3.Distance(polygon2D[i-1], polygon2D[i]);
            float d3 = Vector3.Distance(polygon2D[i-2], polygon2D[i]);

            // Check which point is in the middle
            if (Mathf.Approximately(d1 + d2, d3))      // polygon2D[i-1] is in the middle
                for (int j = i-1; j < polygon2D.Count - 1; j++) 
                    polygon2D[j] = polygon2D[j + 1];
            else if (Mathf.Approximately(d1 + d3, d2)) // polygon2D[i-2] is in the middle
                for (int j = i-2; j < polygon2D.Count - 1; j++) 
                    polygon2D[j] = polygon2D[j + 1];
            else                                       // polygon2D[i] is in the middle
                for (int j = i; j < polygon2D.Count - 1; j++) 
                    polygon2D[j] = polygon2D[j + 1];
            
            polygon2D.RemoveAt(polygon2D.Count - 1); // Remove the last element since it's now a duplicate after shifting
            i--; // Decrement 'i' to recheck the current position (since it now contains the next element)
        }
        // Getting rid of duplicates 
        polygon2D = polygon2D.Select(v => new Vector3((float)Math.Round(v.x, 4), (float)Math.Round(v.y, 4))).Distinct().ToList();
    }
    
    //After slicing vertices can be created randomly, so in order to create triangles correctly the vertices have to be sorted clockwise
    private void SortVerticesClockwise(ref List<Vector3> vertices, Vector3 centroid){
        // Sorting vertices based on their angle from the centroid
        vertices.Sort((a, b) => {
            float angleA = Mathf.Atan2(a.y - centroid.y, a.x - centroid.x);
            float angleB = Mathf.Atan2(b.y - centroid.y, b.x - centroid.x);
            return angleA.CompareTo(angleB);
        });
    }
    
    // Function to project a 3D point onto the slicing plane's 2D space
    private Vector2 ProjectTo2D(Vector3 point, Vector3 planeUp) {
        // Translate the point relative to the slicing plane origin
        Vector3 localPoint = point - player.position;
        float x = Vector3.Dot(localPoint, planeRight);   // X-coordinate
        float y = Vector3.Dot(localPoint, planeUp);      // Y-coordinate
        return new Vector2(x, y);
    }

    // Here we set up the new sprite renderer, add a sprite and fit it to the collider
    // Also the layer mask is set here
    private void SetUpSpriteRenderer(GameObject polygonObject, PolygonCollider2D coll) {
        SpriteRenderer spriteRenderer = polygonObject.GetComponent<SpriteRenderer>();
        
        // Each object type has its own sprite
        if (_tagOfSlicedObject is "Acid" or "Spikes") {
            spriteRenderer.sprite = acidSprite;
            polygonObject.layer = LayerMask.NameToLayer("Deadly"); // Needed to create objects deadly to the player
        }
        else if(_tagOfSlicedObject == "Ground")
            spriteRenderer.sprite = topGroundSprite;
        else   
            spriteRenderer.sprite = bottomGroundSprite;
        
        // Set the SpriteRenderer to use Tiled mode or Sliced mode, which allows resizing without affecting scale
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        // Get the bounds of the PolygonCollider2D
        Bounds colliderBounds = coll.bounds;
        // Set the size of the SpriteRenderer to match the collider's size
        Vector2 newSize = new Vector2(colliderBounds.size.x, colliderBounds.size.y);
        spriteRenderer.size = newSize;
        
        coll.autoTiling = true; //Fixes little offsets after assigning a sprite
    }
    
    // For debug purposes drawing the resulting plane in the editor
    private void DrawSlicingPlane(Plane slicingPlane, Vector3 planeCenter, float planeSize = 5.0f){
        Vector3 planeNormal = slicingPlane.normal; // Get the normal of the plane
        // Draw the normal direction
        Debug.DrawRay(planeCenter, planeNormal * 2.0f, Color.red); // Red line for the normal
        // Find two vectors that are perpendicular to the plane's normal
        planeRight = Vector3.Cross(planeNormal, Vector3.up).normalized;
        Debug.DrawRay(planeCenter, planeRight * -2.0f, Color.blue); // Blue line for the right
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
}