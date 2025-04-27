using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Vision.PoseLandmarker;

public class LandmarkRig : MonoBehaviour
{
    private Animator animator;

    private enum Landmark
    {
        Nose = 0,
        LeftEyeInner = 1,
        LeftEye = 2,
        LeftEyeOuter = 3,
        RightEyeInner = 4,
        RightEye = 5,
        RightEyeOuter = 6,
        LeftEar = 7,
        RightEar = 8,
        MouthLeft = 9,
        MouthRight = 10,
        LeftShoulder = 11,
        RightShoulder = 12,
        LeftElbow = 13,
        RightElbow = 14,
        LeftWrist = 15,
        RightWrist = 16,
        LeftPinky = 17,
        RightPinky = 18,
        LeftIndex = 19,
        RightIndex = 20,
        LeftThumb = 21,
        RightThumb = 22,
        LeftHip = 23,
        RightHip = 24,
        LeftKnee = 25,
        RightKnee = 26,
        LeftAnkle = 27,
        RightAnkle = 28,
        LeftHeel = 29,
        RightHeel = 30,
        LeftFootIndex = 31,
        RightFootIndex = 32
    }

    private Dictionary<Landmark, Vector3> landmarks;
    private Bone boneRightShoulder;
    private Bone boneRightUpperArm;
    private Bone boneRightLowerArm;
    private Bone boneLeftShoulder;
    private Bone boneLeftUpperArm;
    private Bone boneLeftLowerArm;
    private Bone boneRightUpperLeg;
    private Bone boneRightLowerLeg;
    private Bone boneRightFoot;
    private Bone boneLeftUpperLeg;
    private Bone boneLeftLowerLeg;
    private Bone boneLeftFoot;
    private Bone boneHead;
    private Vector3 initialUpDown;
    private Vector3 initialTwist;
    private Vector3 initialShoulderTwist;
    private Quaternion initialHipRotation;
    private Quaternion initialSpineRotation;
    private Quaternion initialChestRotation;

