using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public struct Water {

        // Private fields
        private float volume;
        private float quality;


        // Public properties
        public float? MaxCapacity { get; set; }

        public float Volume {
            get {
                return volume;
            }
            set {
                if (value < 0) {
                    throw new System.ArgumentException("Volume must be greater than 0. Tried to set volume " + value);
                }
                else if (MaxCapacity != null && value > MaxCapacity) {
                    throw new System.ArgumentException("Maximum volume is " + MaxCapacity.ToString() + ". Tried to set volume " + value);
                }
                else {
                    volume = value;
                }
            }
        }

        public float Quality {
            get {
                return quality;
            }
            set {
                if (value >= 0 && value <= 1) {
                    quality = value;
                }
                else {
                    throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + value);
                }
            }
        }

        public float Level {
            get {
                float level = Volume / (float)MaxCapacity;

                if (level >= 0 && level <= 1) {
                    return level;
                }
                else {
                    throw new System.ArgumentException("The storage level must be between 0 and 1 inclusive, value is " + level + "Volume is " + Volume + "Capacity is " + MaxCapacity);
                }
            }
        }

        public float? RemainingCapacity {
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

        public float Product {
            get {
                return Quality * Volume;
            }
        }

        // Formatted strings
        public string FormattedVolume {
            get {
                if (Volume < 300000) {
                    return Mathf.Round(Volume / 30f).ToString() + " L/day";
                }
                else if (Volume >= 300000 && Volume < 30000000) {
                    return Mathf.Round((Volume / 30000f)).ToString("F0") + "k" + " L/day";
                }
                else if (Volume >= 30000000) {
                    return Mathf.Round((Volume / 30000000f)).ToString("F0") + "M" + " L/day";
                }
                else {
                    throw new System.ArgumentException("Something has gone wrong formatting the supply to a string");
                }
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

        public string FormattedMaxCapacity {
            get {
                float maxCapacity = (float)MaxCapacity;

                if (maxCapacity < 300000) {
                    return Mathf.Round(maxCapacity / 30f).ToString() + " L/day";
                }
                else if (maxCapacity >= 300000 && maxCapacity < 30000000) {
                    return Mathf.Round((maxCapacity / 30000f)).ToString("F0") + "k" + " L/day";
                }
                else if (maxCapacity >= 30000000) {
                    return Mathf.Round((maxCapacity / 30000000f)).ToString("F0") + "M" + " L/day";
                }
                else {
                    throw new System.ArgumentException("Something has gone wrong formatting the max capacity to a string");
                }
            }
        }

        // Operators
        public static Water operator +(Water a, Water b) {

            if (a.Volume + b.Volume > 0) {
                return new Water(a.Volume + b.Volume, (a.Product + b.Product) / (a.Volume + b.Volume));
            }
            else {
                return new Water(0, 1);
            }
        }

        public static Water operator -(Water source, Water abstraction) {


            if (abstraction.Quality == 1) {
                return new Water(source.Volume - abstraction.Volume, source.Quality);
            }
            //else if (source.Quality == 1) {
            //    return new Water(abstraction.Volume - source.Volume, abstraction.Quality);
            //}
            else {
                throw new System.ArgumentOutOfRangeException("One of the water qualities when doing an abstraction should be 1");
            }
        }

        // Constructors
        public Water(float volume, float quality) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            MaxCapacity = null;
            this.volume = volume;
        }

        public Water(float volume, float quality, float maxCapacity) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            MaxCapacity = maxCapacity;

            if (volume > maxCapacity) {
                throw new System.ArgumentException("Maximum volume is " + MaxCapacity.ToString());
            }
            this.volume = volume;
        }
    }
}

