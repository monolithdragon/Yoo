using System;

namespace YooX.StateMachine {
    public class FuncPredicate : IPredicate {
        private readonly Func<bool> predicate;

        public FuncPredicate(Func<bool> predicate) {
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate.Invoke();
    }

}
