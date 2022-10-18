using System.ComponentModel.DataAnnotations;

namespace BlobStorage.Domain.Models
{
	public class UploadFileRequest
	{
		[Required]
		public string FilePath { get; set; }
		[Required]
		public string FileName { get; set; }
	}
}
