namespace BlazorChecklist.Shared
{
    public class Item
    {
        public long Id { get; set; }
        public string ChecklistName { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}