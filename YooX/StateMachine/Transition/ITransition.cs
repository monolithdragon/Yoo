namespace YooX.StateMachine {
    /// <summary>
    /// This interface represents a transition between states in a state machine.
    /// It defines properties for the target state and the condition that must be met
    /// for the transition to occur.
    /// </summary>
    public interface ITransition {
        /// <summary>
        /// Gets the target state to which the transition leads.
        /// The transition will occur to this state if the condition is met.
        /// </summary>
        IState To { get; }

        /// <summary>
        /// Gets the condition that must be evaluated to true for the transition to happen.
        /// The transition occurs only if this condition is satisfied.
        /// </summary>
        IPredicate Condition { get; }
    }

}
