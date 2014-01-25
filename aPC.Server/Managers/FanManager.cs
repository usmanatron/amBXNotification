﻿using amBXLib;
using aPC.Common.Entities;
using aPC.Common.Integration;
using System.Linq;
using System;
using aPC.Common.Server.Managers;
using System.Collections.Generic;

namespace aPC.Server.Managers
{
  class FanManager : ManagerBase
  {
    public FanManager(CompassDirection xiDirection)
      : this(xiDirection, null)
    {
    }

    public FanManager(CompassDirection xiDirection, Action xiEventCallback)
      : base(xiEventCallback)
    {
      mDirection = xiDirection;
      SetupNewScene(CurrentScene);
    }

    // A scene is applicable if there is at least one non-null fan in a "somewhat" correct direction defined.
    protected override bool FramesAreApplicable(List<Frame> xiFrames)
    {
      var lFans = xiFrames
        .Select(frame => frame.Fans)
        .Where(section => section != null)
        .Select(section => CompassDirectionConverter.GetFan(mDirection, section));

      return lFans.Any(fan => fan != null);
    }

    public override Data GetNext()
    {
      var lFrame = GetNextFrame();
      var lFan = CompassDirectionConverter.GetFan(mDirection, lFrame.Fans);

      return new ComponentData(lFan, lFrame.Fans.FadeTime, lFrame.Length);
    }

    readonly CompassDirection mDirection;
  }
}