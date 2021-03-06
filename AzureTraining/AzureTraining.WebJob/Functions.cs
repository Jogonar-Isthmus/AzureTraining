﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureTraining.WebJob
{
	public class Functions
	{
		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public static void ProcessQueueMessage([QueueTrigger("messages")] string message, TextWriter log)
		{
			log.WriteLine(message);
		}

		public static void ProcessBlob([BlobTrigger("filecontainer/{name}")] CloudBlockBlob myBlob, string name, TraceWriter log)
		{
			log.Info($"C#Blob trigger function Processedblob\n Name:{name}\n Size: {myBlob.Properties.Length} Bytes");
		}
	}
}
