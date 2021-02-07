using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fabel_extractor
{
	[Serializable]
	public class FabelEntry
	{
		private int index;
		private int vIID;
		private DateTime date;
		private string dienstzeit;
		private string tourname;
		private DateTime from;
		private DateTime to;
		private int adminBezirk;
		private string vehicle;
		private string vehicleType;
		private string comment;
		private string driver;
		private string medic;
		private string medic2;
		private string doctor;
		private int rowNumber;

		public FabelEntry(int index, int vIID, DateTime date, string dienstzeit, string tourname, DateTime from, DateTime to, int adminBezirk, string vehicle, string vehicleType, string comment, string driver, string medic, string medic2, string doctor, int rowNumber)
		{
			this.Index = index;
			this.VIID = vIID;
			this.Date = date;
			this.Dienstzeit = dienstzeit;
			this.Tourname = tourname;
			this.From = from;
			this.To = to;
			this.AdminBezirk = adminBezirk;
			this.Vehicle = vehicle;
			this.VehicleType = vehicleType;
			this.Comment = comment;
			this.Driver = driver;
			this.Medic = medic;
			this.Medic2 = medic2;
			this.Doctor = doctor;
			this.RowNumber = rowNumber;
		}

		public int Index { get => index; set => index = value; }
		public int VIID { get => vIID; set => vIID = value; }
		public DateTime Date { get => date; set => date = value; }
		public string Dienstzeit { get => dienstzeit; set => dienstzeit = value; }
		public string Tourname { get => tourname; set => tourname = value; }
		public DateTime From { get => from; set => from = value; }
		public DateTime To { get => to; set => to = value; }
		public int AdminBezirk { get => adminBezirk; set => adminBezirk = value; }
		public string Vehicle { get => vehicle; set => vehicle = value; }
		public string VehicleType { get => vehicleType; set => vehicleType = value; }
		public string Comment { get => comment; set => comment = value; }
		public string Driver { get => driver; set => driver = value; }
		public string Medic { get => medic; set => medic = value; }
		public string Medic2 { get => medic2; set => medic2 = value; }
		public string Doctor { get => doctor; set => doctor = value; }
		public int RowNumber { get => rowNumber; set => rowNumber = value; }

		public override bool Equals(object obj)
		{
			return obj is FabelEntry entry &&
						 VIID == entry.VIID &&
						 Date == entry.Date;
		}

		public override int GetHashCode()
		{
			int hashCode = -2043952930;
			hashCode = hashCode * -1521134295 + VIID.GetHashCode();
			hashCode = hashCode * -1521134295 + Date.GetHashCode();
			return hashCode;
		}

		public override string ToString()
		{
			return $"{{{nameof(Index)}={Index.ToString()}, {nameof(VIID)}={VIID.ToString()}, {nameof(Date)}={Date.ToString()}, {nameof(Dienstzeit)}={Dienstzeit}, {nameof(Tourname)}={Tourname}, {nameof(From)}={From.ToString()}, {nameof(To)}={To.ToString()}, {nameof(AdminBezirk)}={AdminBezirk.ToString()}, {nameof(Vehicle)}={Vehicle}, {nameof(VehicleType)}={VehicleType}, {nameof(Comment)}={Comment}, {nameof(Driver)}={Driver}, {nameof(Medic)}={Medic}, {nameof(Medic2)}={Medic2}, {nameof(Doctor)}={Doctor}, {nameof(RowNumber)}={RowNumber.ToString()}}}";
		}
	}
}
