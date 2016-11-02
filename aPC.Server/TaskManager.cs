﻿using aPC.Common;
using aPC.Common.Entities;
using aPC.Server.Engine;
using aPC.Server.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aPC.Server
{
  // Handles the masses of tasks flying around.
  public class TaskManager
  {
    private readonly EngineActor engineActor;
    private readonly DirectionalComponentTaskList directionalComponentActionList;
    private eSceneType runningSceneType;

    public TaskManager(EngineActor engineActor, DirectionalComponentTaskList directionalComponentActionList)
    {
      this.engineActor = engineActor;
      this.directionalComponentActionList = directionalComponentActionList;
    }

    public void RefreshTasks(RunningDirectionalComponentList runningDirectionalComponentList)
    {
      runningSceneType = runningDirectionalComponentList.SceneType;
      var components = runningDirectionalComponentList.Get(runningSceneType);

      switch (runningSceneType)
      {
        case eSceneType.Desync:
          foreach (var directionalComponent in components)
          {
            ReScheduleTask(directionalComponent);
          }
          break;
        case eSceneType.Sync:
        case eSceneType.Event:
          directionalComponentActionList.CancelAll();
          ScheduleTask(components.Single(), 0);
          break;
      }
    }

    private void ReScheduleTask(RunningDirectionalComponent runningComponent)
    {
      directionalComponentActionList.Cancel(runningComponent.DirectionalComponent);
      ScheduleTask(runningComponent, 0);
    }

    private void RunFrameForDirectionalComponent(RunningDirectionalComponent runningComponent, CancellationTokenSource cancellationToken)
    {
      directionalComponentActionList.Remove(cancellationToken);

      var frame = GetFrame(runningComponent);

      if (runningSceneType == eSceneType.Desync)
      {
        var component = frame.GetComponentInDirection(runningComponent.DirectionalComponent.ComponentType, runningComponent.DirectionalComponent.Direction);
        engineActor.UpdateComponent(component);
      }
      else
      {
        foreach (eComponentType componentType in Enum.GetValues(typeof(eComponentType)))
          foreach (eDirection direction in EnumExtensions.GetCompassDirections())
          {
            var component = frame.GetComponentInDirection(componentType, direction);
            if (component != null)
            {
              engineActor.UpdateComponent(component);
            }
          }
      }

      DoPostUpdateActions(runningComponent, frame.Length);
    }

    private Frame GetFrame(RunningDirectionalComponent runningComponents)
    {
      var frames = runningComponents.Ticker.IsFirstRun
        ? runningComponents.Scene.Frames
        : runningComponents.Scene.RepeatableFrames;
      return frames[runningComponents.Ticker.Index];
    }

    private void DoPostUpdateActions(RunningDirectionalComponent runningComponent, int delay)
    {
      runningComponent.Ticker.Advance();

      // When we've run the scene once through, we need to check that there are either:
      // * repeatable frames
      // * that it's not an event
      // If neither of these hold, then we terminate running by NOT scheduling the next task.
      if (runningComponent.Ticker.Index == 0 && (runningComponent.Scene.SceneType == eSceneType.Event || runningComponent.Scene.RepeatableFrames.Count == 0))
      {
        return;
      }

      ScheduleTask(runningComponent, delay);
    }

    private void ScheduleTask(RunningDirectionalComponent runningComponent, int delay)
    {
      var cancellationToken = new CancellationTokenSource();
      Task.Run(async delegate
                     {
                       await Task.Delay(TimeSpan.FromMilliseconds(delay), cancellationToken.Token);
                       RunFrameForDirectionalComponent(runningComponent, cancellationToken);
                     }, cancellationToken.Token);

      directionalComponentActionList.Add(new DirectionalComponentTask(cancellationToken, runningComponent.DirectionalComponent));
    }
  }
}