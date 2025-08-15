using System;
using System.Collections.Generic;
using System.Linq;

// a) Generic Repository<T>
public class Repository<T>
{
    private readonly List<T> items = new();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new(items);

    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item == null) return false;
        return items.Remove(item);
    }
}

// b) Patient
public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
        => (Id, Name, Age, Gender) = (id, name, age, gender);

    public override string ToString() => $"[{Id}] {Name}, {Age}, {Gender}";
}

// c) Prescription
public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string name, DateTime dateIssued)
        => (Id, PatientId, MedicationName, DateIssued) = (id, patientId, name, dateIssued);

    public override string ToString()
        => $"Rx #{Id} for Patient {PatientId}: {MedicationName} ({DateIssued:d})";
}

public class HealthSystemApp
{
    // g) Fields
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    // Methods
    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Smith", 28, "F"));
        _patientRepo.Add(new Patient(2, "Bob Johnson", 41, "M"));
        _patientRepo.Add(new Patient(3, "Cynthia Doe", 35, "F"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-7)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Today.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Cetirizine", DateTime.Today.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(104, 3, "Vitamin D", DateTime.Today.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(105, 3, "Paracetamol", DateTime.Today));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    // f) Helper
    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        => _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach (var p in _patientRepo.GetAll())
            Console.WriteLine("  " + p);
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        var prescriptions = GetPrescriptionsByPatientId(id);
        Console.WriteLine($"\nPrescriptions for Patient {id}:");
        if (prescriptions.Count == 0) Console.WriteLine("  (none)");
        foreach (var rx in prescriptions) Console.WriteLine("  " + rx);
    }
}

public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(1);
    }
}

