using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorage.Domain.Enums;
using BlobStorage.Domain.Extensions;
using BlobStorage.Domain.Helpers;
using BlobStorage.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorage.Domain.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Models.BlobInfo> GetBlobAsync(string name, string container)
        {
            try
            {         
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(name);
                //var xxx = containerClient.FindBlobsByTagsAsync


                var blobDownloadInfo = await blobClient.DownloadAsync();
                return new Models.BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType, blobDownloadInfo.Value.ContentLength);           
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                return null;
            }

        }

        public async Task<bool> BlobExistsAsync(string name, string container)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                 var blobClient = containerClient.GetBlobClient(name);
                 var pro = blobClient.GetPropertiesAsync().Result.Value.AccessTier;
                 return await blobClient.ExistsAsync();
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                return false;
            }
        }
        public async Task<BlobProperties> GetBlobAccessTierAsync(string name, string container)
        {
         
            try 
            {            
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(name);
                //.Result.Value.AccessTier
                return await blobClient.GetPropertiesAsync();
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                return null;
            }
        }
        public async Task<IEnumerable<FileProperties>> ListBlobsAsync(string container)
        {
            var items = new List<FileProperties>();
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    FileProperties ctr = new FileProperties { FileName = blobItem.Name, Tier = blobItem.Properties.AccessTier.ToString() };
                    items.Add(ctr);
                }
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }
            
            return items;
        }

        public async Task UploadBlobFilePathAsync(string filepath, string filename, string container)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(filename);
            await blobClient.UploadAsync(filepath, new BlobHttpHeaders { ContentType = filepath.GetContentType() });
        }

        public async Task<bool> UploadBase64BlobAsync(string content, string filename, string container, Tier tier)
        {
            bool result = false;

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(filename);
                var bytes = Convert.FromBase64String(content);

                using var memoryStream = new MemoryStream(bytes);
                {

                    AccessTier _tier = AccessTier.Hot;
                    switch (tier)
                    {
                        case Enums.Tier.Hot:
                            _tier = AccessTier.Hot;
                            break;
                        case Enums.Tier.Cool:
                            _tier = AccessTier.Cool;
                            break;
                        case Enums.Tier.Archive:
                            _tier = AccessTier.Archive;
                            break;
                    }

                    var blobHttpHeaders = new BlobHttpHeaders() { ContentType = filename.GetContentType() };
                    var uploadOptions = new BlobUploadOptions() { AccessTier = _tier, HttpHeaders = blobHttpHeaders };
                    var response = await blobClient.UploadAsync(memoryStream, uploadOptions);

                    //if (response.Value.LastModified.Date.ToShortDateString() == DateTime.Now.ToShortDateString())
                    //{
                    //    await blobClient.SetTagsAsync(Global_Helper.CreateTags(container, filename));
                    //    await blobClient.SetMetadataAsync(Global_Helper.CreateMetadata(container, filename));
                    //    result = true; // await UploadThumbnailAsync(memoryStream, filename, container, _tier);
                    //}
                    //Console.WriteLine(response.Value.VersionId);

                    await UploadThumbnailAsync(memoryStream, filename, container, _tier);
                    //await blobClient.SetTagsAsync(Global_Helper.CreateTags(container, filename));
                    //await blobClient.SetMetadataAsync(Global_Helper.CreateMetadata(container, filename));
                    result = true;
                    //await blobClient.SetTagsAsync()
                }

            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                result = false;
            }

            return result;

        }

        private async Task<bool> UploadThumbnailAsync(MemoryStream memoryStream, string filename, string container, AccessTier tier)
        {

            bool result = false;
            try
            {

                //switch (container.ToLower())
                //{
                //	case "archiveinli":
                //		break;
                //	case "archivegnomon":
                //		break;
                //	case "claims":
                //		break;
                //	case "sales":
                //		break;
                //	default:
                //		result = true;
                //		return result;
                //}

                ContentType contype = ContentType.image_jpeg;
                if (filename.GetContentType().Contains("image") == false)
                {
                    result = true;
                    return result;
                }
                else
                {
                    switch (filename.GetContentType())
                    {
                        case "image/jpeg":
                            contype = ContentType.image_jpeg;
                            break;
                        case "image/png":
                            contype = ContentType.image_png;
                            break;
                        case "image/tiff":
                            contype = ContentType.image_tiff;
                            break;
                        case "image/gif":
                            contype = ContentType.image_gif;
                            break;
                        case "image/bmp":
                            contype = ContentType.image_bmp;
                            break;
                    }
                }

                if (filename.Contains("_thumb"))
                {
                    result = true;
                    return result;
                }


                string thumb_filename = Path.GetFileNameWithoutExtension(filename) + "_thumb" + Path.GetExtension(filename);
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(thumb_filename);
                var blobHttpHeaders = new BlobHttpHeaders() { ContentType = thumb_filename.GetContentType() };
                var uploadOptions = new BlobUploadOptions() { AccessTier = tier, HttpHeaders = blobHttpHeaders };
                var response = await blobClient.UploadAsync(memoryStream.ResizeImage(100, contype), uploadOptions);

                if (response.Value.LastModified.Date.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                result = false;
            }

            return result;

        }

        public async Task<bool> DeleteBlobAsync(string filename, string container)
        {
            bool result = false;

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(filename);
                result = await blobClient.DeleteIfExistsAsync();

                if (result == true)
                {
                    await DeleteThumbnailAsync(filename, container);
                }

            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                result = false;
            }

            return result;
        }

        private async Task<bool> DeleteThumbnailAsync(string filename, string container)
        {
            bool result = false;

            try
            {

                if (filename.GetContentType().Contains("image") == false)
                {
                    result = true;
                    return result;
                }

                string thumb_filename = Path.GetFileNameWithoutExtension(filename) + "_thumb" + Path.GetExtension(filename);
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(thumb_filename);
                result = await blobClient.DeleteIfExistsAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = false;
            }

            return result;

        }

        public async Task ChangeTierAsync(Tier Tier, string filename, string container)
        {
           
            try
            { 
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(filename);
            

                switch (Tier)
                {
                    case Enums.Tier.Hot:
                        await blobClient.SetAccessTierAsync(AccessTier.Hot);
                        break;
                    case Enums.Tier.Cool:
                        await blobClient.SetAccessTierAsync(AccessTier.Cool);
                        break;
                    case Enums.Tier.Archive:
                        await blobClient.SetAccessTierAsync(AccessTier.Archive);
                        break;
                }

             }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }

}


        public async Task<bool> ResetBlobIndexAndMetadataAsync(string filename, string container)
        {
            bool result = false;
            try
            {
                var newMetadata = new Dictionary<string, string>();
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(filename);
                await blobClient.SetMetadataAsync(newMetadata);
                await blobClient.SetTagsAsync(newMetadata);
                result = true;
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }
            return result;
        }


        public async Task<string> RehydrateFileAsync(string filename, string container)
        {
            string result = "";
            
            try
            { 
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(filename);
            var props = await blobClient.GetPropertiesAsync();
            if (props.Value.AccessTier == "Archive")
            {
                if (props.Value.ArchiveStatus == null)
                {
                    await blobClient.SetAccessTierAsync(AccessTier.Hot);
                    result = $"Begin rehydration at {DateTime.Now}";
                }
                else
                {
                    result = $"Still rehydrating... {props.Value.ArchiveStatus}, tier changed on {props.Value.AccessTierChangedOn}";
                }
            }
            else
            {
                result = $"Rehydrated blob is now in {props.Value.AccessTier} Tier";
            }

            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }

            return result;
        }

        public async Task<bool> UploadMultipartFormDataContentBlobAsync(IEnumerable<MultiPartFormInfo> content, string container, AccessTier tier)
        {
            bool result = true;
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                foreach (var item in content)
                {
                    var blobClient = containerClient.GetBlobClient(item.Filename);
                    using var memoryStream = new MemoryStream(item.content);

                    var blobHttpHeaders = new BlobHttpHeaders() { ContentType = item.Filename.GetContentType() };
                    var uploadOptions = new BlobUploadOptions() { AccessTier = tier, HttpHeaders = blobHttpHeaders };

                    await blobClient.UploadAsync(memoryStream, uploadOptions);
                }

            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                result = false;
            }
            return result;
        }


        public async Task<bool> CopyBlobAsync(string name, string container, string target_name, string target_container)
        {
            bool result = false;

            try
            {

                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobUri = containerClient.GetBlobClient(name).Uri;
                var targetContainerClient = _blobServiceClient.GetBlobContainerClient(target_container);
                var targetBlobClient = targetContainerClient.GetBlobClient(target_name);
                await targetBlobClient.StartCopyFromUriAsync(blobUri);
                var properties = await targetBlobClient.GetPropertiesAsync();
        
                if (properties.Value.CopyStatus == Azure.Storage.Blobs.Models.CopyStatus.Success)
                {
                    var blobClient = containerClient.GetBlobClient(name);
                    var blobDownloadInfo = await blobClient.DownloadAsync();
                    var memoryStream = new MemoryStream();
                    blobDownloadInfo.Value.Content.CopyTo(memoryStream);
                    await UploadThumbnailAsync(memoryStream, target_name, target_container, AccessTier.Hot);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                result = false;
            }

            return result;

        }

        public async Task<bool> CreateThumbAsync(string name, string container)
        {

            bool result = false;

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(name);
                var blobDownloadInfo = await blobClient.DownloadAsync();
                //return new Models.BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType, blobDownloadInfo.Value.ContentLength);

                MemoryStream _ms = new MemoryStream();
                _ms.Position = 0;
                Stream sourceStream = blobDownloadInfo.Value.Content;
                sourceStream.CopyTo(_ms);


                result = await UploadThumbnailAsync(_ms, name, container, AccessTier.Hot);

            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync(ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
                result = false;
            }

            return result;
        }

    }
}
