namespace Models
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Photo {  get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
