// using UnityEngine;
// using System.Collections.Generic;

// public class LandmarkRig : MonoBehaviour
// {
//     private Animator animator;

//     // 랜드마크 종류 정의 (Foot Index 포함)
//     public enum Landmark
//     {
//         Nose = 0, LeftEyeInner = 1, LeftEye = 2, LeftEyeOuter = 3, RightEyeInner = 4, RightEye = 5, RightEyeOuter = 6, LeftEar = 7, RightEar = 8, MouthLeft = 9, MouthRight = 10,
//         LeftShoulder = 11, RightShoulder = 12, LeftElbow = 13, RightElbow = 14, LeftWrist = 15, RightWrist = 16, LeftPinky = 17, RightPinky = 18, LeftIndex = 19, RightIndex = 20, LeftThumb = 21, RightThumb = 22,
//         LeftHip = 23, RightHip = 24, LeftKnee = 25, RightKnee = 26, LeftAnkle = 27, RightAnkle = 28, LeftHeel = 29, RightHeel = 30, LeftFootIndex = 31, RightFootIndex = 32
//     }

//     // 랜드마크 데이터를 저장할 Dictionary (테스트 또는 실시간 데이터)
//     private Dictionary<Landmark, Vector3> landmarks = new Dictionary<Landmark, Vector3>();

//     // 각 뼈를 관리할 Bone 객체들
//     private Bone boneRightUpperArm;
//     private Bone boneRightLowerArm;
//     private Bone boneLeftUpperArm;
//     private Bone boneLeftLowerArm;
//     private Bone boneRightUpperLeg;
//     private Bone boneRightLowerLeg;
//     private Bone boneRightFoot;
//     private Bone boneLeftUpperLeg;
//     private Bone boneLeftLowerLeg;
//     private Bone boneLeftFoot;
//     private Bone boneHead;
//     // 참고: 어깨(Shoulder)뼈는 Torso 로직에 의해 제어되거나 별도 처리가 필요할 수 있어 제외함

//     // 초기 T-Pose 기준 Torso 상태 변수
//     private Vector3 initialTposeUpDown;
//     private Vector3 initialTposeTwist;
//     private Vector3 initialTposeShoulderTwist;
//     private Quaternion initialHipRotation;   // T-Pose 기준 회전
//     private Quaternion initialSpineRotation; // T-Pose 기준 회전
//     private Quaternion initialChestRotation; // T-Pose 기준 회전


//     void Start()
//     {
//         animator = GetComponent<Animator>();
//         if (animator == null || !animator.isHuman)
//         {
//             Debug.LogError("Humanoid Animator 컴포넌트가 필요합니다. 스크립트를 비활성화합니다.");
//             enabled = false;
//             return;
//         }

//         try
//         {
//             // --- 1. T-Pose 상태 캡처 ---
//             Debug.Log("LandmarkRig: T-Pose 상태 캡처 시작...");

//             // 필수 Bone Transform 가져오기
//             Transform hipsT = animator.GetBoneTransform(HumanBodyBones.Hips);
//             Transform spineT = animator.GetBoneTransform(HumanBodyBones.Spine);
//             Transform chestT = animator.GetBoneTransform(HumanBodyBones.Chest);
//             Transform neckT = animator.GetBoneTransform(HumanBodyBones.Neck);
//             Transform headT = animator.GetBoneTransform(HumanBodyBones.Head);
//             Transform rightShoulderT = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
//             Transform leftShoulderT = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
//             Transform rightHipT = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
//             Transform leftHipT = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
//             Transform rightUpperArmT = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
//             Transform rightLowerArmT = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
//             Transform rightHandT = animator.GetBoneTransform(HumanBodyBones.RightHand);
//             Transform leftUpperArmT = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
//             Transform leftLowerArmT = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
//             Transform leftHandT = animator.GetBoneTransform(HumanBodyBones.LeftHand);
//             Transform rightUpperLegT = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
//             Transform rightLowerLegT = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
//             Transform rightFootT = animator.GetBoneTransform(HumanBodyBones.RightFoot);
//             Transform rightToesT = animator.GetBoneTransform(HumanBodyBones.RightToes);
//             Transform leftUpperLegT = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
//             Transform leftLowerLegT = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
//             Transform leftFootT = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
//             Transform leftToesT = animator.GetBoneTransform(HumanBodyBones.LeftToes);

