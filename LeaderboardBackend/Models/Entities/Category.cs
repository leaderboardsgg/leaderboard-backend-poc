using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NodaTime;

namespace LeaderboardBackend.Models.Entities;

public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
///     Represents a `Category` tied to a `Leaderboard`.
/// </summary>
public class Category
{
    /// <summary>
    ///     The unique identifier of the `Category`.<br/>
    ///     Generated on creation.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     The ID of the `Leaderboard` the `Category` is a part of.
    /// </summary>
    public long LeaderboardId { get; set; }

    /// <summary>
    ///     Relationship model for `LeaderboardId`.
    /// </summary>
    public Leaderboard? Leaderboard { get; set; }

    /// <summary>
    ///     The display name of the `Category`.
    /// </summary>
    /// <example>Foo Bar Baz%</example>
    public required string Name { get; set; }

    /// <summary>
    ///     The URL-scoped unique identifier of the `Category`.<br/>
    ///     Must be [2, 25] in length and consist only of alphanumeric characters and hyphens.
    /// </summary>
    /// <example>foo-bar-baz</example>
    [StringLength(80, MinimumLength = 2)]
    [RegularExpression(@"^([a-zA-Z0-9\-_]|%[A-F0-9]{2})*$")]
    public required string Slug { get; set; }

    /// <summary>
    ///     Information pertaining to the `Category`.
    /// </summary>
    /// <example>Video proof is required.</example>
    public string? Info { get; set; }

    /// <summary>
    ///     The direction used to rank runs belonging to this category.
    /// </summary>
    public SortDirection SortDirection { get; set; }

    /// <summary>
    ///     The time the Category was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    ///     The last time the Category was updated or <see langword="null" />.
    /// </summary>
    public Instant? UpdatedAt { get; set; }

    /// <summary>
    ///     The time at which the Category was deleted, or <see langword="null" /> if the Category has not been deleted.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Category category
            && Id == category.Id
            && Name == category.Name
            && Slug == category.Slug
            && Info == category.Info
            && SortDirection == category.SortDirection
            && LeaderboardId == category.LeaderboardId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Slug, LeaderboardId);
    }
}
