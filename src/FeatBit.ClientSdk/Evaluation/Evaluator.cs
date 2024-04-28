using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Store;

namespace FeatBit.ClientSdk.Evaluation
{
    internal interface IEvaluator
    {
        (EvalResult evalResult, FeatureFlag flag) Evaluate(string key);
    }

    internal sealed class Evaluator : IEvaluator
    {
        private readonly IMemoryStore _store;

        public Evaluator(IMemoryStore store)
        {
            _store = store;
        }

        public (EvalResult evalResult, FeatureFlag flag) Evaluate(string key)
        {
            var flag = _store.Get(key);

            return flag == null
                ? (EvalResult.FlagNotFound, null)
                : (EvalResult.Of(flag), flag);
        }
    }
}