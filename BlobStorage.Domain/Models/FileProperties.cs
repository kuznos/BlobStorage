using System.ComponentModel.DataAnnotations;

namespace BlobStorage.Domain.Models
{
	public class FileProperties
	{
		[Required]
		public string Tier { get; set; }
		[Required]
		public string FileName { get; set; }
	}
}
