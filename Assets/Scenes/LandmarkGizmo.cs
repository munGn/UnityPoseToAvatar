using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Tasks.Components.Containers;

public class LandmarkGizmo
{
    private PoseLandmarkerResult result;

    public void SetResult(PoseLandmarkerResult result)
    {
        this.result = result;
    }
    
    public void DrawLandmarks()
    {
        if (result.poseLandmarks == null) return;
        var poseWorldLandmark = result.poseWorldLandmarks[0];
        
        // Set the color with custom alpha.
        const float radius = 0.01f;
        const float alpha = 0.5f;
        Gizmos.color = new Color(1f, 0f, 0f, alpha); // Red with custom alpha

        foreach (var landmark in poseWorldLandmark.landmarks)
        {
            Gizmos.DrawSphere(LandmarkToVector(landmark), radius);
        }
    }
    
    private Vector3 LandmarkToVector(Landmark landmark)
    {
        var x = landmark.x;
        var y = -landmark.y;
        var z = landmark.z / 2;
        return new Vector3(x, y, z);
    }
}
