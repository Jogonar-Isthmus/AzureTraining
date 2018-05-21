using System;
using System.Collections.Generic;
using System.Linq;
using BibliotecaMusical.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace BibliotecaMusical.Services {
	public static class UserActionService {
		private const string USER_ACTION_TABLE_NAME = "useractionlog";

		public static void SaveUserAction(UserActionModel userActionModel) {
			userActionModel.PartitionKey = userActionModel.Email.Substring(0, 2);
			userActionModel.RowKey = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");;

			AzureService.SaveRecordToTable(USER_ACTION_TABLE_NAME, userActionModel);
		}

		public static List<UserActionModel> GetUserActionList() {
			var query = new TableQuery<UserActionModel>();
			var queryResults = AzureService.GetQueryResults(USER_ACTION_TABLE_NAME, query)
				.OrderByDescending(ual => ual.Timestamp)
				.ToList();

			return queryResults;
		}
	}
}