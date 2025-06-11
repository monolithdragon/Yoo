namespace YooX.StateMachine {
    /// <summary>
    /// This interface defines a contract for evaluating a condition. 
    /// It contains a single method, Evaluate, which returns a boolean value indicating 
    /// whether the condition is satisfied or not.
    /// </summary>
    public interface IPredicate {
        /// <summary>
        /// Evaluates the condition defined by the implementing class.
        /// </summary>
        /// <returns>True if the condition is met, otherwise false.</returns>
        bool Evaluate();
    }

}
