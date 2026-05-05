namespace Library.LMS.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Content { get; set; }

        public Module()
        {
            Content = new List<string>();
        }

        // Deep copy constructor
        public Module(Module source) : this()
        {
            Name = source.Name;
            Content = new List<string>(source.Content);
        }
    }
}
