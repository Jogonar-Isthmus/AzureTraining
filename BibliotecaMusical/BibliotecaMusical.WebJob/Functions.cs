using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BibliotecaMusical.WebJob {
	public class Functions {
		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public static void ProcessQueueMessage([QueueTrigger("messages")] string message, TextWriter log) {
			log.WriteLine(message);
		}

		public static void ProcessBlob([BlobTrigger("filecontainer/{name}")] CloudBlockBlob myBlob, string name, TraceWriter log) {
			log.Info($"C#Blob trigger function Processedblob\n Name:{name}\n Size: {myBlob.Properties.Length} Bytes");
		}
	}
}
