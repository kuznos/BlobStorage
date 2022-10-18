using NUnit.Framework;
using BlobStorage.Domain.Services;
using BlobStorage.Domain.Models;
using BlobStorage.Domain.Helpers;
using BlobStorage.Domain.Enums;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BlobStorage.Tests
{
	public class BlobService_Should
	{
		private AppEnvironment appEnv;
		private MemoryStream memStream;
		private string container;
		private string filename;
		private const string FPath = @"C:\temp\blobs\";
		private IBlobService _blobService;

		#region NUnit3

		[SetUp]
		public void Setup()
		{
			appEnv = AppEnvironment.Dev;
			memStream = new MemoryStream();
			_blobService = new BlobService(new Azure.Storage.Blobs.BlobServiceClient(Global_Helper.GetAzureBlobStorageConnStrFromAzureKeyVault(appEnv)));
			container = "";
			filename = "";
		}

		#endregion

		[Category("BlobStorage.GetBlobAsync")]
		[Test(Description = "Test for downloading blob file from Azure Blob Storage container")]
		[Order(1)]
		public async Task GetBlobAsync()
		{

			// Arrange
			appEnv = AppEnvironment.Dev;
			//IBlobService _blobService = new BlobService(new Azure.Storage.Blobs.BlobServiceClient(Global_Helper.GetAzureBlobStorageConnStr(appEnv)));
			container = "contracts";
			filename = "10.139537.pdf";

			// Act
			var data = await _blobService.GetBlobAsync(filename, container);
			data.Content.CopyTo(memStream);
			byte[] bArray = memStream.ToArray();

			// Assert
			try
			{

				if (await CreateFileAsync(FPath + filename, bArray) == false) //System.IO.File.WriteAllBytes("C:\\temp\\" + filename, memStream.ToArray());
				{
					Assert.Fail("Failed creating file : " + filename);
				}
				Assert.IsNotNull(data.Content);
				Assert.That(data.ContentLength, Is.GreaterThan(0));

			}
			catch (System.Exception ex)
			{
				Assert.Fail(ex.Message);
			}

		}

		[Category("BlobStorage.GetBlobAsync")]
		[Test(Description = "Test for downloading blob file from Azure Blob Storage container")]
		[Order(1)]
		public async Task UploadBase64BlobAsync()
		{

			// Arrange
			//appEnv = AppEnvironment.Dev;
			//IBlobService _blobService = new BlobService(new Azure.Storage.Blobs.BlobServiceClient(Global_Helper.GetAzureBlobStorageConnStr(appEnv)));
			container = "contracts";
			filename = "binder_big.pdf";

			// Act
			var data = await _blobService.GetBlobAsync(filename, container);
			data.Content.CopyTo(memStream);
			byte[] bArray = memStream.ToArray();

			// Assert
			try
			{
				if (await CreateFileAsync(FPath + filename, bArray) == false) //System.IO.File.WriteAllBytes("C:\\temp\\" + filename, memStream.ToArray());
				{
					Assert.Fail("Failed creating file : " + filename);
				}
				Assert.IsNotNull(data.Content);
				Assert.That(data.ContentLength, Is.GreaterThan(0));

			}
			catch (System.Exception ex)
			{
				Assert.Fail(ex.Message);
			}

		}


		[Category("BlobStorage.DeleteBlobAsync")]
		[Test(Description = "Delete blob file from Azure Blob Storage container")]
		[Order(1)]
		public async Task DeleteBlobAsync()
		{

			// Arrange
			//appEnv = AppEnvironment.Dev;
			//IBlobService _blobService = new BlobService(new Azure.Storage.Blobs.BlobServiceClient(Global_Helper.GetAzureBlobStorageConnStr(appEnv)));
			container = "contracts";
			filename = "binder_big.pdf";

			// Act
			bool result = await _blobService.DeleteBlobAsync(filename, container);

			// Assert
			try
			{


			}
			catch (System.Exception ex)
			{
				Assert.Fail(ex.Message);
			}

		}




		#region Helpers
		private async Task<bool> CreateFileAsync(string FilePath, byte[] bArray)
		{
			bool result = false;
			try
			{

				using (FileStream SourceStream = File.Open(FilePath, FileMode.OpenOrCreate))
				{
					SourceStream.Seek(0, SeekOrigin.End);
					await SourceStream.WriteAsync(bArray, 0, bArray.Length);
					result = true;
				}
			}

			catch (System.Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return result;
		}
		#endregion

	}
}