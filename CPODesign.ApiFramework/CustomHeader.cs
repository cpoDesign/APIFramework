namespace CPODesign.ApiFramework
{
    public struct CustomHeader
    {
        public string Name, Value;

        public CustomHeader(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}