using System;
using System.IO;
using BibliotecaMusical.WebJob.Models;
using BibliotecaMusical.WebJob.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BibliotecaMusical.WebJob {
	public class Functions {

		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public static void ProcessQueueMessage([QueueTrigger("bibliotecamusicaldeletelog")] string message, TextWriter log) {
			AzureService.DeleteBlob("bibliotecamusical", message);

			var userActionModel = new UserActionModel {
				Email = "WebJob",
				Description = $"File [{message}] was deleted from the Library."
			};
			UserActionService.SaveUserAction(userActionModel);
		}

		public static void ProcessBlob([BlobTrigger("bibliotecamusical/{name}")] CloudBlockBlob myBlob, string name, TraceWriter log) {
			var userActionModel = new UserActionModel {
				Email = "WebJob",
				Description = $"The file [{name}] was added to the Library. Size: {Convert.ToDecimal((double)myBlob.Properties.Length / 1024 / 1024).ToString("#,##0.00")} MB."
			};
			UserActionService.SaveUserAction(userActionModel);
		}
	}
}
