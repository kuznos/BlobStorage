using System.ComponentModel.DataAnnotations;

namespace BlobStorage.Domain.Models
{
	public class MultiPartFormInfo
	{
		[Required]
		public byte[] content { get; set; }
		[Required]
		public string Filename { get; set; }
	}
}
