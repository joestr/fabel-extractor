using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Threading;
using System.Collections.Generic;

namespace fabel_extractor
{
	class Program
	{
		static void Main(string[] args)
		{
			NameValueCollection config;
			config = ConfigurationManager.AppSettings;

			if (bool.Parse(config.Get("verbose_local_output_only"))) {
				log("VERBOSE", "LOCAL OUTPUT ONLY");

				PcTerminalData localdataManager = new PcTerminalData(
					config.Get("pcterminal_connectionstring"),
					config.Get("pcterminal_query")
				);

				List<IList<object>> localData = localdataManager.GetDataAsTableForSpreadsheet();

				log("INFO", "---+++---+++---");
				foreach (List<object> localrow  in localData) {
					foreach (object column in localrow) {
						Console.Write("'" + column + "';");
					}
					Console.Write("\n");
				}
				log("INFO", "---+++---+++---");
				Console.ReadKey();
				System.Environment.Exit(0);
			}

			log("INFO", "Creating SpreadsheetManager ...");

			SpreadsheetManager spreadsheetManager = new SpreadsheetManager(
				config.Get("spreadsheet_secretsfile"),
				config.Get("spreadsheet_appname"),
				config.Get("spreadsheet_id"),
				config.Get("spreadsheet_sheetname_today")
			);

			log("INFO", "Created SpreadsheetManager.");

			log("INFO", "Creating PcTerminalData manager ...");

			PcTerminalData dataManager = new PcTerminalData(
				config.Get("pcterminal_connectionstring"),
				config.Get("pcterminal_query")
			);

			log("INFO", "Created PcTerminalData.");

			while(true)
			{
				log("INFO", "Clearing all data from spreadsheet ...");

				spreadsheetManager.ClearAll();

				log("INFO", "Cleared all data from spreadsheet.");

				log("INFO", "Getting info and uploading ...");

				List<IList<object>> data = dataManager.GetDataAsTableForSpreadsheet();

				if (data != null) {
					if (DateTime.Now.Date == ((DateTime)data[0][2]).AddDays(1).Date)
					{
						// yesterday
						spreadsheetManager.SpreadsheetSheetName = config.Get("spreadsheet_sheetname_yesterday");
						spreadsheetManager.FabelInsert(data);
					}
					if (DateTime.Now.Date == ((DateTime)data[0][2]).Date) {
						// Today
						spreadsheetManager.SpreadsheetSheetName = config.Get("spreadsheet_sheetname_today");
						spreadsheetManager.FabelInsert(data);
					}
					if (DateTime.Now.AddDays(1).Date == ((DateTime)data[0][2]).Date)
					{
						spreadsheetManager.SpreadsheetSheetName = config.Get("spreadsheet_sheetname_tomorrow");
						spreadsheetManager.FabelInsert(data);
					}
					if (DateTime.Now.AddDays(2).Date == ((DateTime)data[0][2]).Date)
					{
						spreadsheetManager.SpreadsheetSheetName = config.Get("spreadsheet_sheetname_tomorrow2");
						spreadsheetManager.FabelInsert(data);
					}
				}

				log("INFO", $"Finished! Again in {config.Get("app_sleep_seconds")} seconds");

				Thread.Sleep(TimeSpan.FromSeconds(Int32.Parse(config.Get("app_sleep_seconds"))));
			}
		}

		static void log(string level, string message)
		{
			Console.WriteLine($"[{string.Concat(DateTime.UtcNow.ToString("s"), "Z")}] [{level}]: {message}");
		}
	}
}
