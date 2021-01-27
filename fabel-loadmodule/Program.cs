using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Access = Microsoft.Office.Interop.Access;

namespace fabel_loadmodule
{
	class Program
	{
		static void Main(string[] args)
		{
			Access.Application acApp = new Access.Application();

			for(int i = 0; i < args.Length; i++)
			{
				Console.WriteLine($"args[{i}] = {args[i]}");
			}

			acApp.OpenCurrentDatabase($@"{args[0]}", false, null);
			object oMissing = System.Reflection.Missing.Value;
			//Run the Test macro in the module
			Console.WriteLine(acApp);
			object[] runargs = new object[0];
			acApp.Run($"{args[1]}");
			acApp.Quit();
			
		}
	}
}
