using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk.Evaluation
{
    internal class EvalResult
    {
        public bool IsValid { get; set; }

        public string Reason { get; set; }

        public string Value { get; set; }

        private EvalResult(bool isValid, string reason, string value)
        {
            Reason = reason;
            Value = value;
        }

        // Indicates that the caller provided a flag key that did not match any known flag.
        public static readonly EvalResult FlagNotFound =
            new EvalResult(false, "flag not found", string.Empty);

        public static EvalResult Of(FeatureFlag flag)
        {
            return new EvalResult(true, flag.MatchReason, flag.Variation);
        }
    }
}