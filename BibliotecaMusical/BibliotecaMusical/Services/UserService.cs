using System;
using System.Linq;
using BibliotecaMusical.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.Services {
	public static class UserService {
		private const string USER_TABLE_NAME = "users";

		public static void SaveUser(UserModel user) {
			user.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
			user.PartitionKey = user.Email.Substring(0, 2);
			user.RowKey = user.Email;

			AzureService.SaveRecordToTable(USER_TABLE_NAME, user);
		}

		public static UserModel GetUser(string email) {
			var query = new TableQuery<UserModel>()
				.Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

			var user = AzureService.GetQueryResults(USER_TABLE_NAME, query).FirstOrDefault();

			return user;
		}

		public static bool CheckLoginUser(UserModel user) {
			var dbUser = GetUser(user.Email);
			
			return dbUser != null && dbUser.Password == user.Password;
		}
	}
}