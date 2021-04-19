using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public struct Water {
        private float volume;
        private float quality;

        public float Volume {
            get {
                return volume;
            }
            set {
                volume = value;

                if (MaxCapacity != null) {
                    if (volume > MaxCapacity) {
                        throw new System.ArgumentException("Maximum volume is " + MaxCapacity.ToString());
                    }
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
                    throw new System.ArgumentException("The storage level must be between 0 and 1 inclusive, value is " + level);
                }
            }
        }

        public float? MaxCapacity { get; private set; }

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

        public float Product {
            get {
                return Quality * Volume;
            }
        }
    }
}

