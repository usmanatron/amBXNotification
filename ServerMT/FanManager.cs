﻿using amBXLib;
using Common.Entities;
using Common.Integration;
using System.Collections.Generic;
using System.Linq;
using Common.Server.Managers;

namespace ServerMT
{
  class FanManager : ManagerBase<Fan>
  {
    public FanManager(CompassDirection xiDirection) : base()
    {
      mDirection = xiDirection;
      base.SetupNewScene(mCurrentScene);
    }

    // A scene is applicable if there is at least one non-null fan in a "somewhat" correct direction defined.
    protected override bool SceneIsApplicable(amBXScene xiNewScene)
    {
      var lFans = xiNewScene
        .Frames
        .Select(frame => frame.Fans)
        .Where(fans => fans != null)
        .Select(cnt => CompassDirectionConverter.GetFan(mDirection, cnt));

      return lFans.Any(fan => fan != null);
    }

    public override Data<Fan> GetNext()
    {
      var lFrame = GetNextFrame();
      var lFan = CompassDirectionConverter.GetFan(mDirection, lFrame.Fans);

      return new Data<Fan>(lFan, lFrame.Fans.FadeTime, lFrame.Length);
    }

    CompassDirection mDirection;
  }
}