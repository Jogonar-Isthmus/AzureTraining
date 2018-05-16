using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureTraining.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTraining.Services {
	public class AzureService {
		private const string CONTAINER_NAME = "filecontainer";
		private const string TABLE_NAME = "tasks";

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
					Name = b.Name,
					Size = Convert.ToDecimal((double)b.Properties.Length / 1024 / 1024).ToString("#,##0.00")
				})
				.ToList();

			return blobs;
		}

		public void DeleteBlobByName(string fileName) {
			GetBlobContainer().GetBlockBlobReference(fileName).DeleteIfExists();
		}

		private CloudTable GetTable(string tableName) {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
			var cloudTable = cloudTableClient.GetTableReference(tableName);
			cloudTable.CreateIfNotExists();

			return cloudTable;
		}

		public void SaveTaskToTable(TaskModel task) {
			task.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

			var cloudTable = GetTable(TABLE_NAME);
			TableOperation insertOperation = TableOperation.Insert(task);
			cloudTable.Execute(insertOperation);
		}

		public List<TaskModel> GetTableList() {
			var cloudTable = GetTable(TABLE_NAME);
			//TableQuery<Task> query = new TableQuery<Task>():
			//cloudTable.Execute(selectOperation);
			
			return null;
		}
	}
}