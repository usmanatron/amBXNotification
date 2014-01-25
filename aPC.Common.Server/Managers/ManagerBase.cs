﻿using aPC.Common.Accessors;
using aPC.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aPC.Common.Server.Managers
{
  public abstract class ManagerBase
  {
    protected ManagerBase()
    {
      var lScene = new SceneAccessor().GetScene("Empty");

      PreviousScene = lScene;
      CurrentScene = lScene;
    }

    protected ManagerBase(Action xiEventCallback) : this()
    {
      mEventCallback = xiEventCallback;
    }

    public void UpdateScene(amBXScene xiNewScene)
    {
      lock (mSceneLock)
      {
        if (xiNewScene.IsEvent && CurrentScene.IsEvent)
        {
          // Skip updating the previous scene, to ensure that we don't get 
          // stuck in an infinite loop of events.
        }
        else
        {
          PreviousScene = CurrentScene;
        }

        SetupNewScene(xiNewScene);
      }
    }

    protected void SetupNewScene(amBXScene xiNewScene)
    {
      if (FramesAreApplicable(xiNewScene.Frames))
      {
        IsDormant = false;
        CurrentScene = xiNewScene;
        Ticker = new AtypicalFirstRunInfiniteTicker(CurrentScene.Frames.Count, CurrentScene.RepeatableFrames.Count);
      }
      else
      {
        if (FramesAreApplicable(PreviousScene.Frames))
        {
          return;
        }
        else
        {
          IsDormant = true;
        }
      }
    }

    protected abstract bool FramesAreApplicable(List<Frame> xiFrames);

    public abstract Data GetNext();

    protected Frame GetNextFrame()
    {
      List<Frame> lFrames;

      lock (mSceneLock)
      {
        lFrames = Ticker.IsFirstRun
          ? CurrentScene.Frames
          : CurrentScene.RepeatableFrames;
      }

      if (!lFrames.Any())
      {
        // This can only happen in one of two cases:
        // * This isn't an event and all frames are not repeatable.
        // * There aren't any frames at all (though this should never happen)
        // Either way, return a frame which specifies everything off (as a failsafe)
        return new FrameAccessor().AllOff;
      }
      return lFrames[Ticker.Index];
    }

    public void AdvanceScene()
    {
      lock (mSceneLock)
      {
        Ticker.Advance();

        // If we've run the scene once through, we need to check for a few special circumstances
        if (Ticker.Index == 0)
        {
          DoSceneCompletedChecks();
        }
      }
    }

    private void DoSceneCompletedChecks()
    {
      if (CurrentScene.IsEvent)
      {
        // The event has completed one full cycle.  Revert to
        // previous scene
        SetupNewScene(PreviousScene);
        if (mEventCallback != null)
        {
          mEventCallback();
        }
      }
      else if (FramesAreApplicable(CurrentScene.RepeatableFrames))
      {
        IsDormant = true;
      }
    }

    public bool IsDormant;
    protected amBXScene CurrentScene;
    protected amBXScene PreviousScene;
    protected AtypicalFirstRunInfiniteTicker Ticker;

    private readonly object mSceneLock = new object();
    private Action mEventCallback;
  }
}