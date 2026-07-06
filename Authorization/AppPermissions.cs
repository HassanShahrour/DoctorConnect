namespace DoctorConnect.Authorization
{
    public static class AppPermissions
    {
        public static class Doctors
        {
            public const string Read = "Doctors.Read";
            public const string Create = "Doctors.Create";
            public const string Update = "Doctors.Update";
            public const string Delete = "Doctors.Delete";
        }

        public static class Patients
        {
            public const string Read = "Patients.Read";
            public const string Create = "Patients.Create";
            public const string Update = "Patients.Update";
            public const string Delete = "Patients.Delete";
        }

        public static class Appointments
        {
            public const string Read = "Appointments.Read";
            public const string Create = "Appointments.Create";
            public const string Update = "Appointments.Update";
            public const string Delete = "Appointments.Delete";
        }

        public static class Services
        {
            public const string Read = "Services.Read";
            public const string Create = "Services.Create";
            public const string Update = "Services.Update";
            public const string Delete = "Services.Delete";
        }

        public static class Clinics
        {
            public const string Read = "Clinics.Read";
            public const string Create = "Clinics.Create";
            public const string Update = "Clinics.Update";
            public const string Delete = "Clinics.Delete";
        }

        public static class Specialities
        {
            public const string Read = "Specialities.Read";
            public const string Create = "Specialities.Create";
            public const string Update = "Specialities.Update";
            public const string Delete = "Specialities.Delete";
        }

        public static class Tasks
        {
            public const string Read = "Tasks.Read";
            public const string Create = "Tasks.Create";
            public const string Update = "Tasks.Update";
            public const string Delete = "Tasks.Delete";
        }

        public static class Settings
        {
            public const string Read = "Settings.Read";
            public const string Update = "Settings.Update";
        }

        public static class Dashboard
        {
            public const string Admin = "Dashboard.Admin";
            public const string Doctor = "Dashboard.Doctor";
            public const string Patient = "Dashboard.Patient";
        }

        public static class RolePermissions
        {
            public const string Read = "RolePermissions.Read";
            public const string Update = "RolePermissions.Update";
        }

        public static IReadOnlyList<string> All => new[]
        {
            Doctors.Read, Doctors.Create, Doctors.Update, Doctors.Delete,
            Patients.Read, Patients.Create, Patients.Update, Patients.Delete,
            Appointments.Read, Appointments.Create, Appointments.Update, Appointments.Delete,
            Services.Read, Services.Create, Services.Update, Services.Delete,
            Clinics.Read, Clinics.Create, Clinics.Update, Clinics.Delete,
            Specialities.Read, Specialities.Create, Specialities.Update, Specialities.Delete,
            Tasks.Read, Tasks.Create, Tasks.Update, Tasks.Delete,
            Settings.Read, Settings.Update,
            Dashboard.Admin, Dashboard.Doctor, Dashboard.Patient,
            RolePermissions.Read, RolePermissions.Update
        };
    }
}
