using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BibliotecaMusical.Models;

namespace BibliotecaMusical.Services {
	public class LibraryService {
		private const string CONTAINER_NAME = "bibliotecamusical";
		private const string DELETE_QUEUE_NAME = "bibliotecamusicaldeletelog";

		public static void SaveFile(string fileName, Stream stream, string userEmail) {
			AzureService.SaveBlob(CONTAINER_NAME, fileName, stream);

			var userActionModel = new UserActionModel {
				Email = userEmail,
				Description = $"User [{userEmail}] added the file [{fileName}] to the Library."
			};
			UserActionService.SaveUserAction(userActionModel);
		}

		public static List<FileModel> GetFileList() {
			var fileList = AzureService.GetBlobList(CONTAINER_NAME)
				.Select(b => new FileModel {
					Id = Guid.NewGuid(),
					Name = b.Name,
					Size = Convert.ToDecimal((double)b.Properties.Length / 1024 / 1024).ToString("#,##0.00")
				})
				.ToList();

			return fileList;
		}

		public static byte[] GetFileData(string fileName) {
			var file = AzureService.GetBlob(CONTAINER_NAME, fileName);

			byte[] fileData = new byte[file.Properties.Length];
			file.DownloadToByteArray(fileData, 0);

			return fileData;
		}

		public static void DeleteFile(string fileName, string userEmail) {
			//AzureService.DeleteBlob(CONTAINER_NAME, fileName);
			AzureService.SaveMessage(DELETE_QUEUE_NAME, fileName);

			var userActionModel = new UserActionModel {
				Email = userEmail,
				Description = $"User [{userEmail}] deleted the file [{fileName}] from the Library."
			};
			UserActionService.SaveUserAction(userActionModel);
		}
	}
}