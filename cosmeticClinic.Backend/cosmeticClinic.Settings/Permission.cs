namespace cosmeticClinic.Settings;

public enum Permission
{
    ViewDoctors =  1 << 0,        
    CreateDoctor = 1 << 1,       
    MangeDoctor = 1 << 2,       
    DeleteDoctor = 1 << 3,       
      
    
    ViewAppointments =  1 << 4,
    CreateAppointment = 1 << 5,
    MangeAppointment = 1 << 6,
    CancelAppointment = 1 << 7,
    
    ViewPatients = 1 << 8,
    CreatePatient = 1 << 9,
    MangePatient = 1 << 10,
    DeletePatient = 1 << 11, 
    
    ViewTreatments = 1 << 12,
    CreateTreatment = 1 << 13,
    MangeTreatment = 1 << 14,
    DeleteTreatment = 1 << 15,
    
    ViewProducts = 1 << 16,
    CreateProduct = 1 << 17,
    MangeProduct = 1 << 18,
    DeleteProduct = 1 << 19,
    
    ViewReports = 1 << 20,
    ManageUsers = 1 << 21
}