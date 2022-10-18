using System;
using System.IO;

namespace BlobStorage.Domain.Models
{
	public class BlobInfo
	{
		public Stream Content { get; set; }
		public String ContentType { get; set; }
		public long ContentLength { get; set; }
		public BlobInfo(Stream _Content, String _ContentType, long _ContentLength)
		{
			Content = _Content;
			ContentType = _ContentType;
			ContentLength = _ContentLength;
		}

	}
}
