using Cwiczenia9.Data;
using Cwiczenia9.DTOs;
using Cwiczenia9.Exceptions;
using Cwiczenia9.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia9.Services;

public interface IDbService
{
    Task<IActionResult> AddPrescriptionAsync(PrescriptionRequestDto request);
    Task<IActionResult> GetPatientDetailsAsync(int patientId);
}

public class DbService(AppDbContext data) : IDbService
{
    public async Task<IActionResult> AddPrescriptionAsync(PrescriptionRequestDto request)
    {
        if (request.Medicaments.Count > 10)
        {
            return new BadRequestObjectResult("Recepta może obejmować maksymalnie 10 leków");
        }

        if (request.DueDate < request.Date)
        {
            return new BadRequestObjectResult("Wrong date");
        }
        
        var doctor = await data.Doctors.FindAsync(request.IdDoctor);
        if (doctor == null)
        {
            return new NotFoundObjectResult("Doctor not found");
        }

        Patient patient = null;
        if (request.Patient.IdPatient != 0)
        {
            patient = await data.Patients.FindAsync(request.Patient.IdPatient);
        }

        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = request.Patient.FirstName,
                LastName = request.Patient.LastName,
                Birthdate = request.Patient.Birthdate
            };
            data.Patients.Add(patient);
            await data.SaveChangesAsync();
        }

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            IdDoctor = request.IdDoctor,
            //IdPatient = request.IdPatient,
            Prescription_Medicaments = new List<Prescription_Medicament>()
        };
        foreach (var med in request.Medicaments)
        {
            var medicament = await data.Medicaments.FindAsync(med.IdMedicament);
            if (medicament == null)
            {
                return new NotFoundObjectResult("Medicament not found");
            }

            prescription.Prescription_Medicaments.Add(new Prescription_Medicament
            {
                IdMedicament = med.IdMedicament,
                Dose = med.Dose,
                Details = med.Description
            });
        }
        data.Prescriptions.Add(prescription);
        await data.SaveChangesAsync();
        
        return new OkObjectResult(prescription);
    }

    public async Task<IActionResult> GetPatientDetailsAsync(int patientId)
    {
        var patient = await data.Patients.Where(p => p.IdPatient == patientId).Select(p => new PatientDetailsDto
        {
            IdPatient = p.IdPatient,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Birthdate = p.Birthdate,
            Prescriptions = p.Prescriptions.OrderBy(l => l.DueDate).Select(l => new PrescriptionDto
            {
                IdPrescription = l.IdPrescription,
                Date = l.Date,
                DueDate = l.DueDate,
                Medicaments = l.Prescription_Medicaments.Select(k => new MedicamentDto
                {
                    IdMedicament = k.IdMedicament,
                    Dose = k.Dose,
                    Description = k.Details
                }).ToList(),
                Doctor = new DoctorDto
                {
                    IdDoctor = l.IdDoctor,
                    FirstName = l.Doctor.FirstName,
                    LastName = l.Doctor.LastName,
                    Email = l.Doctor.Email,
                }
            }).ToList(),
        }).FirstOrDefaultAsync();

        if (patient == null)
        {
            return new NotFoundObjectResult("Patient not found");
        }
        return new OkObjectResult(patient);
    }
    
}