namespace CleanArchitecture.Domain.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
