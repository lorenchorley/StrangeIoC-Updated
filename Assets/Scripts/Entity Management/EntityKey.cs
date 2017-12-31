using UnityEngine;
using System.Collections;

namespace eventsourcing {

    // Allows fast access and reference replacement for entities, bypasses the need for a dictionary lookup
    // Good for entities that are structs, since they need to be entirely written back into the registry when changes are made
    public struct EntityKey {

        public int Index;
        public IEntityRegistry Registry;

    }

}