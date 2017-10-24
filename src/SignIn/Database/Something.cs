using Starcounter;

namespace SignIn
{
    [Database]
    public class Something
    {
        public bool Deleted { get; set; }
        public string Name { get; set; }

        public string Key
        {
            get { return this.GetObjectID(); }
        }
    }
}
