// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mediapipe.Unity.Sample.PoseLandmarkDetection
{
  public class PoseLandmarkResultRunner : VisionTaskApiRunner<PoseLandmarker>
  {
    private Experimental.TextureFramePool _textureFramePool;
    [SerializeField] private LandmarkRig landmarkRig;

    public override void Stop()
    {
      base.Stop();
      _textureFramePool?.Dispose();
      _textureFramePool = null;
    }

    protected override IEnumerator Run()
    {
      const string modelPath = "pose_landmarker_full.bytes";
      yield return AssetLoader.PrepareAssetAsync(modelPath);

      var options = new PoseLandmarkerOptions(
        new Tasks.Core.BaseOptions(modelAssetPath: modelPath),
        runningMode: Tasks.Vision.Core.RunningMode.VIDEO
      );
      taskApi = PoseLandmarker.CreateFromOptions(options, GpuManager.GpuResources);
      var imageSource = ImageSourceProvider.ImageSource;

      yield return imageSource.Play();
      if (!imageSource.isPrepared)
      {
        Logger.LogError(TAG, "Failed to start ImageSource, exiting...");
        yield break;
      }

      // Use RGBA32 as the input format.
      _textureFramePool = new Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight);
      // NOTE: The screen will be resized later, keeping the aspect ratio.
      screen.Initialize(imageSource);

      var transformationOptions = imageSource.GetTransformationOptions();
      var flipHorizontally = transformationOptions.flipHorizontally;
      var flipVertically = transformationOptions.flipVertically;
      var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: 0);

      AsyncGPUReadbackRequest req = default;
      var waitUntilReqDone = new WaitUntil(() => req.done);
      var result = PoseLandmarkerResult.Alloc(options.numPoses, options.outputSegmentationMasks);

      while (true)
      {
        if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
        {
          yield return new WaitForEndOfFrame();
          continue;
        }

        // Build the input Image
        Image image;
        req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
        yield return waitUntilReqDone;

        if (req.hasError) continue;
        image = textureFrame.BuildCPUImage();
        textureFrame.Release();

        if (taskApi.TryDetectForVideo(image, GetCurrentTimestampMillisec(), imageProcessingOptions, ref result))
        {
          landmarkRig.SetPose(result);
        }
        DisposeAllMasks(result);
      }
    }

    private void DisposeAllMasks(PoseLandmarkerResult result)
    {
      if (result.segmentationMasks != null)
      {
        foreach (var mask in result.segmentationMasks)
        {
          mask.Dispose();
        }
      }
    }
  }
}