    void Start()
    {
        animator = GetComponent<Animator>();

        boneRightShoulder = new Bone(animator.GetBoneTransform(HumanBodyBones.RightShoulder), animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
        boneRightUpperArm = new Bone(animator.GetBoneTransform(HumanBodyBones.RightUpperArm), animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
        boneRightLowerArm = new Bone(animator.GetBoneTransform(HumanBodyBones.RightLowerArm), animator.GetBoneTransform(HumanBodyBones.RightHand));
        boneLeftShoulder = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftShoulder), animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
        boneLeftUpperArm = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm), animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
        boneLeftLowerArm = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), animator.GetBoneTransform(HumanBodyBones.LeftHand));
        boneRightUpperLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg), animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
        boneRightLowerLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg), animator.GetBoneTransform(HumanBodyBones.RightFoot));
        boneRightFoot = new Bone(animator.GetBoneTransform(HumanBodyBones.RightFoot), animator.GetBoneTransform(HumanBodyBones.RightToes));
        boneLeftUpperLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg), animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
        boneLeftLowerLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg), animator.GetBoneTransform(HumanBodyBones.LeftFoot));
        boneLeftFoot = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftFoot), animator.GetBoneTransform(HumanBodyBones.LeftToes));
        boneHead = new Bone(animator.GetBoneTransform(HumanBodyBones.Neck), animator.GetBoneTransform(HumanBodyBones.Head));
        
        Transform neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        initialUpDown = (neck.position - hips.position).normalized;
        initialHipRotation = hips.rotation;
        initialSpineRotation = animator.GetBoneTransform(HumanBodyBones.Spine).rotation;
        initialChestRotation = animator.GetBoneTransform(HumanBodyBones.Chest).rotation;

        Transform rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        initialTwist = (rightUpperLeg.position - leftUpperLeg.position).normalized;
        
        Transform rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        Transform leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        initialShoulderTwist = (rightShoulder.position - leftShoulder.position).normalized;
    }    

    void Update()
    {
        if (landmarks is null) return;
        
        boneRightUpperLeg.Update(landmarks[Landmark.RightHip], landmarks[Landmark.RightKnee]);
        boneRightLowerLeg.Update(landmarks[Landmark.RightKnee], landmarks[Landmark.RightAnkle]);
        boneRightFoot.Update(landmarks[Landmark.RightAnkle], landmarks[Landmark.RightHeel]);
        boneLeftUpperLeg.Update(landmarks[Landmark.LeftHip], landmarks[Landmark.LeftKnee]);
        boneLeftLowerLeg.Update(landmarks[Landmark.LeftKnee], landmarks[Landmark.LeftAnkle]);
        boneLeftFoot.Update(landmarks[Landmark.LeftAnkle], landmarks[Landmark.LeftHeel]);   
        boneRightUpperArm.Update(landmarks[Landmark.RightShoulder], landmarks[Landmark.RightElbow]);
        boneRightLowerArm.Update(landmarks[Landmark.RightElbow], landmarks[Landmark.RightWrist]);
        boneLeftUpperArm.Update(landmarks[Landmark.LeftShoulder], landmarks[Landmark.LeftElbow]);
        boneLeftLowerArm.Update(landmarks[Landmark.LeftElbow], landmarks[Landmark.LeftWrist]);
        
        Vector3 neckPos = (landmarks[Landmark.RightShoulder] + landmarks[Landmark.LeftShoulder]) / 2;
        Vector3 hipsPos = (landmarks[Landmark.RightHip] + landmarks[Landmark.LeftHip]) / 2;
        
        boneHead.Update(neckPos, landmarks[Landmark.Nose]);

        Vector3 currentUpDown = (neckPos - hipsPos).normalized;
        Quaternion deltaRotUpDown = Quaternion.FromToRotation(initialUpDown, currentUpDown);

        Vector3 currentTwist = (landmarks[Landmark.RightHip] - landmarks[Landmark.LeftHip]).normalized;
        Quaternion deltaRotTwist = Quaternion.FromToRotation(initialTwist, currentTwist);
        Vector3 currentShoulderTwist = (landmarks[Landmark.RightShoulder] - landmarks[Landmark.LeftShoulder]).normalized;
        Quaternion deltaRotShoulderTwist = Quaternion.FromToRotation(initialShoulderTwist, currentShoulderTwist);
        
        animator.GetBoneTransform(HumanBodyBones.Hips).rotation = deltaRotUpDown * deltaRotTwist * initialHipRotation;
        animator.GetBoneTransform(HumanBodyBones.Spine).rotation = deltaRotUpDown * Quaternion.Slerp(deltaRotTwist, deltaRotShoulderTwist, 0.5f) * initialSpineRotation;
        animator.GetBoneTransform(HumanBodyBones.Chest).rotation = deltaRotUpDown * deltaRotShoulderTwist * initialChestRotation;

    }

    public void SetPose(PoseLandmarkerResult result)
    {
        landmarks ??= new Dictionary<Landmark, Vector3>();
        var poseWorldLandmark = result.poseLandmarks[0];
        
        landmarks[Landmark.Nose] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.Nose]);
        landmarks[Landmark.LeftShoulder] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftShoulder]);
        landmarks[Landmark.RightShoulder] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightShoulder]);
        landmarks[Landmark.LeftElbow] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftElbow]);
        landmarks[Landmark.RightElbow] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightElbow]);
        landmarks[Landmark.LeftWrist] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftWrist]);
        landmarks[Landmark.RightWrist] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightWrist]);
        landmarks[Landmark.LeftHip] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftHip]);
        landmarks[Landmark.RightHip] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightHip]);
        landmarks[Landmark.LeftKnee] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftKnee]);
        landmarks[Landmark.RightKnee] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightKnee]);
        landmarks[Landmark.LeftAnkle] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftAnkle]);
        landmarks[Landmark.RightAnkle] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightAnkle]);
        landmarks[Landmark.LeftHeel] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.LeftHeel]);
        landmarks[Landmark.RightHeel] = LandmarkToVector(poseWorldLandmark.landmarks[(int)Landmark.RightHeel]);
    }

    Vector3 LandmarkToVector(NormalizedLandmark landmark)
    {
        var x = landmark.x;
        var y = -landmark.y;
        var z = landmark.z;
        return new Vector3(x, y, z);
    }
}

class Bone 
{
    public Transform parent;
    public Transform child;
    public Vector3 initialDir;
    public Quaternion initialRotation;
    public Bone(Transform parent, Transform child) 
    {
        this.parent = parent;
        this.child = child;
        initialDir = (child.position - parent.position).normalized;
        initialRotation = parent.rotation;
    }
    public void Update(Vector3 parentPos, Vector3 childPos)
    {
        Vector3 currentDir = (childPos - parentPos).normalized;
        Quaternion deltaRotTracked = Quaternion.FromToRotation(initialDir, currentDir);
        parent.rotation = deltaRotTracked * initialRotation;
    }
} 