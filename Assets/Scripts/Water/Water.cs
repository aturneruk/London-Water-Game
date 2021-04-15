using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public struct Water {
        private float volume;
        private float quality;
        private bool groundwater;

        public float Volume {
            get {
                return volume;
            }
            set {
                volume = value;

                if (groundwater == true) {
                    if (volume > 5000000000) {
                        throw new System.ArgumentException("Maximum volume is 5e9 L");
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

        public Water(float volume, float quality) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            groundwater = false;
            this.volume = volume;
        }

        public Water(float volume, float quality, bool groundwater) {

            if (quality >= 0 && quality <= 1) {
                this.quality = quality;
            }
            else {
                throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + quality);
            }

            if (groundwater == true) {
                if (volume > 5000000000) {
                    throw new System.ArgumentException("Maximum volume is 5e9 L");
                }
                this.groundwater = true;
            }
            else {
                this.groundwater = false;
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

