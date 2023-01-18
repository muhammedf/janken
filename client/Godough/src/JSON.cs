using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Godough
{
	internal static class fJSON
	{
		public static string Serialize(object obj)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}

		public static T Deserialize<T>(string json)
		{
			GD.Print(typeof(T).Name, json);
			try
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
			}
			catch (Exception ex)
			{
				GD.PrintErr(ex.Message);
				throw;
			}
		}
	}
}
