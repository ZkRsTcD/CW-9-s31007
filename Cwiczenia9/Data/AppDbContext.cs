using Cwiczenia9.Models;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia9.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription_Medicament> Prescription_Medicaments { get; set; }
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var doctor = (
            new Doctor { IdDoctor = 1, FirstName = "Jan", LastName = "Kowalski", Email = "bije.zone@o2.com" }
        );

        var medicament = (
            new Medicament { IdMedicament = 1, Name = "Ibuprofen", Description = "BijeBul", Type = "Tabletka" }
        );

        var patient = (
            new Patient { IdPatient = 1, FirstName = "Mariusz", LastName = "Pudzianowski", Birthdate = new DateTime(1995, 5, 15) }
        );
        
        var prescription = ( 
            new Prescription 
            { 
                IdPrescription = 1, 
                Date = new DateTime(2025, 5, 28), 
                DueDate = new DateTime(2025, 6, 28), 
                IdDoctor = 1, 
                IdPatient = 1 
            });

        var prescriptionMedicament = (
            new Prescription_Medicament
            {
                IdMedicament = 1,
                IdPrescription = 1,
                Dose = 2,
                Details = "Jak sie zachce"
            });
        
        modelBuilder.Entity<Patient>().HasData(patient);
        modelBuilder.Entity<Patient>().HasData(medicament);
        modelBuilder.Entity<Patient>().HasData(doctor);
        modelBuilder.Entity<Patient>().HasData(prescription);
        modelBuilder.Entity<Patient>().HasData(prescriptionMedicament);
    }
    
}