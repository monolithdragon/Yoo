namespace YooX.StateMachine {
    /// <summary>
    /// This class implements the <see cref="ITransition"/> interface, representing a transition
    /// between two states in a state machine. It holds the target state and the condition
    /// that must be met for the transition to occur.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the Transition class with a target state and a condition.
    /// </remarks>
    /// <param name="to">The target state to which the transition leads.</param>
    /// <param name="condition">The condition that must be satisfied for the transition to occur.</param>
    public class Transition : ITransition {
        /// <inheritdoc/>
        public IState To { get; }

        /// <inheritdoc/>
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition) {
            To = to;
            Condition = condition;
        }
    }

}
