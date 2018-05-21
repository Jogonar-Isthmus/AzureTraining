using System.IO;
using System.Web;
using System.Web.Mvc;
using BibliotecaMusical.Services;

namespace BibliotecaMusical.Controllers {
	[Authorize]
	public class LibraryController : Controller {
		// GET: Library/Files
		public ActionResult Files() {
			var files = LibraryService.GetFileList();
			return View(files);
		}

		// POST: Library/SaveFile
		[HttpPost]
		public ActionResult SaveFile(HttpPostedFileBase file) {
			var userEmail = System.Web.HttpContext.Current.User.Identity.Name;

			if (file != null && file.ContentLength > 0) {
				var fileName = Path.GetFileName(file.FileName);
				LibraryService.SaveFile(fileName, file.InputStream, userEmail);
			}

			return RedirectToAction("Files");
		}
		
		// GET: Library/Delete/<fileName>
		[HttpGet]
		public ActionResult Delete(string fileName) {
			var userEmail = System.Web.HttpContext.Current.User.Identity.Name;

			LibraryService.DeleteFile(fileName, userEmail);

			return RedirectToAction("Files");
		}

		// GET: Library/Stream/<fileName>
		public ActionResult Stream(string fileName) {
			byte[] file = LibraryService.GetFileData(fileName);

			return File(file, "audio/mpeg");
		}
	}
}
