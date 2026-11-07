namespace ToyosiAndGIG.Domain.Entities
{
    public class Award
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Organization { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
