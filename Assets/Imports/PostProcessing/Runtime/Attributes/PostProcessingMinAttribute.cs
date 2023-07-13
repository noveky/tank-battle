namespace UnityEngine.PostProcessing
{
    public sealed class PostProcessingMinAttribute : PropertyAttribute
    {
        public readonly float min;

        public PostProcessingMinAttribute(float min)
        {
            this.min = min;
        }
    }
}
