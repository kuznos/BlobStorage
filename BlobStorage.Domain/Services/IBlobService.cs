using Azure.Storage.Blobs.Models;
using BlobStorage.Domain.Enums;
using BlobStorage.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlobStorage.Domain.Services
{
	public interface IBlobService
	{
		public Task<Models.BlobInfo> GetBlobAsync(string name, string container);
		public Task<bool> BlobExistsAsync(string name, string container);
		public Task<BlobProperties> GetBlobAccessTierAsync(string name, string container);
		public Task<IEnumerable<FileProperties>> ListBlobsAsync(string container);
		public Task UploadBlobFilePathAsync(string filepath, string filename, string container);
		public Task<bool> UploadBase64BlobAsync(string content, string filename, string container, Tier Tier);
		public Task<bool> UploadMultipartFormDataContentBlobAsync(IEnumerable<MultiPartFormInfo> content, string container, AccessTier Tier);
		public Task ChangeTierAsync(Tier Tier, string filename, string container);
		public Task<string> RehydrateFileAsync(string filename, string container);
		public Task<bool> DeleteBlobAsync(string filename, string container);
		public Task<bool> CopyBlobAsync(string name, string container, string target_name, string target_container);
		public Task<bool> CreateThumbAsync(string name, string container);

		public Task<bool> ResetBlobIndexAndMetadataAsync(string filename, string container);

	}
}
