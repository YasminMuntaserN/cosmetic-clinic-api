using System.Runtime.InteropServices.JavaScript;
using cosmeticClinic.DTOs.Common;

namespace cosmeticClinic.DTOs.Appointment;

public class AppointmentReportDto
{
    public DateTime Day { get; set; }
    public IEnumerable<ReportDto> Data { get; set; } = null!;
    
}