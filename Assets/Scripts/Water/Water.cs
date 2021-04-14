using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public struct Water
    {
        private float volume;
        private float quality;

        public float Volume {
            get {
                return volume;
            }
            set {
                volume = value;
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

        public Water(float volume, float quality) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            this.volume = volume;
        }

    }
}

