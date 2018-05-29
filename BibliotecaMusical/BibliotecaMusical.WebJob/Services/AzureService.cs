using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.WebJob.Services {
	public static class AzureService {
		#region Blob Container

		private static CloudBlobContainer GetBlobContainer(string containerName) {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
			cloudBlobContainer.CreateIfNotExists();

			return cloudBlobContainer;
		}

		public static IEnumerable<CloudBlockBlob> GetBlobList(string containerName) {
			var blobs = GetBlobContainer(containerName)
				.ListBlobs(useFlatBlobListing: true)
				.OfType<CloudBlockBlob>();

			return blobs;
		}

		public static CloudBlockBlob GetBlob(string containerName, string blobName) {
			var blob = GetBlobContainer(containerName).GetBlockBlobReference(blobName);
			blob.FetchAttributes();

			return blob;
		}

		public static void SaveBlob(string containerName, string blobName, Stream stream) {
			var blob = GetBlobContainer(containerName).GetBlockBlobReference(blobName);
			blob.UploadFromStream(stream);
		}

		public static void DeleteBlob(string containerName, string blobName) {
			GetBlobContainer(containerName).GetBlockBlobReference(blobName).DeleteIfExists();
		}

		#endregion Blob Container

		#region Tables

		private static CloudTable GetTable(string tableName) {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
			var cloudTable = cloudTableClient.GetTableReference(tableName);
			cloudTable.CreateIfNotExists();

			return cloudTable;
		}

		public static void SaveRecordToTable(string tableName, ITableEntity model) {
			var cloudTable = GetTable(tableName);
			TableOperation insertOperation = TableOperation.InsertOrReplace(model);
			cloudTable.Execute(insertOperation);
		}

		public static IEnumerable<T> GetQueryResults<T>(string tableName, TableQuery<T> query) where T : TableEntity, new() {
			var cloudTable = GetTable(tableName);
			var queryResults = cloudTable.ExecuteQuery(query);

			return queryResults;
		}

		#endregion Tables

		#region Queues

		private static CloudQueue GetQueue(string queueName) {
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

			var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
			var cloudQueue = cloudQueueClient.GetQueueReference(queueName);
			cloudQueue.CreateIfNotExists();

			return cloudQueue;
		}

		public static void SaveMessage(string queueName, string message) {
			var cloudQueue = GetQueue(queueName);

			var cloudQueueMessage = new CloudQueueMessage(message);
			cloudQueue.AddMessage(cloudQueueMessage);
		}

		public static List<string> GetMessageList(string queueName) {
			var messages = new List<string>();

			var cloudQueue = GetQueue(queueName);
			cloudQueue.FetchAttributes();
			var amount = cloudQueue.ApproximateMessageCount;

			if (amount.HasValue && amount.Value > 0) {
				amount = amount <= 32 ? amount : 32;
				messages = cloudQueue
					.GetMessages(amount.Value, TimeSpan.FromSeconds(1))
					.OrderByDescending(m => m.InsertionTime)
					.Select(m => m.AsString)
					.ToList();
			}

			return messages;
		}

		#endregion Queues
	}
}