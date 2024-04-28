namespace FeatBit.Sdk.Client.Evaluation
{
    public sealed class EvalDetail<TValue>
    {
        /// <summary>
        /// A string describing the main factor that influenced the flag evaluation value.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The result of the flag evaluation. This will be either one of the flag's variations or the default
        /// value that was specified when the flag was evaluated.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Constructs a new EvalDetail instance.
        /// </summary>
        /// <param name="reason">the evaluation reason</param>
        /// <param name="value">the flag value</param>
        public EvalDetail(string reason, TValue value)
        {
            Reason = reason;
            Value = value;
        }
    }
}