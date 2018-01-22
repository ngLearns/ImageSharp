﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using SixLabors.ImageSharp.MetaData.Profiles.Icc;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion.Implementation.Icc.Calculators
{
    internal partial class CurveCalculator : ISingleCalculator
    {
        private LutCalculator lutCalculator;
        private float gamma;
        private CalculationType type;

        public CurveCalculator(IccCurveTagDataEntry entry, bool inverted)
        {
            if (entry.IsIdentityResponse)
            {
                this.type = CalculationType.Identity;
            }
            else if (entry.IsGamma)
            {
                this.gamma = entry.Gamma;
                if (inverted)
                {
                    this.gamma = 1f / this.gamma;
                }

                this.type = CalculationType.Gamma;
            }
            else
            {
                this.lutCalculator = new LutCalculator(entry.CurveData, inverted);
                this.type = CalculationType.Lut;
            }
        }

        public float Calculate(float value)
        {
            switch (this.type)
            {
                case CalculationType.Identity:
                    return value;

                case CalculationType.Gamma:
                    return (float)Math.Pow(value, this.gamma);

                case CalculationType.Lut:
                    return this.lutCalculator.Calculate(value);

                default:
                    throw new InvalidOperationException("Invalid calculation type");
            }
        }
    }
}