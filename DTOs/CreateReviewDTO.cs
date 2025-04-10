namespace MiniProjet.DTOs
{
    public class CreateReviewDTO
    {
        public string Commentaire { get; set; }
        public int Note { get; set; }
        public string UserId { get; set; }
        public string PlaceId { get; set; }
    }
}
