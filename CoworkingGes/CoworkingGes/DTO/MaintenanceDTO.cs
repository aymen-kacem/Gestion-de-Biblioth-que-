namespace CoworkingGes.DTO
{
    public class MaintenanceDTO
    {
        
        public string Description { get; set; }
        public string Statut { get; set; } 
        public DateTime Date { get; set; }
        public int TechnicienId { get; set; }
        public int EspaceId { get; set; }
    }
}