//             // 모든 필수 Transform Null 체크
//             if (hipsT == null || spineT == null || chestT == null || neckT == null || headT == null ||
//                 rightShoulderT == null || leftShoulderT == null || rightHipT == null || leftHipT == null ||
//                 rightUpperArmT == null || rightLowerArmT == null || rightHandT == null || leftUpperArmT == null ||
//                 leftLowerArmT == null || leftHandT == null || rightUpperLegT == null || rightLowerLegT == null ||
//                 rightFootT == null || rightToesT == null || leftUpperLegT == null || leftLowerLegT == null ||
//                 leftFootT == null || leftToesT == null)
//             {
//                 // 어떤 본이 문제인지 더 자세히 로그를 남기면 좋음
//                 Debug.LogError("초기화에 필요한 Bone Transform 중 일부를 찾을 수 없습니다. Humanoid 설정 및 모델 구조를 확인하세요.");
//                 enabled = false; // 또는 throw new System.NullReferenceException(...)
//                 return;
//             }
//             Debug.Log(" - 모든 필수 Bone Transform 찾음.");

//             // T-Pose 회전값 저장
//             initialHipRotation = hipsT.rotation;
//             initialSpineRotation = spineT.rotation;
//             initialChestRotation = chestT.rotation;
//             Debug.Log($" - 초기 Torso 회전값 저장: Hip={initialHipRotation.eulerAngles}");


//             // T-Pose 뼈 위치 기준 초기 Torso 벡터 계산
//             initialTposeUpDown = (neckT.position - hipsT.position).normalized;
//             initialTposeTwist = (rightHipT.position - leftHipT.position).normalized;
//             initialTposeShoulderTwist = (rightShoulderT.position - leftShoulderT.position).normalized;

//             // T-Pose 벡터 유효성 검사
//             if (initialTposeUpDown == Vector3.zero || initialTposeTwist == Vector3.zero || initialTposeShoulderTwist == Vector3.zero) {
//                  Debug.LogWarning("초기 Torso 벡터 중 일부가 Zero입니다. T-Pose 뼈 위치 및 간격을 확인하세요.");
//             }
//             Debug.Log($" - 초기 T-Pose 벡터 계산: UpDown={initialTposeUpDown}, Twist={initialTposeTwist}, ShoulderTwist={initialTposeShoulderTwist}");


//             // Bone 객체 생성 (T-Pose Transform 전달)
//             boneRightUpperArm = new Bone(rightUpperArmT, rightLowerArmT);
//             boneRightLowerArm = new Bone(rightLowerArmT, rightHandT);
//             boneLeftUpperArm = new Bone(leftUpperArmT, leftLowerArmT);
//             boneLeftLowerArm = new Bone(leftLowerArmT, leftHandT);
//             boneRightUpperLeg = new Bone(rightUpperLegT, rightLowerLegT);
//             boneRightLowerLeg = new Bone(rightLowerLegT, rightFootT);
//             boneLeftUpperLeg = new Bone(leftUpperLegT, leftLowerLegT);
//             boneLeftLowerLeg = new Bone(leftLowerLegT, leftFootT);
//             boneRightFoot = new Bone(rightFootT, rightToesT);
//             boneLeftFoot = new Bone(leftFootT, leftToesT);
//             boneHead = new Bone(neckT, headT);
//             Debug.Log(" - 모든 Bone 객체 생성 완료.");

