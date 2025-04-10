namespace MiniProjet.DTOs
{
    public class UpdateListeDTO
    {
        public string Nom { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public string CreateurId { get; set; }
        public List<string> LieuxIds { get; set; }
    }
}
