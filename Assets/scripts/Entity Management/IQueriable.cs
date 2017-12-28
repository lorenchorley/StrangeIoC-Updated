using UnityEngine;
using System.Collections;

namespace eventsourcing {

    public interface IQueriable<Q> where Q : IQuery {
        void Query(Q q);
    }

}