//             // --- 2. 테스트용 랜드마크 데이터 로드 ---
//             Debug.Log(" - 테스트 랜드마크 데이터 로드 시작...");
//             landmarks[Landmark.Nose] = new Vector3(0.5994642376899719f, 0.34193509817123413f, -0.12461798638105392f);
//             landmarks[Landmark.LeftShoulder] = new Vector3(0.6434094905853271f, 0.40489909052848816f, -0.053518664091825485f);
//             landmarks[Landmark.RightShoulder] = new Vector3(0.5827612280845642f, 0.4122781753540039f, 0.09672942012548447f);
//             landmarks[Landmark.LeftElbow] = new Vector3(0.683460533618927f, 0.3781096339225769f, -0.1641639620065689f);
//             landmarks[Landmark.RightElbow] = new Vector3(0.5399335622787476f, 0.41232290863990784f, 0.11780522018671036f);
//             landmarks[Landmark.LeftWrist] = new Vector3(0.673826277256012f, 0.2868189513683319f, -0.253589004278183f);
//             landmarks[Landmark.RightWrist] = new Vector3(0.541679322719574f, 0.3352228105068207f, 0.06141410022974014f);
//             landmarks[Landmark.LeftHip] = new Vector3(0.6191152334213257f, 0.6231337189674377f, -0.0432804599404335f);
//             landmarks[Landmark.RightHip] = new Vector3(0.5809868574142456f, 0.5994294881820679f, 0.04323191940784454f);
//             landmarks[Landmark.LeftKnee] = new Vector3(0.6063733696937561f, 0.7694557309150696f, -0.09366676211357117f);
//             landmarks[Landmark.RightKnee] = new Vector3(0.5398731231689453f, 0.6717719435691833f, -0.17873378098011017f);
//             landmarks[Landmark.LeftAnkle] = new Vector3(0.5968359708786011f, 0.9348261952400208f, -0.051896825432777405f);
//             landmarks[Landmark.RightAnkle] = new Vector3(0.5260046720504761f, 0.827457070350647f, -0.09467283636331558f);
//             landmarks[Landmark.LeftHeel] = new Vector3(0.5931583642959595f, 0.9527189135551453f, -0.054122187197208405f);
//             landmarks[Landmark.RightHeel] = new Vector3(0.5343356728553772f, 0.851600706577301f, -0.08930729329586029f);
//             // Foot Index 데이터가 없으면 Heel 좌표 사용
//             landmarks[Landmark.LeftFootIndex] = landmarks.ContainsKey(Landmark.LeftFootIndex) ? landmarks[Landmark.LeftFootIndex] : landmarks[Landmark.LeftHeel];
//             landmarks[Landmark.RightFootIndex] = landmarks.ContainsKey(Landmark.RightFootIndex) ? landmarks[Landmark.RightFootIndex] : landmarks[Landmark.RightHeel];
//             // 필요한 다른 랜드마크들도 추가... (Eye, Ear, Mouth 등)
//             Debug.Log(" - 테스트 랜드마크 데이터 로드 완료.");


//             // --- 3. 테스트 데이터 좌표 변환 ---
//             Debug.Log(" - 테스트 랜드마크 좌표 변환 시작...");
//             var keys = new List<Landmark>(landmarks.Keys);
//             foreach (var key in keys)
//             {
//                 // TryGetValue를 사용하여 키 존재 확인 및 값 가져오기 동시 수행
//                 if (landmarks.TryGetValue(key, out Vector3 pos))
//                 {
//                     pos.x = -pos.x;
//                     pos.z = -pos.z;
//                     landmarks[key] = pos; // 변환된 테스트 데이터 저장
//                 } else {
//                      Debug.LogWarning($"landmarks 딕셔너리에 키 [{key}]가 없습니다 (좌표 변환 중).");
//                 }
//             }
//              Debug.Log(" - 테스트 랜드마크 좌표 변환 완료.");


//             Debug.Log("LandmarkRig: 초기화 성공!");

//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"LandmarkRig 초기화 중 심각한 오류 발생: {e.Message}\n{e.StackTrace}");
//             enabled = false; // 오류 발생 시 스크립트 비활성화
//         }
//     }

//     void Update()
//     {
//         // landmarks 딕셔너리는 현재 테스트 데이터 또는 실시간 데이터를 담고 있음
//         // 이 함수가 호출되기 전에 UpdateLiveLandmarks 등을 통해 landmarks가 업데이트되어야 함

//         // --- 1. Torso 회전 계산 ---
//         // 현재 랜드마크 값으로 현재 Torso 벡터 계산
//         Vector3 neckPosLm = (landmarks[Landmark.RightShoulder] + landmarks[Landmark.LeftShoulder]) / 2f;
//         Vector3 hipsPosLm = (landmarks[Landmark.RightHip] + landmarks[Landmark.LeftHip]) / 2f;

//         Vector3 currentUpDownLm = Vector3.zero;
//         if (neckPosLm != hipsPosLm) currentUpDownLm = (neckPosLm - hipsPosLm).normalized;
//         if (currentUpDownLm == Vector3.zero) currentUpDownLm = transform.up; // Fallback

//         Vector3 currentTwistLm = Vector3.zero;
//         if (landmarks[Landmark.RightHip] != landmarks[Landmark.LeftHip]) currentTwistLm = (landmarks[Landmark.RightHip] - landmarks[Landmark.LeftHip]).normalized;
//          if (currentTwistLm == Vector3.zero) currentTwistLm = transform.right; // Fallback

