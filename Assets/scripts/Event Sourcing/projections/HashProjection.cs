using UnityEngine;
using System.Collections;

namespace eventsourcing.examples.network {

    public class HashProjection : IProjection {

        private int hashcode;
        private EventSource ES;

        public HashProjection(EventSource ES) {
            this.ES = ES;
        }

        public void Reset() {
            hashcode = 0;
        }
        
        public bool Process(IEvent e) {
            hashcode += e.GetHashCode();
            return true;
        }

        public void OnFinish() {
            hashcode = hashcode.GetHashCode();

            Debug.Log("Hash results: " + hashcode + " from " + ES.EventCount + " events");

        }

        public override int GetHashCode() {
            return hashcode;
        }

    }

}