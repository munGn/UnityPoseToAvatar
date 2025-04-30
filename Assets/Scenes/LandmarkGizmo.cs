using System.Collections.Generic;
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
        
        var pairs = new List<(int, int)>
        {
            (12, 11), (12, 24), (11, 23), (24, 23),
            (24, 26), (23, 25), (26, 28), (25, 27),
            (28, 32), (27, 31), (28, 30), (27, 29),
            (32, 30), (31, 29), (14, 12), (11, 13),
            (14, 16), (13, 15), (0, 5), (0, 1),
        };
      
        foreach (var (i1, i2) in pairs)
        {
            var from = LandmarkToVector(poseWorldLandmark.landmarks[i1]);
            var to = LandmarkToVector(poseWorldLandmark.landmarks[i2]);
            Gizmos.DrawLine(from, to);
        }
    }
    
    private Vector3 LandmarkToVector(Landmark landmark)
    {
        var x = landmark.x;
        var y = -landmark.y;
        var z = landmark.z;
        return new Vector3(x, y, z);
    }
}
