using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;
using static DoctorConnect.Models.Enums;

namespace DoctorConnect.DbServices.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IGenericRepository<Doctor> _doctors;
        private readonly IGenericRepository<Patient> _patients;
        private readonly IGenericRepository<Appointment> _appointments;

        public DashboardService(IGenericRepository<Doctor> doctors, IGenericRepository<Patient> patients, IGenericRepository<Appointment> appointments)
        {
            _doctors = doctors;
            _patients = patients;
            _appointments = appointments;
        }

        public async Task<int> GetCompletedAppointmentsAsync()
        {
            var all = await _appointments.GetAllAsync();
            return all.Count(a => a.Status == AppointmentStatus.Completed);
        }

        public async Task<int> GetPendingAppointmentsAsync()
        {
            var all = await _appointments.GetAllAsync();
            return all.Count(a => a.Status == AppointmentStatus.Pending);
        }

        public async Task<int> GetTotalAppointmentsAsync()
        {
            var all = await _appointments.GetAllAsync();
            return all.Count();
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            var all = await _doctors.GetAllAsync();
            return all.Count();
        }

        public async Task<int> GetTotalPatientsAsync()
        {
            var all = await _patients.GetAllAsync();
            return all.Count();
        }
    }
}
