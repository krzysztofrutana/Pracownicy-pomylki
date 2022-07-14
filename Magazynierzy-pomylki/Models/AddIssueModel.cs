using System;

namespace Magazynierzy_pomylki.Models
{
    public class AddIssueModel
    {
        public string? UserName { get; set; }
        public int Change { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
