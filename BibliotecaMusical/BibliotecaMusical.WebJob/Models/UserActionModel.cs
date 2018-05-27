using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.WebJob.Models {
	public class UserActionModel : TableEntity {
		public string Email { get; set; }
		public string Description { get; set; }
	}
}