using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;
using fabel_extractor_blogger;
using Google.Apis.Blogger.v3.Data;

namespace fabel_extractor
{
	class Program
	{

		private static readonly HttpClient client = new HttpClient();
		static async System.Threading.Tasks.Task Main(string[] args)
		{
			NameValueCollection config;
			config = ConfigurationManager.AppSettings;

			if (bool.Parse(config.Get("verbose_localdata"))) {
				log("VERBOSE", "LOCAL DATA");

				PcTerminalData localdataManager = new PcTerminalData(
					config.Get("pcterminal_connectionstring"),
					config.Get("pcterminal_query")
				);

				List<FabelEntry> localData = localdataManager.LoadAndSave();

				log("INFO", "---+++---+++---");
				foreach (FabelEntry localrow  in localData) {
						Console.WriteLine(localrow.ToString());
				}
				log("INFO", "---+++---+++---");

				Console.ReadKey();
				System.Environment.Exit(0);
			}

			if (bool.Parse(config.Get("verbose_checkblogger"))) {
				await BloggerManager.Instance.AuthAndIntitAsync(config.Get("blogger_appname"), config.Get("blogger_appcredentialspath"));

				log("VERBOSE", "CHECK BLOGGER");

				log("INFO", "---+++---+++---");
				foreach (Post p in BloggerManager.Instance.GetPosts(config.Get("blogger_blogid"))) {
					Console.WriteLine($"Post ID: {p.Id}; Post title: {p.Title}");
				}
				log("INFO", "---+++---+++---");

				Console.ReadKey();
				System.Environment.Exit(0);
			}

			log("INFO", "Creating PcTerminalData manager ...");

			PcTerminalData dataManager = new PcTerminalData(
				config.Get("pcterminal_connectionstring"),
				config.Get("pcterminal_query")
			);

			log("INFO", "Created PcTerminalData.");

			await BloggerManager.Instance.AuthAndIntitAsync(config.Get("blogger_appname"), config.Get("blogger_appcredentialspath"));

			while(true)
			{

				log("INFO", "Getting info and uploading ...");

				List<FabelEntry> data = dataManager.LoadAndSave();

				var base64Data = SerializeBase64(data);

				BloggerManager.Instance.SetPostConent(
					config.Get("blogger_blogid"),
					config.Get("blogger_postid"),
					base64Data
				);

				log("INFO", $"Finished! Again in {config.Get("app_sleep_seconds")} seconds");

				Thread.Sleep(TimeSpan.FromSeconds(Int32.Parse(config.Get("app_sleep_seconds"))));
			}
		}

		static void log(string level, string message)
		{
			Console.WriteLine($"[{string.Concat(DateTime.UtcNow.ToString("s"), "Z")}] [{level}]: {message}");
		}

		public static string SerializeBase64(object o)
		{
			// Serialize to a base 64 string
			byte[] bytes;
			long length = 0;
			System.IO.MemoryStream ws = new System.IO.MemoryStream();
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter sf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			sf.Serialize(ws, o);
			length = ws.Length;
			bytes = ws.GetBuffer();
			string encodedData = bytes.Length + ":" + Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
			return encodedData;
		}
	}
}
