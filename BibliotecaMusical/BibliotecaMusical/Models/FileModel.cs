using System;

namespace BibliotecaMusical.Models {
	public class FileModel {
		public Guid Id { get; set; }
		//{
		//	get {
				//return Name
				//	.Replace(" ", string.Empty)
				//	.Replace("(", string.Empty)
				//	.Replace(")", string.Empty)
				//	.Replace("[", string.Empty)
				//	.Replace("]", string.Empty)
				//	.Replace("#", string.Empty)
				//	.Replace("-", string.Empty)
				//	.Replace("_", string.Empty)
				//	.Replace(".mp3", string.Empty);
		//	}
		//}
		public string Name { get; set; }
		public string Size { get; set; }
	}
}