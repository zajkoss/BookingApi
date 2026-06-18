namespace BookingApi.Models;

public class Resource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }

    public Resource()
    {
    }

    protected bool Equals(Resource other)
    {
        return Id.Equals(other.Id) && Name == other.Name && Description == other.Description &&
               Capacity == other.Capacity && IsActive == other.IsActive;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Resource)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Description, Capacity, IsActive);
    }

    public override string ToString()
    {
        return
            $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Capacity)}: {Capacity}, {nameof(IsActive)}: {IsActive}";
    }
}