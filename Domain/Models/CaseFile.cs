namespace Domain.Models
{
    public struct CaseFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public string FullName { get; private set; }

        public CaseFile(string filePath, string name, string extension)
        {
            Path = filePath;
            Name = name;
            Extention = extension;
            FullName = $"{Name}{Extention}";
        }
    }
}