//         Vector3 currentShoulderTwistLm = Vector3.zero;
//          if (landmarks[Landmark.RightShoulder] != landmarks[Landmark.LeftShoulder]) currentShoulderTwistLm = (landmarks[Landmark.RightShoulder] - landmarks[Landmark.LeftShoulder]).normalized;
//         if (currentShoulderTwistLm == Vector3.zero) currentShoulderTwistLm = transform.right; // Fallback


//         // 현재 랜드마크 벡터와 T-Pose 기준 벡터 비교하여 Delta 회전 계산
//         Quaternion deltaRotUpDown = Quaternion.identity;
//         // 초기 벡터와 현재 벡터 모두 유효할 때만 계산
//         if(initialTposeUpDown != Vector3.zero && currentUpDownLm != Vector3.zero)
//             deltaRotUpDown = Quaternion.FromToRotation(initialTposeUpDown, currentUpDownLm);

//         Quaternion deltaRotTwist = Quaternion.identity;
//          if(initialTposeTwist != Vector3.zero && currentTwistLm != Vector3.zero)
//             deltaRotTwist = Quaternion.FromToRotation(initialTposeTwist, currentTwistLm);

//         Quaternion deltaRotShoulderTwist = Quaternion.identity;
//          if(initialTposeShoulderTwist != Vector3.zero && currentShoulderTwistLm != Vector3.zero)
//              deltaRotShoulderTwist = Quaternion.FromToRotation(initialTposeShoulderTwist, currentShoulderTwistLm);


//         // T-Pose 기준 회전값에 Delta 회전 적용 (null 체크 추가)
//         Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
//         Transform spineTransform = animator.GetBoneTransform(HumanBodyBones.Spine);
//         Transform chestTransform = animator.GetBoneTransform(HumanBodyBones.Chest);

//         if(hipsTransform) hipsTransform.rotation = deltaRotUpDown * deltaRotTwist * initialHipRotation;
//         if(spineTransform) spineTransform.rotation = deltaRotUpDown * Quaternion.Slerp(deltaRotTwist, deltaRotShoulderTwist, 0.5f) * initialSpineRotation;
//         if(chestTransform) chestTransform.rotation = deltaRotUpDown * deltaRotShoulderTwist * initialChestRotation;

//         // --- 2. Limb 회전 업데이트 ---
//         // Bone.Update에 현재 랜드마크 위치 전달 (null-safe 연산자 ?. 사용)
//         boneRightUpperArm?.Update(landmarks[Landmark.RightShoulder], landmarks[Landmark.RightElbow], landmarks[Landmark.RightWrist]);
//         boneRightLowerArm?.Update(landmarks[Landmark.RightElbow], landmarks[Landmark.RightWrist], landmarks[Landmark.RightShoulder]);
//         boneLeftUpperArm?.Update(landmarks[Landmark.LeftShoulder], landmarks[Landmark.LeftElbow], landmarks[Landmark.LeftWrist]);
//         boneLeftLowerArm?.Update(landmarks[Landmark.LeftElbow], landmarks[Landmark.LeftWrist], landmarks[Landmark.LeftShoulder]);

//         boneRightUpperLeg?.Update(landmarks[Landmark.RightHip], landmarks[Landmark.RightKnee], landmarks[Landmark.RightAnkle]);
//         boneRightLowerLeg?.Update(landmarks[Landmark.RightKnee], landmarks[Landmark.RightAnkle], landmarks[Landmark.RightHip]);
//         boneLeftUpperLeg?.Update(landmarks[Landmark.LeftHip], landmarks[Landmark.LeftKnee], landmarks[Landmark.LeftAnkle]);
//         boneLeftLowerLeg?.Update(landmarks[Landmark.LeftKnee], landmarks[Landmark.LeftAnkle], landmarks[Landmark.LeftHip]);

//         // FootIndex 사용
//         boneRightFoot?.Update(landmarks[Landmark.RightAnkle], landmarks[Landmark.RightFootIndex], landmarks[Landmark.RightKnee]);
//         boneLeftFoot?.Update(landmarks[Landmark.LeftAnkle], landmarks[Landmark.LeftFootIndex], landmarks[Landmark.LeftKnee]);

