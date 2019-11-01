namespace Kesco.Lib.Win.Data.DALC.Documents.Search
{
    public enum Groups
    {
        Document = 0,
        EForm = 1
    }

    public class Group
    {
        private string name;
        private Option[] options;

        public Group(string name, Option[] options)
        {
            this.name = name;
            this.options = options;
        }

        public string Name
        {
            get { return name; }
        }

        public Option[] Options
        {
            get { return options; }
        }

    }
}