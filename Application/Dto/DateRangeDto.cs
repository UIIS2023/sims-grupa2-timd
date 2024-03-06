using System;

namespace SimsProject.Application.Dto
{
    public class DateRangeDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateRangeDto() { }
        public DateRangeDto(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        public override string ToString()
        {
            return StartDate.ToString("dd.MM.yyyy.") + " - " + EndDate.ToString("dd.MM.yyyy.");
        }
    }
}