//         // Head 업데이트 (계산된 목/엉덩이 위치 사용)
//         boneHead?.Update(neckPosLm, landmarks[Landmark.Nose], hipsPosLm);
//     }

//     /// <summary>
//     /// 외부에서 실시간 랜드마크 데이터를 받아 landmarks 딕셔너리를 업데이트합니다.
//     /// </summary>
//     /// <param name="liveData">새로운 랜드마크 데이터.</param>
//     public void UpdateLiveLandmarks(Dictionary<Landmark, Vector3> liveData)
//     {
//         var keys = new List<Landmark>(liveData.Keys);
//         foreach (var key in keys)
//         {
//             // landmarks 딕셔너리에 해당 키가 있는 경우에만 업데이트
//             if (landmarks.ContainsKey(key))
//             {
//                 Vector3 pos = liveData[key];
//                 // Start와 동일한 좌표 변환 적용
//                 pos.x = -pos.x;
//                 pos.z = -pos.z;
//                 landmarks[key] = pos; // Update()에서 사용할 딕셔너리 값 업데이트
//             }
//         }
//     }
// }


// // Bone 클래스: 뼈 하나의 초기 상태(T-Pose 기준)와 업데이트 로직을 관리합니다.
// // LandmarkRig 클래스 외부에 두거나 내부에 private으로 정의할 수 있습니다.
// // 여기서는 파일 내에 별도로 정의합니다.
// class Bone
// {
//     public Transform parent;            // 회전시킬 뼈의 Transform
//     public Vector3 initialDir;        // T-Pose 기준 뼈 방향 (parent->child)
//     public Quaternion initialRotation; // T-Pose 기준 뼈의 월드 회전
//     public Vector3 initialUp;         // T-Pose 기준 뼈의 Up 벡터 (단순화: parent.up)

//     /// <summary>
//     /// Bone 생성자. T-Pose 상태의 부모/자식 Transform을 받아 초기 상태를 설정합니다.
//     /// </summary>
//     /// <param name="parent">회전시킬 뼈의 Transform.</param>
//     /// <param name="child">자식 뼈의 Transform (초기 방향 계산용).</param>
//     public Bone(Transform parent, Transform child)
//     {
//         this.parent = parent;
//         if (parent == null) {
//              Debug.LogError("Bone 생성자에 parent가 null로 전달되었습니다!");
//              return;
//         }
//         this.initialRotation = parent.rotation; // T-Pose 회전 저장

//         // T-Pose 뼈 위치 기준으로 initialDir 계산
//         if (child != null)
//         {
//             this.initialDir = (child.position - parent.position).normalized;
//         }
//         else // 자식 뼈가 없는 경우 (예: Head)
//         {
//             this.initialDir = parent.forward; // 기본값으로 forward 사용
//             Debug.LogWarning($"[{parent.name}] 자식 Transform이 없어 parent.forward를 초기 방향으로 사용합니다. 필요 시 조정하세요.");
//         }

//         // T-Pose 기준 initialUp 계산 (가장 간단한 방법: parent의 up 벡터 사용)
//         // 더 정확한 방법이 필요하면 T-Pose 구조나 기준 랜드마크를 이용해 계산해야 함
//         this.initialUp = parent.up;

//         // 초기 벡터 유효성 검사
//         if (this.initialDir == Vector3.zero || float.IsNaN(this.initialDir.x))
//         {
//             Debug.LogError($"[{parent.name}] 초기 방향 벡터 계산 실패 (Zero or NaN). T-Pose 뼈 위치 확인 필요.", parent.gameObject);
//             this.initialDir = Vector3.forward; // 비상 대체
//         }
//         if (this.initialUp == Vector3.zero || float.IsNaN(this.initialUp.x))
//         {
//             // parent.up이 0인 경우는 거의 없지만, 만약을 위해 로그 추가
//             Debug.LogWarning($"[{parent.name}] 초기 Up 벡터가 Zero 또는 NaN입니다. (parent.up 사용)", parent.gameObject);
//             this.initialUp = Vector3.up; // 비상 대체
//         }
//     }

//     /// <summary>
//     /// 현재 랜드마크 위치를 기반으로 뼈의 회전을 업데이트합니다.
//     /// </summary>
//     /// <param name="parentPos">현재 부모 랜드마크 위치.</param>
//     /// <param name="childPos">현재 자식 랜드마크 위치.</param>
//     /// <param name="referencePos">현재 참조점 랜드마크 위치 (트위스트 계산용).</param>
//     public void Update(Vector3 parentPos, Vector3 childPos, Vector3 referencePos)
//     {
//         if (parent == null) return; // 안전 장치

