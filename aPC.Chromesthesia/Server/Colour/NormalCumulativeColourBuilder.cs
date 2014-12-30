﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aPC.Chromesthesia.Server.Colour
{
  internal class NormalCumulativeColourBuilder : IColourBuilder
  {
    //TODO: Do some research to determine if this truly is the maximum we should allow!
    private const int maximumNumberOfCDFTerms = 200;

    private readonly int mean;
    private readonly int deviation;

    public NormalCumulativeColourBuilder(int mean, int deviation)
    {
      if (ChromesthesiaConfig.NormalCDFNumberOfTerms > maximumNumberOfCDFTerms)
      {
        var message = string.Format("The config demands that we calculate more terms in the CDF function than is wise (max is {0}).  Aborting!",
          maximumNumberOfCDFTerms);
        throw new ArgumentException(message);
      }

      this.mean = mean;
      this.deviation = deviation;
    }

    public float GetValue(int index)
    {
      var rawValue = GetNormalCDFValue(index);

      return rawValue <= 0.5
        ? rawValue
        : 1 - rawValue;
    }

    /// <summary>
    /// We use the CDF function as defined on http://en.wikipedia.org/wiki/Normal_distribution#Cumulative_distribution_function.
    /// Absolute accuracy is not important here (speed is much more important), therefore, we use
    /// the "integration by parts" decomposition and only compute the first few terms.
    /// </summary>
    private float GetNormalCDFValue(float input)
    {
      var normalisedPoint = (input - mean) / deviation;
      var exponential = Math.Exp(-0.5 * normalisedPoint * normalisedPoint);

      var sequenceValue = 0d;

      for (int term = 0; term < ChromesthesiaConfig.NormalCDFNumberOfTerms; term++)
      {
        var power = (2 * term) + 1;
        var denominator = GetDoubleFactorial(power);
        sequenceValue += Math.Pow(normalisedPoint, power) / denominator;
      }

      return 0.5f + (float)(sequenceValue * exponential) / (float)Math.Sqrt(2 * Math.PI);
    }

    /// <summary>
    /// Defined here: http://en.wikipedia.org/wiki/Double_factorial
    /// Assumes the input is positive!
    /// </summary>
    private double GetDoubleFactorial(int input)
    {
      var value = 1;
      for (int term = input; term > 1; term = term - 2)
      {
        value *= term;
      }

      return value;
    }
  }
}