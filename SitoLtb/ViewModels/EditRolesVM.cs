namespace SitoLtb.ViewModels
{
    public class EditRolesVM
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEditor { get; set; }
        public bool IsData { get; set; }
    }
}
