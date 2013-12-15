/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 08:09
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MVVm.Core;
using PacketManagerCommons.Data;
using PacketManagerCommons.Model;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	public class Settings : Plugin.WPF.Settings
	{
		private Database _db
		{
			get{
				return Database.Instance;
			}
		}
		private Data.Database.Setting _dbitem;
		public Data.Database.Setting RepoDirSetting
		{
			get{
				if(_dbitem == null)
				{
					_dbitem = _db.QuerySetting( RestApi.LOCAL_REPO_START_DIR_KEY);
					if(_dbitem == null)
					{
						_dbitem = new Database.Setting();
						_dbitem.Key = RestApi.LOCAL_REPO_START_DIR_KEY;
						_dbitem.Value = this[ RestApi.LOCAL_REPO_START_DIR_KEY];
						_dbitem.UpdateAsync(true);
					}
					if(!_dbitem.Value.Equals(this[ RestApi.LOCAL_REPO_START_DIR_KEY]))
					{
						_dbitem = new Database.Setting();
						_dbitem.Key = RestApi.LOCAL_REPO_START_DIR_KEY;
						_dbitem.Value = this[ RestApi.LOCAL_REPO_START_DIR_KEY];
						_dbitem.UpdateAsync(true);
					}
				}
				return _dbitem;
			}
		}
		
		[MediatorMessageSink("Persist")]
		void CloseWindow(object dummy)
		{
			foreach(var sett in this.Values)
			{
				var item = _db.QuerySetting( sett.Key);
				if(item == null)
				{
					item = new Database.Setting();
					item.Key = sett.Key;
					item.Value =sett.Value;
					item.UpdateAsync(true);
				}
			}
		}

		public Settings() :base()
		{
			"".ToLower();
		}
	}
}
