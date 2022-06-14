using System.ComponentModel.DataAnnotations;

namespace LeaderboardBackend.Models.Entities;

public enum MetricType {
	INT,
	FLOAT,
	INTERVAL,
}

public class Metric
{
	public long Id { get; set; }

	[Required]
	public MetricType Type { get; set; }

	[Required]
	public string Name { get; set; } = null!;

	[Required]
	public int Min { get; set; }

	[Required]
	public int Max { get; set; }
}
