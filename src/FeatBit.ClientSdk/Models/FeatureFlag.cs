namespace FeatBit.ClientSdk
{
    public class FeatureFlag
    {
        public string Id { get; set; }

        public string Variation { get; set; }

        public string VariationType { get; set; }

        public FeatureFlag()
        {

        }

        public FeatureFlag(
            string id,
            string variation,
            string variationType)
        {
            Id = id;
            Variation = variation;
            VariationType = variationType;
        }
    }
}