using System.Collections.Generic;

namespace HSM {
    public class TransitionSequencer
    {
        public readonly StateMachine Machine;
        public TransitionSequencer(StateMachine machine)
        {
            Machine = machine;
        }

        public void RequestTransition(State from, State to)
        {
            Machine.ChangeState(from, to);
        }

        // Returns lowest common ancestor of 2 states
        public static State Lca(State a,  State b)
        {
            var ap = new HashSet<State>();
            for (var s = a; s != null; s = s.Parent) { ap.Add(s); }
            
            for(var s = b; s != null; s = s.Parent)
            {
                if(ap.Contains(s)) return s;
            }
            return null;
        }
    }
}
