using System.Text.Json.Serialization;
namespace CoworkingFrontend.Models;

public enum Statut
{
    EnAttente,      // Waiting for approval
    Confirmé,       // Approved/Confirmed
    Rejetée,        // Rejected
    Annulée         // Cancelled
}

public enum UserRole
{
    Admin,//0
    Technicien,
    Etudiant
}
public class Abonnement
{
    public int Id { get; set; }
    public int EspaceId { get; set; }
    public int UtilisateurId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public Statut Statut { get; set; }
    public string? Notes { get; set; }
}

public class Espace
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Capacite { get; set; }
    public string Localisation { get; set; } = string.Empty;
}

public class Maintenance
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Statut { get; set; }
    public DateTime Date { get; set; }
    public int TechnicienId { get; set; }
    public int EspaceId { get; set; }
}

public class Reservation
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public int EspaceId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public Statut Statut { get; set; } = Statut.EnAttente; // Valeur par défaut
    public string? Notes { get; set; }
}

public class Ressource
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public int EspaceId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int QuantiteDisponible { get; set; }
}

public class Utilisateur
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
}



