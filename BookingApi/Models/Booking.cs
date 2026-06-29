using System.ComponentModel.DataAnnotations;

namespace BookingApi.Models;

public class Booking
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public Resource? Resource { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    [ConcurrencyCheck]
    public int Version { get; set; }
    
    public Booking()
    {
        CreatedAt = DateTime.UtcNow;

    }

    protected bool Equals(Booking other)
    {
        return Id.Equals(other.Id) && ResourceId.Equals(other.ResourceId) && StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate) && CreatedAt.Equals(other.CreatedAt) && BookedBy == other.BookedBy;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Booking)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ResourceId, StartDate, EndDate, CreatedAt, BookedBy);
    }
}