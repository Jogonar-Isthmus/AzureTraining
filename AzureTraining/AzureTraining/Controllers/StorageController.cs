using System.IO;
using System.Web;
using System.Web.Mvc;
using AzureTraining.Models;
using AzureTraining.Services;

namespace AzureTraining.Controllers {
	public class StorageController : Controller {
		private AzureService _azureService;

		public StorageController() {
			_azureService = new AzureService();
		}

		// GET: Storage
		public ActionResult Blobs() {
			var blobs = _azureService.GetFileList();
			return View(blobs);
		}

		// POST: Storage/SaveFile
		[HttpPost]
		public ActionResult SaveFile(HttpPostedFileBase file) {
			if (file != null && file.ContentLength > 0) {
				var fileName = Path.GetFileName(file.FileName);
				_azureService.SaveFileToBlob(fileName, file.InputStream);
			}

			return RedirectToAction("Blobs");
		}
		
		// GET: Storage/Delete/<fileName>
		[HttpGet]
		public ActionResult Delete(string fileName) {
			_azureService.DeleteBlobByName(fileName);

			return RedirectToAction("Blobs");
		}

		// GET: Storage/Tables
		[HttpGet]
		public ActionResult Tables() {
			var tasks = _azureService.GetTaskList();
			return View(tasks);
		}

		// POST: Storage/Tables
		[HttpPost]
		public ActionResult Tables(TaskModel task) {
			_azureService.SaveTaskToTable(task);

			return RedirectToAction("Tables");
		}
	}
}
