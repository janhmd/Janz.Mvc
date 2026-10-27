namespace Janz.Mvc.ViewModels.Tags
{
    // Admin: teqlər bir səhifədədir — yuxarıda yeni teq, cədvəldə adı dəyişmək və silmək
    public class TagListVM
    {
        public List<TagVM> Tags { get; set; } = new List<TagVM>();
        public string? NewName { get; set; }
        public int? EditedId { get; set; }      // səhv olan sətir: yazılan ad itməsin
        public string? EditedName { get; set; }
    }
}
