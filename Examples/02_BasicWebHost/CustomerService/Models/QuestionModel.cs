using CustomerService.Domain;

namespace CustomerService.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public QuestionStatus Status { get; set; }
    }
}
