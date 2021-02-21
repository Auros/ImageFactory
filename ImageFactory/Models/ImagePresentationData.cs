namespace ImageFactory.Models
{
    public class ImagePresentationData
    {
        public virtual string PresentationID { get; set; } = null!;

        // Zero Duration will be infinite
        public virtual float Duration { get; set; } = 0;
    }
}