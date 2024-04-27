using FeatBit.ClientSdk.Store;

namespace FeatBit.ClientSdk.Evaluation
{
    internal interface IEvaluator
    {
        EvalResult Evaluate(string key);
    }

    internal class Evaluator : IEvaluator
    {
        private readonly IMemoryStore _store;

        public Evaluator(IMemoryStore store)
        {
            _store = store;
        }

        public EvalResult Evaluate(string key)
        {
            var flag = _store.Get(key);

            return flag == null
                ? EvalResult.FlagNotFound
                : EvalResult.Of(flag);
        }
    }
}