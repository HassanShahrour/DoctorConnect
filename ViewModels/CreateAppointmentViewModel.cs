using DoctorConnect.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.ViewModels
{
    public class CreateAppointmentViewModel
    {
        public string? Id { get; set; }
        public string DoctorId { get; set; }
        public string? PatientId { get; set; }
        public bool IsGuestPatient { get; set; }
        public string? GuestPatientFullName { get; set; }
        public string? GuestPatientPhoneNumber { get; set; }
        public Gender? GuestPatientGender { get; set; }
        public DateTime? GuestPatientDateOfBirth { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? Notes { get; set; }
        public decimal? Fees { get; set; }
        public AppointmentStatus Status { get; set; }

        public List<SelectListItem> Clinics { get; set; } = new();
        public string? SelectedClinicId { get; set; }

        public static CreateAppointmentViewModel FromAppointment(Appointment appt)
        {
            return new CreateAppointmentViewModel
            {
                Id = appt.Id,
                DoctorId = appt.DoctorId,
                PatientId = appt.PatientId,
                IsGuestPatient = appt.IsGuestPatient,
                GuestPatientFullName = appt.GuestPatientFullName,
                GuestPatientPhoneNumber = appt.GuestPatientPhoneNumber,
                GuestPatientGender = appt.GuestPatientGender,
                GuestPatientDateOfBirth = appt.GuestPatientDateOfBirth,
                AppointmentDate = appt.AppointmentDate,
                AppointmentTime = appt.AppointmentTime,
                Notes = appt.Notes,
                Fees = appt.Fees,
                Status = appt.Status
            };
        }
    }
}
