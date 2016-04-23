﻿namespace aPC.Common.Communication
{
  public static class CommunicationSettings
  {
    public static string GetServiceUrl(string hostname, eApplicationType applicationType)
    {
      return @"http://" + hostname + @"/" + applicationType.ToString();
    }
  }

  public enum eApplicationType
  {
    amBXPeripheralController,
    aPCTest
  }
}