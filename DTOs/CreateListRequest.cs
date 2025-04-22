namespace MiniProjet.DTOs
{
    public class CreateListeRequest
    {
        public string Nom { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
    }
}
