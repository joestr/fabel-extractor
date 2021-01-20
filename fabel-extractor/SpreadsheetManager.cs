using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.IO;

namespace fabel_extractor
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

		public SpreadsheetManager(string appCredentialPath, string appName, string spreadsheetId, string spreadsheetSheetName)
		{
			this.appCredentialPath = appCredentialPath;
			this.appName = appName;
			this.spreadsheetId = spreadsheetId;
			this.spreadsheetSheetName = spreadsheetSheetName;

			AuthAndInit();
		}

		private void AuthAndInit()
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

			var deleteRequest = this.spreadsheetService.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
			var deleteReponse = deleteRequest.Execute();
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
