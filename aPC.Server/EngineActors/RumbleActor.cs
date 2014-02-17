﻿using aPC.Common;
using aPC.Common.Server.EngineActors;
using aPC.Common.Entities;
using aPC.Common.Server.Managers;
using aPC.Server.Managers;
using System;

namespace aPC.Server.EngineActors
{
  class RumbleActor : ComponentActor
  {
    public RumbleActor(eDirection xiDirection, EngineManager xiEngine) 
      : base (xiDirection, xiEngine)
    {
    }

    public override void ActNextFrame(Snapshot xiData)
    {
      var lRumbleData = (ComponentSnapshot)xiData;

      if (!lRumbleData.IsComponentNull)
      {
        Engine.UpdateRumble(mDirection, (Rumble)lRumbleData.Component);
      }
    }
  }
}
