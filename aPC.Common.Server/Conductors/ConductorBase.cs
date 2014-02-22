﻿using aPC.Common.Entities;
using aPC.Common.Server.EngineActors;
using aPC.Common.Server.Snapshots;
using aPC.Common.Server.SceneHandlers;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace aPC.Common.Server.Conductors
{
  public abstract class ConductorBase<T> where T : SnapshotBase
  {
    protected ConductorBase(EngineActorBase<T> xiActor, SceneHandlerBase<T> xiHandler)
    {
      mActor = xiActor;
      mHandler = xiHandler;
    }

    public void Run()
    {
      if (mHandler.IsDormant)
      {
        // qqUMI Ideally, we would just return here and disable the Conductor (should be more performant etc.
        // at the moment, this won't work, so for now we sleep
        Thread.Sleep(1000);
        // return;
      }
      else
      {
        var lSnapshot = mHandler.GetNextSnapshot();
        if (lSnapshot == null)
        {
          throw new InvalidOperationException("An error occured when retrieving the next snapshot");
        }
        mActor.ActNextFrame(lSnapshot);
        mHandler.AdvanceScene();
        WaitforInterval(lSnapshot.Length);
      }
    }

    public void UpdateScene(amBXScene xiScene)
    {
      mHandler.UpdateScene(xiScene);
    }

    protected void WaitforInterval(int xiLength)
    {
      Thread.Sleep(xiLength);
    }

    private EngineActorBase<T> mActor;
    private SceneHandlerBase<T> mHandler;
  }
}