using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fabel_extractor_backend.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GoogleSpreadsheetController : ControllerBase
	{

		private readonly ILogger<GoogleSpreadsheetController> _logger;

		public GoogleSpreadsheetController(ILogger<GoogleSpreadsheetController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public HttpResponse Get()
		{

			string spreadsheetName = Request.Query["spreadsheetName"];

			SpreadsheetManager.Instance.SpreadsheetSheetName = spreadsheetName;

			string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(SpreadsheetManager.Instance.ReadAll());

			byte[] jsonDataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

			Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
			Response.Body.Write(jsonDataBytes, 0, jsonDataBytes.Length);

			return Response;
		}

		[HttpPut]
		public HttpResponse Put()
		{
			string spreadsheetName = Request.Form["spreadsheetName"];
			string spreadsheetData = Request.Form["spreadsheetData"];

			SpreadsheetManager.Instance.SpreadsheetSheetName = spreadsheetName;

			List<IList<object>> obj = (List<IList<object>>)DeserializeBase64(spreadsheetData);

			SpreadsheetManager.Instance.FabelInsert(obj);

			Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

			return Response;
		}

		[HttpDelete]
		public HttpResponse Delete()
		{
			HttpResponse response = null;

			return response;
		}

		public static object DeserializeBase64(string s)
		{
			// We need to know the exact length of the string - Base64 can sometimes pad us by a byte or two
			int p = s.IndexOf(':');
			int length = Convert.ToInt32(s.Substring(0, p));

			// Extract data from the base 64 string!
			byte[] memorydata = Convert.FromBase64String(s.Substring(p + 1));
			MemoryStream rs = new MemoryStream(memorydata, 0, length);
			BinaryFormatter sf = new BinaryFormatter();
			object o = sf.Deserialize(rs);
			return o;
		}
	}
}
