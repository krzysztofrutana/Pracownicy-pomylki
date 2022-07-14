using System;

namespace Magazynierzy_pomylki.Models
{
    public class IssueModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Change { get; set; }
        public int ChangeAsNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueDateAsString => IssueDate.ToString("dd.MM.yyyy");
        public string? Description { get; set; }
        public bool IsDriverExplainNeed { get; set; }
        public bool ExplainedAtDriver { get; set; }
        public string? DriverExplain { get; set; }
        public string? CreatedBy { get; set; }
        public bool WithoutrDriverExplain
        {
            get
            {
                if (IsDriverExplainNeed && !ExplainedAtDriver)
                    return true;
                else
                    return false;
            }
        }

        public System.Windows.Media.Brush Color
        {
            get
            {
                if (WithoutrDriverExplain)
                    return System.Windows.Media.Brushes.Orange;
                else if (ExplainedAtDriver)
                    return System.Windows.Media.Brushes.GreenYellow;
                else
                    return System.Windows.Media.Brushes.White;
            }
        }
    }
}
