using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureTraining.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureTraining.Services {
	public class AzureService {
		private const string CONTAINER_NAME = "filecontainer";

		private CloudBlobContainer GetBlobContainer() {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(CONTAINER_NAME);

			return cloudBlobContainer;
		}

		public void SaveFileToBlob(string fileName, Stream stream) {
			var blob = GetBlobContainer().GetBlockBlobReference(fileName);
			blob.UploadFromStream(stream);
		}

		public List<BlobModel> GetFileList() {
			var blobs = GetBlobContainer().ListBlobs(useFlatBlobListing: true)
				.OfType<CloudBlockBlob>()
				.Select(b => new BlobModel {
					Name = b.Name
				})
				.ToList();

			return blobs;
		}
	}
}