using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.WebJob.Models {
	public class UserModel : TableEntity {
		public string Email { get; set; }
		public string Password { get; set; }
		public string CreatedDate { get; set; }
		public string Status { get; set; }
	}
}