using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BibliotecaMusical.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.Services {
	public class AzureService {
		private const string CONTAINER_NAME = "bibliotecamusical";
		private const string USER_TABLE_NAME = "users";

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

		public void SaveUserToTable(UserModel user) {
			user.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
			user.PartitionKey = user.Email.Substring(0, 2);
			user.RowKey = user.Email;

			var cloudTable = GetTable(USER_TABLE_NAME);
			TableOperation insertOperation = TableOperation.InsertOrReplace(user);
			cloudTable.Execute(insertOperation);
		}

		public bool CheckLoginUser(UserModel user) {
			var cloudTable = GetTable(USER_TABLE_NAME);
			var query = new TableQuery<UserModel>()
				.Where(TableQuery.CombineFilters(
					TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, user.Email),
					TableOperators.And,
					TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, user.Password)
				));

			var queryResults = cloudTable.ExecuteQuery(query);
			var userChecksOut = queryResults.Any();

			return userChecksOut;
		}
	}
}