﻿using aPC.Common;
using aPC.Server.EngineActors;
using aPC.Common.Entities;
using aPC.Server;
using aPC.Server.Engine;
using aPC.Server.Snapshots;

namespace aPC.Server.EngineActors
{
  class FanActor : ComponentActor<Fan>
  {
    public FanActor(IEngine xiEngine) : base (xiEngine)
    {
    }

    public override void ActNextFrame(eDirection xiDirection, ComponentSnapshot<Fan> xiSnapshot)
    {
      if (!xiSnapshot.IsComponentNull)
      {
        Engine.UpdateFan(xiDirection, xiSnapshot.Item);
      }
    }
  }
}
