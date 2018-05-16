using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTraining.Models {
	public class TaskModel : TableEntity {
		public string Description { get { return PartitionKey; } set { PartitionKey = value; } }
		public string CreatedDate { get { return RowKey; } set { RowKey = value; } }
	}
}