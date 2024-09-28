using Profiler.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Profiler.TaskManager
{
	public abstract class ExternalStorage
	{
		public abstract String DisplayName { get; }
		public abstract String Icon { get; }
		public abstract bool IsPublic { get; }

		public abstract Uri UploadFile(String name, System.IO.Stream data, Action<double> onProgress, CancellationToken token);
	}

	class NetworkStorage : ExternalStorage
	{
		public String UploadURL { get; set; }
		public String DownloadURL { get; set; }

		public String GUID { get; set; } = Utils.GenerateShortGUID();
		public DateTime Date { get; set; } = DateTime.Now;

		public String IntermediateFolder => String.Format("{0}/{1}", Date.ToString("yyyy-MM-dd"), GUID);

		public override string DisplayName => String.Format("{0} ({1})", DownloadURL, UploadURL);
		public override string Icon => "appbar_folder";
		public override bool IsPublic => false;

		public NetworkStorage(String uploadURL, String downloadURL)
		{
			UploadURL = uploadURL;
			DownloadURL = downloadURL;
		}

		public override Uri UploadFile(string name, System.IO.Stream data, Action<double> onProgress, CancellationToken token)
		{
			String uploadFolder = System.IO.Path.Combine(UploadURL, IntermediateFolder);
			System.IO.Directory.CreateDirectory(uploadFolder);

			String uploadPath = System.IO.Path.Combine(uploadFolder, name);
			data.Position = 0;
			using (System.IO.FileStream outputStream = new System.IO.FileStream(uploadPath, System.IO.FileMode.Create))
				Utils.CopyStream(data, outputStream, (p) => { token.ThrowIfCancellationRequested(); onProgress(p); });

			String downloadPath = String.Format("{0}/{1}/{2}", DownloadURL, IntermediateFolder, name);
			return new Uri(downloadPath);
		}
	}
}
