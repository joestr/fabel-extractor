using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.IO;

namespace fabel_extractor_backend
{
	class SpreadsheetManager
	{
		private string[] scopes = new string[]
		{
			SheetsService.Scope.Spreadsheets
		};
		private GoogleCredential googleCredential;
		private SheetsService spreadsheetService;
		private string appCredentialPath, appName, spreadsheetId, spreadsheetSheetName;

		private static SpreadsheetManager instance = null;
		private static readonly object padlock = new object();

		private List<IList<object>> currentData;

		public string AppCredentialPath { get => appCredentialPath; set => appCredentialPath = value; }
		public string AppName { get => appName; set => appName = value; }
		public string SpreadsheetId { get => spreadsheetId; set => spreadsheetId = value; }
		public string SpreadsheetSheetName { get => spreadsheetSheetName; set => spreadsheetSheetName = value; }

		public static SpreadsheetManager Instance
		{
			get {
				lock (padlock) {
					if (instance == null)
					{
						instance = new SpreadsheetManager();
					}
					return instance;
				}
			}
		}
		public SpreadsheetManager()
		{
		}

		public void AuthAndInit()
		{
			using (var stream = new FileStream(this.appCredentialPath, FileMode.Open, FileAccess.Read))
			{
				this.googleCredential = GoogleCredential.FromStream(stream)
				.CreateScoped(scopes);
			}

			this.spreadsheetService = new SheetsService(
				new BaseClientService.Initializer()
				{
					HttpClientInitializer = this.googleCredential,
					ApplicationName = this.appName
				}
			);
		}

		public void ClearAll()
		{
			var range = $"{this.spreadsheetSheetName}!A:Z";
			var requestBody = new ClearValuesRequest();

			var deleteRequest = this.spreadsheetService.Spreadsheets.Values.Clear(requestBody, this.spreadsheetId, range);
			var deleteReponse = deleteRequest.Execute();
		}

		public void CreateRow(List<object> entry)
		{
			var range = $"{this.spreadsheetSheetName}!A:Z";
			var valueRange = new ValueRange();

			valueRange.Values = new List<IList<object>> { entry };

			var appendRequest = this.spreadsheetService.Spreadsheets.Values.Append(valueRange, this.spreadsheetId, range);
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
			var appendReponse = appendRequest.Execute();
		}

		public IList<IList<object>> ReadRow(int rowId)
		{
			var range = $"{this.spreadsheetSheetName}!A{rowId}:Z";
			SpreadsheetsResource.ValuesResource.GetRequest request =
			this.spreadsheetService.Spreadsheets.Values.Get(this.spreadsheetId, range);

			var response = request.Execute();
			return response.Values;
		}

		public void UpdateRow(int rowId, List<object> entry)
		{
			var range = $"{this.spreadsheetSheetName}!A{rowId}:Z";
			var valueRange = new ValueRange();

			valueRange.Values = new List<IList<object>> { entry };

			var updateRequest = this.spreadsheetService.Spreadsheets.Values.Update(valueRange, this.spreadsheetId, range);
			updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			var appendReponse = updateRequest.Execute();
		}

		public void DeleteRow(int rowId)
		{
			var range = $"{this.spreadsheetSheetName}!{rowId}:Z";
			var requestBody = new ClearValuesRequest();

			var deleteRequest = this.spreadsheetService.Spreadsheets.Values.Clear(requestBody, this.spreadsheetId, range);
			var deleteReponse = deleteRequest.Execute();
		}

		public IList<IList<object>> ReadAll()
		{
			var range = $"{this.spreadsheetSheetName}!A:Z";
			SpreadsheetsResource.ValuesResource.GetRequest request =
			this.spreadsheetService.Spreadsheets.Values.Get(this.spreadsheetId, range);

			var response = request.Execute();
			return response.Values;
		}

		public void FabelInsert(List<IList<object>> obs) {
			foreach (List<object> row in obs) {
				this.UpdateRow(((int)row[15])+1, row);
			}
		}

		public void FillAll(List<IList<object>> obs)
		{
			var range = $"{this.spreadsheetSheetName}!A:Z";
			var valueRange = new ValueRange();

			valueRange.Values = obs;

			var appendRequest = this.spreadsheetService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
			var appendReponse = appendRequest.Execute();
		}
	}
}
