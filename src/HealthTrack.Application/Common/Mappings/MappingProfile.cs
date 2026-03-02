using AutoMapper;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Application.Features.Audit.DTOs;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Application.Features.Providers.DTOs;
using HealthTrack.Domain.Entities;

namespace HealthTrack.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone.Number))
            .ForMember(d => d.DateOfBirth, opt => opt.MapFrom(s => s.DateOfBirth.Value));

        CreateMap<Patient, PatientDetailDto>()
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone.Number))
            .ForMember(d => d.DateOfBirth, opt => opt.MapFrom(s => s.DateOfBirth.Value));

        // Provider mappings
        CreateMap<Provider, ProviderDto>();

        CreateMap<Provider, ProviderDetailDto>()
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone.Number));

        // AppointmentSlot mapping
        CreateMap<AppointmentSlot, AppointmentSlotDto>();

        // Appointment mapping
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.FullName))
            .ForMember(d => d.ProviderName, opt => opt.MapFrom(s => s.Provider.FullName));

        // Prescription mapping
        CreateMap<Prescription, PrescriptionDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.FullName))
            .ForMember(d => d.ProviderName, opt => opt.MapFrom(s => s.Provider.FullName))
            .ForMember(d => d.PharmacyName, opt => opt.MapFrom(s => s.Pharmacy != null ? s.Pharmacy.Name : null));

        // ClinicalNote mapping
        CreateMap<ClinicalNote, ClinicalNoteDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.FullName))
            .ForMember(d => d.ProviderName, opt => opt.MapFrom(s => s.Provider.FullName));

        // AuditLog mapping
        CreateMap<AuditLog, AuditLogDto>();

        // Pharmacy mapping
        CreateMap<Pharmacy, PharmacyDto>()
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone.Number));
    }
}