//         // 1. 현재 방향 벡터 계산 (랜드마크 기준)
//         Vector3 currentDir = (childPos - parentPos).normalized;

//         // 현재 방향 벡터가 유효하지 않으면 업데이트 중단
//         if (currentDir == Vector3.zero || float.IsNaN(currentDir.x)) return;

//         // 2. 현재 '위쪽' 벡터 계산 (랜드마크 기준)
//         Vector3 currentPlaneNormal = Vector3.Cross(currentDir, referencePos - parentPos);
//         Vector3 currentUp = Vector3.Cross(currentPlaneNormal, currentDir).normalized;

//         // 3. 방향 변화량 계산 (T-Pose 기준 initialDir -> 현재 랜드마크 기준 currentDir)
//         Quaternion dirRotation = Quaternion.FromToRotation(initialDir, currentDir);

//         // 4. 현재 랜드마크가 일직선이어서 currentUp 계산이 불가능한 경우 처리
//         if (currentUp == Vector3.zero || float.IsNaN(currentUp.x))
//         {
//             // 방향 회전만 적용하고 종료 (T-Pose 기준 initialRotation 사용)
//             parent.rotation = dirRotation * initialRotation;
//             return;
//         }

//         // 5. 방향 회전만 적용했을 때의 '위쪽' 벡터 계산 (T-Pose 기준 initialUp을 dirRotation만큼 회전)
//         Vector3 intermediateUp = dirRotation * initialUp;

//         // 6. 트위스트 보정 회전량 계산 (intermediateUp -> currentUp)
//         Quaternion twistRotation = Quaternion.FromToRotation(intermediateUp, currentUp);

//         // 7. 최종 회전 적용 (T-Pose 기준 initialRotation에 방향 변화량과 트위스트 보정량을 순서대로 곱함)
//         parent.rotation = twistRotation * dirRotation * initialRotation;
//     }
// }

using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Vision.PoseLandmarker;

public class LandmarkRig : MonoBehaviour
{
    private Animator animator;

    private enum Landmark
    {
        Nose = 0, LeftEyeInner = 1, LeftEye = 2, LeftEyeOuter = 3, RightEyeInner = 4, RightEye = 5, RightEyeOuter = 6, LeftEar = 7, RightEar = 8, MouthLeft = 9, MouthRight = 10,
        LeftShoulder = 11, RightShoulder = 12, LeftElbow = 13, RightElbow = 14, LeftWrist = 15, RightWrist = 16, LeftPinky = 17, RightPinky = 18, LeftIndex = 19, RightIndex = 20, LeftThumb = 21, RightThumb = 22,
        LeftHip = 23, RightHip = 24, LeftKnee = 25, RightKnee = 26, LeftAnkle = 27, RightAnkle = 28, LeftHeel = 29, RightHeel = 30, LeftFootIndex = 31, RightFootIndex = 32
    }

    private Dictionary<Landmark, Vector3> landmarks = new Dictionary<Landmark, Vector3>();
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

        landmarks[Landmark.Nose] = new Vector3(0.5994642376899719f, 0.34193509817123413f, -0.12461798638105392f);
        landmarks[Landmark.LeftShoulder] = new Vector3(0.6434094905853271f, 0.40489909052848816f, -0.053518664091825485f);
        landmarks[Landmark.RightShoulder] = new Vector3(0.5827612280845642f, 0.4122781753540039f, 0.09672942012548447f);
        landmarks[Landmark.LeftElbow] = new Vector3(0.683460533618927f, 0.3781096339225769f, -0.1641639620065689f);
        landmarks[Landmark.RightElbow] = new Vector3(0.5399335622787476f, 0.41232290863990784f, 0.11780522018671036f);
        landmarks[Landmark.LeftWrist] = new Vector3(0.673826277256012f, 0.2868189513683319f, -0.253589004278183f);
        landmarks[Landmark.RightWrist] = new Vector3(0.541679322719574f, 0.3352228105068207f, 0.06141410022974014f);
        landmarks[Landmark.LeftHip] = new Vector3(0.6191152334213257f, 0.6231337189674377f, -0.0432804599404335f);
        landmarks[Landmark.RightHip] = new Vector3(0.5809868574142456f, 0.5994294881820679f, 0.04323191940784454f);
        landmarks[Landmark.LeftKnee] = new Vector3(0.6063733696937561f, 0.7694557309150696f, -0.09366676211357117f);
        landmarks[Landmark.RightKnee] = new Vector3(0.5398731231689453f, 0.6717719435691833f, -0.17873378098011017f);
        landmarks[Landmark.LeftAnkle] = new Vector3(0.5968359708786011f, 0.9348261952400208f, -0.051896825432777405f);
        landmarks[Landmark.RightAnkle] = new Vector3(0.5260046720504761f, 0.827457070350647f, -0.09467283636331558f);
        landmarks[Landmark.LeftHeel] = new Vector3(0.5931583642959595f, 0.9527189135551453f, -0.054122187197208405f);
        landmarks[Landmark.RightHeel] = new Vector3(0.5343356728553772f, 0.851600706577301f, -0.08930729329586029f);

