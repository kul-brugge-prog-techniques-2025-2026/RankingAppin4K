namespace Models
{
    public class Subject
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Photo {  get; set; }
    }
}
