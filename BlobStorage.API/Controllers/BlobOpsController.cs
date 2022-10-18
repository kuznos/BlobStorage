using Azure.Storage.Blobs.Models;
using BlobStorage.API.Filters;
using BlobStorage.Domain.Enums;
using BlobStorage.Domain.Extensions;
using BlobStorage.Domain.Models;
using BlobStorage.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorage.API.Controllers
{
    /// <summary>
    /// Blob API
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [ApiKeyAuth]
    public class BlobOpsController : ControllerBase
    {
        private readonly IBlobService _blobService;
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="blobService"></param>
        public BlobOpsController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        /// <summary>
        /// Downloads blob file from Azure Blob Storage Container.
        /// </summary>
        /// <response code="200">Successfully downloded Bbloblob file from Azure Storage Container.</response>
        /// <response code="404">Unable to Download blob file from Azure Storage Container. File not found</response>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/{blobFileName}")]
        public async Task<IActionResult> GetBlobAsync(string blobFileName, string container)
        {
            try
            {
                if (string.IsNullOrEmpty(container)) { return BadRequest(); }
                var data = await _blobService.GetBlobAsync(blobFileName, container);

                return File(data.Content, data.ContentType);
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Copies blob file from Azure Blob Storage Container to target Container (Applies to the same Blob Storage account (container/targetcontainer)).
        /// </summary>
        /// <response code="200">Successfully copied blob file to target Container.</response>
        /// <response code="400">Unable to copy blob file to target Container. Bad Request</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/{blobFileName}/CopyBlobAsync/{targetcontainer}/{targetblobFileName}")]
        public async Task<IActionResult> CopyBlobAsync(string blobFileName, string container, string targetblobFileName, string targetcontainer)
        {
            try
            {
                if (string.IsNullOrEmpty(container)) { return BadRequest(); }
                var result = await _blobService.CopyBlobAsync(blobFileName, container,targetblobFileName, targetcontainer);

                if (result == true)
                {
                    return Ok();
                }
                else {
                    return BadRequest(); 
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Finds if blob file exists in Azure Blob Storage Container.
        /// </summary>
        /// <response code="200">Found blob file in Azure Storage Container.</response>
        /// <response code="400">Unable to find blob file in Azure Storage Container. Bad Request</response>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/ExistsAsync/{blobFileName}")]
        public async Task<IActionResult> BlobExistsAsync(string blobFileName, string container)
        {
            try
            {
                if (string.IsNullOrEmpty(container)) { return BadRequest(); }
                bool result = await _blobService.BlobExistsAsync(blobFileName, container);

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }

        /// <summary>
        /// Returns blob file access tier in Azure Blob Storage Container.
        /// </summary>
        /// <response code="200">Returns blob file access tier in Azure Storage Container.</response>
        /// <response code="400">Unable to return blob file access tier in Azure Storage Container. Bad Request</response>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/GetAccessTierAsync/{blobFileName}")]
        public async Task<IActionResult> GetBlobAccessTierAsync(string blobFileName, string container)
        {
            try
            {
                if (string.IsNullOrEmpty(container)) { return BadRequest(); }
                var result = await _blobService.GetBlobAccessTierAsync(blobFileName, container);
                return Ok(result.AccessTier);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }


        /// <summary>
        /// Gets a list of all the blob files in from Azure Blob Storage Container, including AccessTier.
        /// </summary>
        /// <response code="200">Successfully got the list of blob files.</response>
        /// <response code="400">Unable to get the lift of blob files.</response>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/ListBlobsAsync")]
        [ProducesResponseType(typeof(IEnumerable<FileProperties>), statusCode: 200)]
        public async Task<IActionResult> ListBlobsAsync(string container)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            return Ok(await _blobService.ListBlobsAsync(container));
        }


        /// <summary>
        /// Uploads blob file to Azure Blob Storage Container from File Path.
        /// </summary>
        /// <response code="200">Successfully uploaded blob file to Azure Storage Container.</response>
        /// <response code="400">Unable to upload blob file to Azure Storage Container.</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/UploadBlobFileFromPathAsync")]
        public async Task<IActionResult> UploadBlobFileFromPathAsync(string container, [FromBody] UploadFileRequest request)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            await _blobService.UploadBlobFilePathAsync(request.FilePath, request.FileName, container);
            return Ok();
        }


        /// <summary>
        /// Uploads blob file to Azure Blob Storage Container from Base64 string.
        /// </summary>
        /// <response code="200">Successfully uploaded blob file to Azure Storage Container.</response>
        /// <response code="400">Unable to upload blob file to Azure Storage Container.</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/{tier}/UploadBase64BlobAsync")]
        public async Task<IActionResult> UploadFileAsync(string container, string tier, [FromBody] UploadBase64Request request)
        {
            try
            {

                if (string.IsNullOrEmpty(container)) { return BadRequest(); }
                var result = await _blobService.UploadBase64BlobAsync(request.ContentBase64, request.Filename, container, tier.ToEnum<Tier>());

                if (result == true)
                    return Ok();
                else
                {
                    return BadRequest();
                }

            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }




        /// <summary>
        /// Changes the AccessTier of blob file stored in Azure Storage Container.
        /// </summary>
        /// <response code="200">Successfully changed blob file AccessTier.</response>
        /// <response code="400">Unable to change blob file AccessTier.</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/ChangeAccessTierAsync")]
        public async Task<IActionResult> ChangeAccessTierAsync(string container, [FromBody] ChangeTierRequest request)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            await _blobService.ChangeTierAsync(request.Tier.ToEnum<Tier>(), request.FileName, container);
            return Ok();
        }

        /// <summary>
        /// Begins rehydration proccess of archived blob file.
        /// </summary>
        /// <response code="200">Successfully started the rehydration of blob file.</response>
        /// <response code="400">Unable to start the rehydration of blob file.</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/RehydrateFileAsync/{blobFileName}")]
        public async Task<IActionResult> RehydrateFileAsync(string container, string blobFileName)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            return Ok(await _blobService.RehydrateFileAsync(blobFileName, container));
        }

        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/ResetBlobIndexAndMetadataAsync/{blobFileName}")]
        public async Task<IActionResult> ResetBlobIndexAndMetadataAsync(string container, string blobFileName)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            return Ok(await _blobService.ResetBlobIndexAndMetadataAsync(blobFileName, container));
        }


        /// <summary>
        /// Deletes blob file from Azure Storage Container.
        /// </summary>
        /// <response code="200">Successfully deleted blob file from Azure Storage Container.</response>
        /// <response code="400">Unable to delete blob file from Azure Storage Container.</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/DeleteFileAsync/{blobFileName}")]
        public async Task<IActionResult> DeleteFileAsync(string container, string blobFileName)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            bool result = await _blobService.DeleteBlobAsync(blobFileName, container);

            if (result == false)
            {
                return NotFound();
            }

            return Ok();
        }



        /// <summary>
        /// Uploads blob files to Azure Blob Storage Container from MultiPart Form request (requires keys and files).
        /// </summary>
        /// <response code="200">Successfully uploaded blob files to Azure Storage Container.</response>
        /// <response code="400">Unable to upload blob files to Azure Storage Container.</response>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/{tier}/UploadMultiPartFormDataAsync")]
        public async Task<IActionResult> UploadMultiPartFormDataAsync(string container, string tier)
        {

            try
            {

                if (string.IsNullOrEmpty(container)) { return BadRequest(); }
                if (string.IsNullOrEmpty(tier)) { return BadRequest(); }

                var req = HttpContext.Request;
                if (req.Form.Keys.Count == 0 || req.Form.Files.Count == 0)
                {
                    return BadRequest();
                }

                //Keys
                //string claimNumber = "";
                //var data = req.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                //foreach (var item in data)
                //{
                //	if (item.Key == "claim")
                //	{
                //		claimNumber = item.Value;
                //	}
                //}


                //Files
                List<MultiPartFormInfo> list = new List<MultiPartFormInfo>();
                foreach (var f in req.Form.Files)
                {
                    using (var ms = new MemoryStream())
                    {
                        f.CopyTo(ms);
                        MultiPartFormInfo mpfi = new MultiPartFormInfo { content = ms.ToArray(), Filename = f.FileName };
                        list.Add(mpfi);
                    }
                }
                IEnumerable<MultiPartFormInfo> ienmpfi = list;

                AccessTier accTier = null;
                switch (tier.ToEnum<Tier>())
                {
                    case Tier.Hot:
                        accTier = AccessTier.Hot;
                        break;
                    case Tier.Cool:
                        accTier = AccessTier.Cool;
                        break;
                    case Tier.Archive:
                        accTier = AccessTier.Hot;
                        break;
                    default:
                        throw new System.Exception();
                }
                await _blobService.UploadMultipartFormDataContentBlobAsync(ienmpfi, container, accTier);
                return Ok();

            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message.ToString());
            }



        }



        /// <summary>
        /// Creates thumb  from Azure Storage Container file.
        /// </summary>
        /// <response code="200">Successfully creates blob file thumb from Azure Storage Container.</response>
        /// <response code="400">Unable to create blob file from Azure Storage Container.</response>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]/{container}/CreateThumbAsync/{blobFileName}")]
        public async Task<IActionResult> CreateThumbAsync(string container, string blobFileName)
        {
            if (string.IsNullOrEmpty(container)) { return BadRequest(); }
            bool result = await _blobService.CreateThumbAsync(blobFileName, container);

            if (result == false)
            {
                return NotFound();
            }

            return Ok();
        }


    }
}
