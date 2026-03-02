using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.ValueObjects;
using HealthTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HealthTrack.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);

        if (await context.Providers.AnyAsync()) return;

        var providers = SeedProviders();
        context.Providers.AddRange(providers);
        await context.SaveChangesAsync();

        var pharmacies = SeedPharmacies();
        context.Pharmacies.AddRange(pharmacies);
        await context.SaveChangesAsync();

        var patients = SeedPatients();
        context.Patients.AddRange(patients);
        await context.SaveChangesAsync();

        var appointments = SeedAppointments(patients, providers);
        context.Appointments.AddRange(appointments);
        await context.SaveChangesAsync();

        var prescriptions = SeedPrescriptions(patients, providers, pharmacies);
        context.Prescriptions.AddRange(prescriptions);
        await context.SaveChangesAsync();

        var notes = SeedClinicalNotes(patients, providers, appointments);
        context.ClinicalNotes.AddRange(notes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Admin", "Provider", "Patient", "Nurse", "Staff"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new (string Email, string First, string Last, string Role)[]
        {
            ("admin@healthtrack.dev", "Admin", "User", "Admin"),
            ("provider@healthtrack.dev", "Dr. Sarah", "Chen", "Provider"),
            ("patient@healthtrack.dev", "John", "Smith", "Patient"),
        };

        foreach (var (email, first, last, role) in users)
        {
            if (await userManager.FindByEmailAsync(email) is not null) continue;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = first,
                LastName = last,
                Role = role
            };

            var result = await userManager.CreateAsync(user, "Demo123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }

    private static List<Provider> SeedProviders()
    {
        return
        [
            new Provider
            {
                FirstName = "Sarah",
                LastName = "Chen",
                Specialty = "Cardiology",
                LicenseNumber = "MD-2024-001",
                Email = "sarah.chen@healthtrack.dev",
                Phone = new PhoneNumber("(555) 100-0001"),
                IsAcceptingPatients = true
            },
            new Provider
            {
                FirstName = "James",
                LastName = "Wilson",
                Specialty = "Family Medicine",
                LicenseNumber = "MD-2024-002",
                Email = "james.wilson@healthtrack.dev",
                Phone = new PhoneNumber("(555) 100-0002"),
                IsAcceptingPatients = true
            },
            new Provider
            {
                FirstName = "Maria",
                LastName = "Rodriguez",
                Specialty = "Pediatrics",
                LicenseNumber = "MD-2024-003",
                Email = "maria.rodriguez@healthtrack.dev",
                Phone = new PhoneNumber("(555) 100-0003"),
                IsAcceptingPatients = true
            }
        ];
    }

    private static List<Pharmacy> SeedPharmacies()
    {
        return
        [
            new Pharmacy
            {
                Name = "HealthMart Pharmacy",
                Address = new Address("100 Main St", "Springfield", "IL", "62701"),
                Phone = new PhoneNumber("(555) 200-0001"),
                Fax = "(555) 200-0002",
                IsActive = true
            },
            new Pharmacy
            {
                Name = "CareFirst Drugs",
                Address = new Address("250 Oak Ave", "Springfield", "IL", "62702"),
                Phone = new PhoneNumber("(555) 200-0003"),
                Fax = "(555) 200-0004",
                IsActive = true
            },
            new Pharmacy
            {
                Name = "Wellness RX",
                Address = new Address("500 Elm St", "Springfield", "IL", "62703"),
                Phone = new PhoneNumber("(555) 200-0005"),
                IsActive = true
            },
            new Pharmacy
            {
                Name = "MedPlus Pharmacy",
                Address = new Address("75 Pine Rd", "Springfield", "IL", "62704"),
                Phone = new PhoneNumber("(555) 200-0006"),
                IsActive = true
            },
            new Pharmacy
            {
                Name = "QuickCare Pharmacy",
                Address = new Address("320 Maple Dr", "Springfield", "IL", "62705"),
                Phone = new PhoneNumber("(555) 200-0007"),
                IsActive = true
            }
        ];
    }

    private static List<Patient> SeedPatients()
    {
        return
        [
            new Patient
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = new DateOfBirth(new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Male,
                Email = "john.smith@email.com",
                Phone = new PhoneNumber("(555) 300-0001"),
                Address = new Address("123 Main St", "Springfield", "IL", "62701"),
                InsuranceInfo = new InsuranceInfo("BlueCross", "BC-12345", "GRP-001"),
                EmergencyContact = new EmergencyContact("Jane Smith", "Spouse", "(555) 300-0002"),
                MedicalHistory = "Hypertension, Type 2 Diabetes",
                Allergies = ["Penicillin", "Sulfa"],
                Medications = ["Metformin 500mg", "Lisinopril 10mg"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Emily",
                LastName = "Johnson",
                DateOfBirth = new DateOfBirth(new DateTime(1992, 7, 22, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Female,
                Email = "emily.johnson@email.com",
                Phone = new PhoneNumber("(555) 300-0003"),
                Address = new Address("456 Oak Ave", "Springfield", "IL", "62702"),
                InsuranceInfo = new InsuranceInfo("Aetna", "AE-67890", "GRP-002"),
                EmergencyContact = new EmergencyContact("Robert Johnson", "Father", "(555) 300-0004"),
                MedicalHistory = "Asthma",
                Allergies = ["Latex"],
                Medications = ["Albuterol inhaler"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Michael",
                LastName = "Williams",
                DateOfBirth = new DateOfBirth(new DateTime(1978, 11, 5, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Male,
                Email = "michael.williams@email.com",
                Phone = new PhoneNumber("(555) 300-0005"),
                Address = new Address("789 Elm St", "Springfield", "IL", "62703"),
                InsuranceInfo = new InsuranceInfo("UnitedHealth", "UH-11111", "GRP-003"),
                EmergencyContact = new EmergencyContact("Linda Williams", "Wife", "(555) 300-0006"),
                MedicalHistory = "Coronary artery disease, Hyperlipidemia",
                Allergies = [],
                Medications = ["Atorvastatin 40mg", "Aspirin 81mg", "Metoprolol 50mg"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Sophia",
                LastName = "Brown",
                DateOfBirth = new DateOfBirth(new DateTime(2015, 4, 10, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Female,
                Email = "sophia.brown.parent@email.com",
                Phone = new PhoneNumber("(555) 300-0007"),
                Address = new Address("321 Pine Rd", "Springfield", "IL", "62704"),
                InsuranceInfo = new InsuranceInfo("Cigna", "CI-22222", "GRP-004"),
                EmergencyContact = new EmergencyContact("David Brown", "Father", "(555) 300-0008"),
                MedicalHistory = "None significant",
                Allergies = ["Peanuts"],
                Medications = [],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Robert",
                LastName = "Davis",
                DateOfBirth = new DateOfBirth(new DateTime(1960, 8, 30, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Male,
                Email = "robert.davis@email.com",
                Phone = new PhoneNumber("(555) 300-0009"),
                Address = new Address("654 Maple Dr", "Springfield", "IL", "62705"),
                InsuranceInfo = new InsuranceInfo("Medicare", "MC-33333", "GRP-005"),
                EmergencyContact = new EmergencyContact("Susan Davis", "Daughter", "(555) 300-0010"),
                MedicalHistory = "COPD, Osteoarthritis, History of MI",
                Allergies = ["Codeine", "Ibuprofen"],
                Medications = ["Tiotropium inhaler", "Acetaminophen 500mg", "Clopidogrel 75mg"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Aisha",
                LastName = "Patel",
                DateOfBirth = new DateOfBirth(new DateTime(1988, 1, 18, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Female,
                Email = "aisha.patel@email.com",
                Phone = new PhoneNumber("(555) 300-0011"),
                Address = new Address("987 Cedar Ln", "Springfield", "IL", "62706"),
                InsuranceInfo = new InsuranceInfo("BlueCross", "BC-44444", "GRP-006"),
                EmergencyContact = new EmergencyContact("Raj Patel", "Husband", "(555) 300-0012"),
                MedicalHistory = "Gestational diabetes (resolved), Anxiety",
                Allergies = [],
                Medications = ["Sertraline 50mg"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Carlos",
                LastName = "Garcia",
                DateOfBirth = new DateOfBirth(new DateTime(1995, 6, 12, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Male,
                Email = "carlos.garcia@email.com",
                Phone = new PhoneNumber("(555) 300-0013"),
                Address = new Address("147 Birch St", "Springfield", "IL", "62707"),
                InsuranceInfo = new InsuranceInfo("Aetna", "AE-55555", "GRP-007"),
                EmergencyContact = new EmergencyContact("Maria Garcia", "Mother", "(555) 300-0014"),
                MedicalHistory = "ACL repair (2020)",
                Allergies = ["Amoxicillin"],
                Medications = [],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Wei",
                LastName = "Zhang",
                DateOfBirth = new DateOfBirth(new DateTime(1972, 9, 25, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Male,
                Email = "wei.zhang@email.com",
                Phone = new PhoneNumber("(555) 300-0015"),
                Address = new Address("258 Walnut Ave", "Springfield", "IL", "62708"),
                InsuranceInfo = new InsuranceInfo("UnitedHealth", "UH-66666", "GRP-008"),
                EmergencyContact = new EmergencyContact("Lin Zhang", "Wife", "(555) 300-0016"),
                MedicalHistory = "Hypertension, Gout",
                Allergies = [],
                Medications = ["Amlodipine 5mg", "Allopurinol 300mg"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Taylor",
                LastName = "Morgan",
                DateOfBirth = new DateOfBirth(new DateTime(2000, 12, 3, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.NonBinary,
                Email = "taylor.morgan@email.com",
                Phone = new PhoneNumber("(555) 300-0017"),
                Address = new Address("369 Spruce Ct", "Springfield", "IL", "62709"),
                InsuranceInfo = new InsuranceInfo("Cigna", "CI-77777", "GRP-009"),
                EmergencyContact = new EmergencyContact("Pat Morgan", "Parent", "(555) 300-0018"),
                MedicalHistory = "Migraines",
                Allergies = ["Aspirin"],
                Medications = ["Sumatriptan 50mg PRN"],
                IsActive = true
            },
            new Patient
            {
                FirstName = "Olivia",
                LastName = "Thompson",
                DateOfBirth = new DateOfBirth(new DateTime(1983, 5, 7, 0, 0, 0, DateTimeKind.Utc)),
                Gender = Gender.Female,
                Email = "olivia.thompson@email.com",
                Phone = new PhoneNumber("(555) 300-0019"),
                Address = new Address("482 Hickory Blvd", "Springfield", "IL", "62710"),
                InsuranceInfo = new InsuranceInfo("BlueCross", "BC-88888", "GRP-010"),
                EmergencyContact = new EmergencyContact("Mark Thompson", "Husband", "(555) 300-0020"),
                MedicalHistory = "Hypothyroidism, Depression",
                Allergies = ["Shellfish"],
                Medications = ["Levothyroxine 75mcg", "Bupropion 150mg"],
                IsActive = true
            }
        ];
    }

    private static List<Appointment> SeedAppointments(List<Patient> patients, List<Provider> providers)
    {
        var now = DateTime.UtcNow;
        return
        [
            // Completed appointments
            new Appointment { PatientId = patients[0].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(-30), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Completed, Type = AppointmentType.Consultation, Notes = "Initial cardiac evaluation" },
            new Appointment { PatientId = patients[0].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(-14), Duration = TimeSpan.FromMinutes(20), Status = AppointmentStatus.Completed, Type = AppointmentType.FollowUp, Notes = "Follow-up on blood pressure management" },
            new Appointment { PatientId = patients[1].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(-21), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Completed, Type = AppointmentType.Routine, Notes = "Annual physical" },
            new Appointment { PatientId = patients[2].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(-10), Duration = TimeSpan.FromMinutes(45), Status = AppointmentStatus.Completed, Type = AppointmentType.Consultation, Notes = "Cardiac follow-up post MI" },
            new Appointment { PatientId = patients[3].Id, ProviderId = providers[2].Id, ScheduledAt = now.AddDays(-7), Duration = TimeSpan.FromMinutes(20), Status = AppointmentStatus.Completed, Type = AppointmentType.Routine, Notes = "Well-child visit" },
            new Appointment { PatientId = patients[4].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(-5), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Completed, Type = AppointmentType.FollowUp, Notes = "COPD management review" },
            new Appointment { PatientId = patients[5].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(-3), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Completed, Type = AppointmentType.Telehealth, Notes = "Anxiety follow-up via telehealth" },

            // Cancelled / No-show
            new Appointment { PatientId = patients[6].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(-2), Duration = TimeSpan.FromMinutes(20), Status = AppointmentStatus.Cancelled, Type = AppointmentType.Routine, CancellationReason = "Patient requested cancellation" },
            new Appointment { PatientId = patients[7].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(-1), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.NoShow, Type = AppointmentType.FollowUp },

            // Scheduled upcoming
            new Appointment { PatientId = patients[0].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(3), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Scheduled, Type = AppointmentType.FollowUp, Notes = "Quarterly cardiac review" },
            new Appointment { PatientId = patients[1].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(5), Duration = TimeSpan.FromMinutes(20), Status = AppointmentStatus.Confirmed, Type = AppointmentType.PhoneCall, Notes = "Lab results discussion" },
            new Appointment { PatientId = patients[2].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(7), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Scheduled, Type = AppointmentType.InPerson },
            new Appointment { PatientId = patients[3].Id, ProviderId = providers[2].Id, ScheduledAt = now.AddDays(10), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Scheduled, Type = AppointmentType.Routine, Notes = "Vaccination appointment" },
            new Appointment { PatientId = patients[4].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(14), Duration = TimeSpan.FromMinutes(45), Status = AppointmentStatus.Scheduled, Type = AppointmentType.Consultation },
            new Appointment { PatientId = patients[5].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(7), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Scheduled, Type = AppointmentType.Telehealth },
            new Appointment { PatientId = patients[6].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(21), Duration = TimeSpan.FromMinutes(20), Status = AppointmentStatus.Scheduled, Type = AppointmentType.Routine },
            new Appointment { PatientId = patients[7].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(4), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Confirmed, Type = AppointmentType.FollowUp, Notes = "Hypertension management" },
            new Appointment { PatientId = patients[8].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(6), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Scheduled, Type = AppointmentType.Consultation, Notes = "Migraine evaluation" },
            new Appointment { PatientId = patients[9].Id, ProviderId = providers[1].Id, ScheduledAt = now.AddDays(8), Duration = TimeSpan.FromMinutes(20), Status = AppointmentStatus.Scheduled, Type = AppointmentType.FollowUp },

            // Recurring
            new Appointment { PatientId = patients[0].Id, ProviderId = providers[0].Id, ScheduledAt = now.AddDays(90), Duration = TimeSpan.FromMinutes(30), Status = AppointmentStatus.Scheduled, Type = AppointmentType.FollowUp, IsRecurring = true, RecurrencePattern = "Every 3 months" },
        ];
    }

    private static List<Prescription> SeedPrescriptions(List<Patient> patients, List<Provider> providers, List<Pharmacy> pharmacies)
    {
        return
        [
            new Prescription { PatientId = patients[0].Id, ProviderId = providers[0].Id, PharmacyId = pharmacies[0].Id, MedicationName = "Lisinopril", Dosage = "10mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-6), RefillsRemaining = 3, Status = PrescriptionStatus.Active, Notes = "For hypertension" },
            new Prescription { PatientId = patients[0].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[0].Id, MedicationName = "Metformin", Dosage = "500mg", Frequency = "Twice daily", StartDate = DateTime.UtcNow.AddMonths(-12), RefillsRemaining = 5, Status = PrescriptionStatus.Active, Notes = "For Type 2 Diabetes" },
            new Prescription { PatientId = patients[1].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[1].Id, MedicationName = "Albuterol Inhaler", Dosage = "90mcg", Frequency = "As needed", StartDate = DateTime.UtcNow.AddMonths(-3), RefillsRemaining = 2, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[2].Id, ProviderId = providers[0].Id, PharmacyId = pharmacies[0].Id, MedicationName = "Atorvastatin", Dosage = "40mg", Frequency = "Once daily at bedtime", StartDate = DateTime.UtcNow.AddMonths(-8), RefillsRemaining = 4, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[2].Id, ProviderId = providers[0].Id, PharmacyId = pharmacies[0].Id, MedicationName = "Aspirin", Dosage = "81mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-8), RefillsRemaining = 6, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[2].Id, ProviderId = providers[0].Id, PharmacyId = pharmacies[0].Id, MedicationName = "Metoprolol", Dosage = "50mg", Frequency = "Twice daily", StartDate = DateTime.UtcNow.AddMonths(-8), RefillsRemaining = 3, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[4].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[2].Id, MedicationName = "Tiotropium Inhaler", Dosage = "18mcg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-4), RefillsRemaining = 2, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[4].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[2].Id, MedicationName = "Clopidogrel", Dosage = "75mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-4), RefillsRemaining = 1, Status = PrescriptionStatus.RefillRequested },
            new Prescription { PatientId = patients[5].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[3].Id, MedicationName = "Sertraline", Dosage = "50mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-2), RefillsRemaining = 5, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[7].Id, ProviderId = providers[0].Id, PharmacyId = pharmacies[1].Id, MedicationName = "Amlodipine", Dosage = "5mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-5), RefillsRemaining = 4, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[7].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[1].Id, MedicationName = "Allopurinol", Dosage = "300mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-5), RefillsRemaining = 3, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[8].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[4].Id, MedicationName = "Sumatriptan", Dosage = "50mg", Frequency = "As needed for migraine", StartDate = DateTime.UtcNow.AddMonths(-1), RefillsRemaining = 2, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[9].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[3].Id, MedicationName = "Levothyroxine", Dosage = "75mcg", Frequency = "Once daily on empty stomach", StartDate = DateTime.UtcNow.AddMonths(-10), RefillsRemaining = 4, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[9].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[3].Id, MedicationName = "Bupropion", Dosage = "150mg", Frequency = "Once daily", StartDate = DateTime.UtcNow.AddMonths(-6), RefillsRemaining = 3, Status = PrescriptionStatus.Active },
            new Prescription { PatientId = patients[6].Id, ProviderId = providers[1].Id, PharmacyId = pharmacies[4].Id, MedicationName = "Ibuprofen", Dosage = "400mg", Frequency = "Every 6 hours as needed", StartDate = DateTime.UtcNow.AddMonths(-18), EndDate = DateTime.UtcNow.AddMonths(-15), RefillsRemaining = 0, Status = PrescriptionStatus.Completed, Notes = "Post ACL repair pain management" },
        ];
    }

    private static List<ClinicalNote> SeedClinicalNotes(List<Patient> patients, List<Provider> providers, List<Appointment> appointments)
    {
        return
        [
            new ClinicalNote
            {
                PatientId = patients[0].Id, ProviderId = providers[0].Id, AppointmentId = appointments[0].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Patient reports occasional chest tightness during exercise. No shortness of breath at rest. Compliant with current medications.",
                Objective = "BP 138/88, HR 72, RR 16. Heart sounds regular, no murmurs. Lungs clear bilaterally.",
                Assessment = "Essential hypertension, partially controlled. Type 2 diabetes, stable on current regimen.",
                Plan = "Continue Lisinopril 10mg daily. Consider dose increase if BP remains elevated at next visit. Order HbA1c and lipid panel. Follow up in 3 months."
            },
            new ClinicalNote
            {
                PatientId = patients[0].Id, ProviderId = providers[0].Id, AppointmentId = appointments[1].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Patient reports improved exercise tolerance. No chest pain. Occasional dizziness in the morning.",
                Objective = "BP 130/82, HR 68. Weight stable. Labs: HbA1c 6.8%, LDL 110.",
                Assessment = "Hypertension improving. Diabetes well controlled.",
                Plan = "Maintain current medications. Discuss dietary modifications. Follow up in 3 months."
            },
            new ClinicalNote
            {
                PatientId = patients[1].Id, ProviderId = providers[1].Id, AppointmentId = appointments[2].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Patient here for annual physical. Reports occasional wheezing with exercise. Using inhaler 1-2 times per week.",
                Objective = "BP 118/74, HR 76. Lungs: mild expiratory wheeze bilaterally. Peak flow 85% predicted.",
                Assessment = "Mild persistent asthma, partially controlled.",
                Plan = "Continue albuterol PRN. Consider adding low-dose ICS if symptoms increase. Spirometry in 6 months."
            },
            new ClinicalNote
            {
                PatientId = patients[2].Id, ProviderId = providers[0].Id, AppointmentId = appointments[3].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Follow-up post MI (6 months ago). Patient reports good exercise tolerance, walking 30 min daily. No chest pain or dyspnea.",
                Objective = "BP 125/78, HR 60. Heart sounds regular. Surgical scar healing well. ECG: normal sinus rhythm.",
                Assessment = "Post-MI recovery progressing well. CAD stable on current therapy.",
                Plan = "Continue current medications. Cardiac rehabilitation program ongoing. Follow up in 3 months with stress test."
            },
            new ClinicalNote
            {
                PatientId = patients[3].Id, ProviderId = providers[2].Id, AppointmentId = appointments[4].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Well-child visit. Parent reports normal growth and development. Good appetite, active play.",
                Objective = "Weight 20kg (50th percentile), Height 110cm (55th percentile). HEENT normal. Heart and lungs clear. Abdomen soft.",
                Assessment = "Healthy child, appropriate growth and development for age.",
                Plan = "Routine vaccinations per schedule. Anticipatory guidance for nutrition and safety. Return in 1 year."
            },
            new ClinicalNote
            {
                PatientId = patients[4].Id, ProviderId = providers[1].Id, AppointmentId = appointments[5].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "COPD follow-up. Patient reports stable symptoms. Using tiotropium daily. Occasional morning cough productive of white sputum.",
                Objective = "BP 140/85, HR 80, O2 sat 94% RA. Lungs: diminished breath sounds bases, scattered rhonchi.",
                Assessment = "COPD, moderate, stable. Osteoarthritis, bilateral knees.",
                Plan = "Continue tiotropium. Encourage smoking cessation. Pulmonary rehab referral. Annual flu vaccine."
            },
            new ClinicalNote
            {
                PatientId = patients[5].Id, ProviderId = providers[1].Id, AppointmentId = appointments[6].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Telehealth follow-up for anxiety. Patient reports improvement on sertraline. Sleeping better, fewer panic episodes.",
                Objective = "Patient appears calm and well-groomed via video. Speech normal rate and rhythm. Affect appropriate. GAD-7 score: 8 (mild).",
                Assessment = "Generalized anxiety disorder, improving on current SSRI.",
                Plan = "Continue Sertraline 50mg daily. Continue cognitive behavioral therapy. Follow up in 2 months."
            },
            new ClinicalNote
            {
                PatientId = patients[7].Id, ProviderId = providers[0].Id,
                NoteType = NoteType.Progress, Version = 1,
                Subjective = "Patient reports increased joint pain in right big toe. Last gout flare 2 months ago.",
                Objective = "BP 135/82. Right 1st MTP joint: mild swelling, no erythema currently. Uric acid: 6.8 mg/dL.",
                Assessment = "Gout, controlled on allopurinol. Hypertension, stable.",
                Plan = "Continue current medications. Dietary counseling for purine reduction. Recheck uric acid in 3 months."
            },
            new ClinicalNote
            {
                PatientId = patients[8].Id, ProviderId = providers[1].Id,
                NoteType = NoteType.Consultation, Version = 1,
                Subjective = "New patient consultation for chronic migraines. Reports 4-5 episodes per month, each lasting 6-12 hours. Photophobia and nausea associated.",
                Objective = "Neuro exam: CN II-XII intact. No papilledema. No focal deficits. BP 122/76.",
                Assessment = "Chronic migraine without aura. Currently using sumatriptan for acute episodes.",
                Plan = "Continue sumatriptan PRN. Start migraine diary. Consider prophylactic therapy if frequency doesn't improve. MRI brain to rule out secondary causes."
            },
            new ClinicalNote
            {
                PatientId = patients[9].Id, ProviderId = providers[1].Id,
                NoteType = NoteType.SOAP, Version = 1,
                Subjective = "Follow-up for hypothyroidism and depression. Patient reports stable energy levels. Mood improved since starting bupropion.",
                Objective = "BP 120/76, HR 72. Thyroid non-tender, no nodules palpable. TSH: 2.4 (normal). PHQ-9: 6 (mild).",
                Assessment = "Hypothyroidism well controlled. Depression improving on bupropion.",
                Plan = "Continue Levothyroxine 75mcg and Bupropion 150mg. Recheck TSH in 6 months. Follow up in 3 months."
            }
        ];
    }
}
