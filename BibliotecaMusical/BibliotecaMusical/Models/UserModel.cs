using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.Models {
	public class UserModel : TableEntity {
		public string Description { get { return PartitionKey; } set { PartitionKey = value; } }
		public string CreatedDate { get { return RowKey; } set { RowKey = value; } }
	}
}