using System.ComponentModel.DataAnnotations;
using NodaTime;

/// <summary>
///     Base class that provides common fields to all Entities.
/// </summary>
public class BaseEntity
{
	/// <summary>
	///     The time this entity was created.
	/// </summary>
	[Required]
	public Instant CreatedAt { get; set; } = SystemClock.Instance.GetCurrentInstant();

	/// <summary>
	///     The time this entity was updated.
	/// </summary>
	[Required]
	public Instant UpdatedAt { get; set; } = SystemClock.Instance.GetCurrentInstant();

	/// <summary>
	///     The time this entity was deleted.
	/// </summary>
	public Instant? DeletedAt { get; set; }

	public override bool Equals(object? obj)
	{
		return obj is BaseEntity entity
			&& CreatedAt == entity.CreatedAt
			&& UpdatedAt == entity.UpdatedAt
			&& DeletedAt == entity.DeletedAt;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(CreatedAt, UpdatedAt, DeletedAt);
	}
}
