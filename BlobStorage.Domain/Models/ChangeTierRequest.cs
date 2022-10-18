using System.ComponentModel.DataAnnotations;


namespace BlobStorage.Domain.Models
{
	public class ChangeTierRequest
	{
		[Required]
		public string Tier { get; set; }
		[Required]
		public string FileName { get; set; }
	}
}
