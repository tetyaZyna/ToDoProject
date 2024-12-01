namespace ToDo.Api.Models;

public class ToDoModel
    {
        public int Id { get; set; }
        private DateTime _expiryDate;
        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set => _expiryDate = value.ToUniversalTime();
        }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int CompletionPercentage { get; set; }
    }
