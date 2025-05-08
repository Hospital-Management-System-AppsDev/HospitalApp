namespace HospitalApp.Models;

public class PatientMedicalInfo
{
    public string diet { get; set; }
    public string exercise { get; set; }
    public string sleep { get; set; }
    public string smoking { get; set; }
    public string alcohol { get; set; }
    public string currentMedication { get; set; }
    public string medicalAllergies { get; set; }
    public bool latexAllergy { get; set; }
    public string foodAllergy { get; set; }
    public string otherAllergies { get; set; }
}