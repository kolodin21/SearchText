namespace SearchText.Models
{
    public class Note
    {
        public Note(string title)
        {
            Title = title;
        }

        public int Id { get; set; }

        public  string Title { get; set; }
    }
}