        // 키 목록 따오기
        var keys = new List<Landmark>(landmarks.Keys);

        // 키로 직접 수정
        foreach (var key in keys)
        {
            Vector3 pos = landmarks[key];
            // pos.x = -pos.x;
            pos.x = pos.x;
            // pos.z = -pos.z;
            pos.z = pos.z;
            landmarks[key] = pos; // 수정한 값을 다시 집어넣기
        }

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
        Vector3 neckPos = (landmarks[Landmark.RightShoulder] + landmarks[Landmark.LeftShoulder]) / 2;
        Vector3 hipsPos = (landmarks[Landmark.RightHip] + landmarks[Landmark.LeftHip]) / 2;

        Vector3 currentUpDown = (neckPos - hipsPos).normalized;
        Quaternion deltaRotUpDown = Quaternion.FromToRotation(initialUpDown, currentUpDown);

        Vector3 currentTwist = (landmarks[Landmark.RightHip] - landmarks[Landmark.LeftHip]).normalized;
        Quaternion deltaRotTwist = Quaternion.FromToRotation(initialTwist, currentTwist);
        Vector3 currentShoulderTwist = (landmarks[Landmark.RightShoulder] - landmarks[Landmark.LeftShoulder]).normalized;
        Quaternion deltaRotShoulderTwist = Quaternion.FromToRotation(initialShoulderTwist, currentShoulderTwist);
        
        animator.GetBoneTransform(HumanBodyBones.Hips).rotation = deltaRotUpDown * deltaRotTwist * initialHipRotation;
        animator.GetBoneTransform(HumanBodyBones.Spine).rotation = deltaRotUpDown * Quaternion.Slerp(deltaRotTwist, deltaRotShoulderTwist, 0.5f) * initialSpineRotation;
        animator.GetBoneTransform(HumanBodyBones.Chest).rotation = deltaRotUpDown * deltaRotShoulderTwist * initialChestRotation;

        boneRightUpperLeg.Update(landmarks[Landmark.RightHip], landmarks[Landmark.RightKnee]);
        boneRightLowerLeg.Update(landmarks[Landmark.RightKnee], landmarks[Landmark.RightAnkle]);
        boneRightFoot.Update(landmarks[Landmark.RightAnkle], landmarks[Landmark.RightHeel]);
        boneLeftUpperLeg.Update(landmarks[Landmark.LeftHip], landmarks[Landmark.LeftKnee]);
        boneLeftLowerLeg.Update(landmarks[Landmark.LeftKnee], landmarks[Landmark.LeftAnkle]);
        boneLeftFoot.Update(landmarks[Landmark.LeftAnkle], landmarks[Landmark.LeftHeel]);   
        boneHead.Update(neckPos, landmarks[Landmark.Nose]);
        boneRightUpperArm.Update(landmarks[Landmark.RightShoulder], landmarks[Landmark.RightElbow]);
        boneRightLowerArm.Update(landmarks[Landmark.RightElbow], landmarks[Landmark.RightWrist]);
        boneLeftUpperArm.Update(landmarks[Landmark.LeftShoulder], landmarks[Landmark.LeftElbow]);
        boneLeftLowerArm.Update(landmarks[Landmark.LeftElbow], landmarks[Landmark.LeftWrist]);
    }

    public void SetPose(PoseLandmarkerResult result)
    {
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