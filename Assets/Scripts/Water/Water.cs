using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public struct Water {

        // Private fields
        private double volume;
        private double quality;
        private double maxCapacity;

        // Public properties
        public double Volume {
            get {
                return volume;
            }
            set {
                if (value < 0) {
                    throw new ArgumentException("Volume must be greater than 0. Tried to set volume " + value);
                }
                else if (MaxCapacity != null && value > MaxCapacity) {
                    throw new ArgumentException("Maximum volume is " + MaxCapacity.ToString() + ". Tried to set volume " + value);
                }
                else {
                    volume = value;
                }
            }
        }

        public double Quality {
            get {
                return quality;
            }
            set {
                if (value >= 0 && value <= 1) {
                    quality = value;
                }
                else {
                    throw new ArgumentException("The quality must be between 0 and 1 inclusive, value is " + value);
                }
            }
        }

        public double? MaxCapacity {
            get {
                if (maxCapacity == double.PositiveInfinity) {
                    return null;
                }
                else {
                    return maxCapacity;
                }
            }
            set {
                if (value == null) {
                    maxCapacity = double.PositiveInfinity;
                }
                else if (value >= Volume) {
                    maxCapacity = (double)value;
                }
                else {
                    throw new ArgumentException("Cannot set a maximum capacity lower than current volume");
                }                
            }
        }

        public double Level {
            get {
                double level = Volume / (double)MaxCapacity;

                if (level >= 0 && level <= 1) {
                    return level;
                }
                else {
                    throw new ArgumentException("The storage level must be between 0 and 1 inclusive, value is " + level + "Volume is " + Volume + "Capacity is " + MaxCapacity);
                }
            }
        }

        public double? RemainingCapacity {
            get {
                if (MaxCapacity == null) {
                    return null;
                }
                else if (MaxCapacity == 0) {
                    return 0;
                }
                else {
                    return MaxCapacity - Volume;
                }
            }
        }

        public double Product {
            get {
                return Quality * Volume;
            }
        }

        // Formatted strings
        public string FormattedFlow {
            get {
                return FormatFlow(Volume);
            }
        }

        public string FormattedVolume {
            get {
                return FormatVolume(Volume);
            }
        }

        public string FormattedQuality {
            get {
                return Quality.ToString("P2");
            }
        }

        public string FormattedLevel {
            get {
                return Level.ToString("P2");
            }
        }

        public string FormattedMaxFlow {
            get {
                return FormatFlow((double)MaxCapacity);
            }
        }

        public string FormattedMaxVolume {
            get {
                return FormatVolume((double)MaxCapacity);
            }
        }

        // formatting methods
        public static string FormatFlow(double flow) {
            if (flow < 300000) {
                return Math.Round(flow / 30).ToString() + " L/day";
            }
            else if (flow >= 300000 && flow < 30000000) {
                return Math.Round(flow / 30000).ToString("F0") + "k" + " L/day";
            }
            else if (flow >= 30000000) {
                return Math.Round(flow / 30000000).ToString("F0") + "M" + " L/day";
            }
            else {
                throw new ArgumentException("Something has gone wrong formatting the flow to a string");
            }
        }

        public static string FormatVolume(double volume) {
            if (volume < 10000) {
                return Math.Round(volume).ToString() + " L";
            }
            else if (volume >= 10000 && volume < 1000000) {
                return Math.Round(volume / 1000d).ToString("F0") + "k" + " L";
            }
            else if (volume >= 1000000) {
                return Math.Round(volume / 1000000d).ToString("F0") + "M" + " L";
            }
            else {
                throw new ArgumentException("Something has gone wrong formatting the max capacity to a string");
            }
        }

        // Operators
        public static Water operator +(Water a, Water b) {

            if (a.Volume + b.Volume > 0) {
                return new Water(a.Volume + b.Volume, (a.Product + b.Product) / (a.Volume + b.Volume), a.MaxCapacity);
            }
            else {
                return new Water(0, 1, a.MaxCapacity);
            }
        }

        public static Water operator -(Water source, Water abstraction) {

            if (abstraction.Quality == 1) {
                return new Water(source.Volume - abstraction.Volume, source.Quality, source.MaxCapacity);
            }
            //else if (source.Quality == 1) {
            //    return new Water(abstraction.Volume - source.Volume, abstraction.Quality);
            //}
            else {
                throw new ArgumentOutOfRangeException("The abstraction quality should be set at 1");
            }
        }

        public static Water operator -(Water source, double abstraction) {

            return new Water(source.Volume - abstraction, source.Quality, source.MaxCapacity);
        }

        // Constructors
        public Water(double volume, double quality) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            maxCapacity = double.PositiveInfinity;
            this.volume = volume;
        }

        public Water(double volume, double quality, double? maxCapacity) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            if (maxCapacity == null) {
                this.maxCapacity = double.PositiveInfinity;
            }
            else {
                this.maxCapacity = (double)maxCapacity;
            }

            if (volume > maxCapacity) {
                throw new ArgumentException("Maximum volume is " + maxCapacity.ToString());
            }
            this.volume = volume;
        }
    }
}

