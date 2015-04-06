﻿using aPC.Chromesthesia.Server;
using aPC.Chromesthesia.Sound;
using aPC.Chromesthesia.Sound.Entities;
using aPC.Common;
using aPC.Common.Entities;
using aPC.Server;
using NAudio.Wave;
using System;
using System.Threading;

namespace aPC.Chromesthesia
{
  internal class SceneGenerator
  {
    private readonly PitchGeneratorProvider pitchGenerator;
    private readonly SceneBuilder sceneBuilder;
    private readonly NewSceneProcessor newSceneProcessor;

    public SceneGenerator(PitchGeneratorProvider pitchGenerator, SceneBuilder sceneBuilder, NewSceneProcessor newSceneProcessor)
    {
      this.pitchGenerator = pitchGenerator;
      this.sceneBuilder = sceneBuilder;
      this.newSceneProcessor = newSceneProcessor;
    }

    public void Execute(int readLength)
    {
      var results = GetResultsFromPitchGenerator(new byte[readLength], 0, readLength);

      // BuildScene
      var scene = sceneBuilder.BuildSceneFromPitchResults(results);

      if (!SceneIsEmpty(scene))
      {
        // Push changes
        newSceneProcessor.Process(scene);
      }

      return;
    }

    private StereoPitchResult GetResultsFromPitchGenerator(byte[] buffer, int offset, int count)
    {
      pitchGenerator.Read(buffer, offset, count);
      return pitchGenerator.PitchResults;
    }

    private bool SceneIsEmpty(amBXScene scene)
    {
      return LightIsEmpty((Light)scene.Frames[0].LightSection.GetComponentSectionInDirection(eDirection.East)) &&
             LightIsEmpty((Light)scene.Frames[0].LightSection.GetComponentSectionInDirection(eDirection.West));
    }

    private bool LightIsEmpty(Light light)
    {
      return Math.Abs(light.Red) < TOLERANCE && Math.Abs(light.Green) < TOLERANCE && Math.Abs(light.Blue) < TOLERANCE;
    }

    private const float TOLERANCE = 0.01f;

    public WaveFormat WaveFormat { get; private set; }
  }
}