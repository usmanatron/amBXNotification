﻿using aPC.Common;
using aPC.Common.Entities;
using System;
using System.Threading;

namespace aPC.Server.Engine
{
  public class EngineActor
  {
    private readonly AmbxEngineWrapper ambxEngineWrapper;

    public EngineActor(AmbxEngineWrapper ambxEngineWrapper)
    {
      this.ambxEngineWrapper = ambxEngineWrapper;
    }

    public void UpdateComponent(DirectionalComponent component)
    {
      switch (component.ComponentType)
      {
        case eComponentType.Light:
          ThreadPool.QueueUserWorkItem(_ => ambxEngineWrapper.UpdateLight(component.GetLight()));
          break;

        case eComponentType.Fan:
          ThreadPool.QueueUserWorkItem(_ => ambxEngineWrapper.UpdateFan(component.GetFan()));
          break;

        case eComponentType.Rumble:
          ThreadPool.QueueUserWorkItem(_ => ambxEngineWrapper.UpdateRumble(component.GetRumble()));
          break;

        default:
          throw new ArgumentException("Unexpected Component Type");
      }
    }
  }
}