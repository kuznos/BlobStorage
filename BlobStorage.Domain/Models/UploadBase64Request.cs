using System.ComponentModel.DataAnnotations;

namespace BlobStorage.Domain.Models
{
	public class UploadBase64Request
	{
		[Required]
		public string ContentBase64 { get; set; }
		[Required]
		public string Filename { get; set; }
	}
}
