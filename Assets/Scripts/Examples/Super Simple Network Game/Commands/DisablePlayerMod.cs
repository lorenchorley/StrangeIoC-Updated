using UnityEngine;
using System;

namespace eventsourcing.examples.network {

    [Serializable]
    public class DisablePlayerMod : IEMInjected, IEventProducing {

        private IEvent _Event;
        public IEvent Event { get { return _Event; } }

        private EntityManager _EM;
        public EntityManager EM { set { _EM = value; } }

        public bool RecordEvent { get; set; }

        public void Execute() {
            _Event = PlayerEntity.ApplyMod(_EM, this);
        }

    }

}