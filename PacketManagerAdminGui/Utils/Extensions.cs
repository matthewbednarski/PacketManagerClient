/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 31/10/2013
 * Time: 21:08
 * 
 */
using System;
using System.Drawing;
using System.IO;

namespace PacketManagerAdminGui.Utils
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public static class Extensions
	{
		public static Icon IconFromFilePath(this string filePath)
		{
			var result = (Icon)null;
			try
			{
				result = Icon.ExtractAssociatedIcon(filePath);
			}
			catch (System.Exception)
			{
				// swallow and return nothing. You could supply a default Icon here as well
			}
			return result;
		}
				
		public static String SaveIconFromFilePath(this string filePath)
		{
			string r = "";
			var icon = filePath.IconFromFilePath();
			if(icon != null)
			{
				r = Path.GetTempPath();
				r = Path.Combine(r, Path.GetFileName(filePath));
				if(!File.Exists(r))
				{
					using(FileStream fs = new FileStream(r, FileMode.Create))
					{
						icon.Save(fs);
					}
				}
			}
			return r;
		}
	}
}
