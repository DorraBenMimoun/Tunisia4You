namespace MiniProjet.DTOs
{
    public class ReviewDTO
    {
        public string? Id { get; set; }
        public string Commentaire { get; set; }
        public int Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string PlaceId { get; set; }
    }
}
