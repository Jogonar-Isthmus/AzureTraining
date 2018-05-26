using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureTraining.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTraining.Services {
	public class AzureService {
		private const string CONTAINER_NAME = "filecontainer";
		private const string TABLE_NAME = "tasks";
		private const string QUEUE_NAME = "messages";

		private CloudBlobContainer GetBlobContainer() {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(CONTAINER_NAME);
			cloudBlobContainer.CreateIfNotExists();

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

		public List<TaskModel> GetTaskList() {
			var tasks = new List<TaskModel>();

			var cloudTable = GetTable(TABLE_NAME);
			TableQuery<TaskModel> query = new TableQuery<TaskModel>();
			tasks = cloudTable.ExecuteQuery(query).ToList();

			//var query = new TableQuery<UserModel>()
			//	.Where(TableQuery.CombineFilters(
			//		TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, user.Email),
			//		TableOperators.And,
			//		TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, user.Password)
			//	));

			//var queryResults = AzureService.GetQueryResults(USER_TABLE_NAME, query);
			//var userChecksOut = queryResults.Any();

			//return userChecksOut;

			return tasks;
		}


		private CloudQueue GetQueue(string queueName) {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
			var cloudQueue = cloudQueueClient.GetQueueReference(queueName);
			cloudQueue.CreateIfNotExists();

			return cloudQueue;
		}

		public void SaveMessageToQueue(string message) {
			var cloudQueue = GetQueue(QUEUE_NAME);

			var cloudQueueMessage = new CloudQueueMessage(message);
			cloudQueue.AddMessage(cloudQueueMessage);
		}

		public List<string> GetMessageList() {
			var messages = new List<string>();

			var cloudQueue = GetQueue(QUEUE_NAME);
			cloudQueue.FetchAttributes();
			var amount = cloudQueue.ApproximateMessageCount;
			
			if(amount.HasValue && amount.Value> 0) {
				amount = amount <= 32 ? amount : 32;
				messages = cloudQueue
					.GetMessages(amount.Value, TimeSpan.FromSeconds(1))
					.OrderByDescending(m => m.InsertionTime)
					.Select(m => m.AsString)
					.ToList();
			}
			
			return messages;
		}
	}
}