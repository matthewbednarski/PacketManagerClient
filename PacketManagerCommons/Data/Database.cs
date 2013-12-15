/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 25/11/2013
 * Time: 21:11
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;
using PacketManagerCommons.Model;
using Plugin;
using Plugin.WPF;
using SQLite;
using StructureMap.Query;

namespace PacketManagerCommons.Data
{
	/// <summary>
	/// Description of Database.
	/// </summary>
	public class Database : SQLiteConnection
	{
		static object sync = new object();
		private static Database _instance;
		public static Database Instance
		{
			get{
				lock(sync)
				{
					if(_instance == null)
					{
						_instance = new Database();
					}
					return _instance;
				}
			}
		}
		public abstract class DBTable
		{
			ISettings _settings;
			public ISettings Settings
			{
				get{
					if(_settings == null){
						_settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
					}
					return _settings;
				}
			}
			private Database _db
			{
				get{
					return Database.Instance;
				}
			}
			
			private BackgroundWorker _bw;
			private BackgroundWorker bw
			{
				get{
					if(_bw == null)
					{
						_bw = new BackgroundWorker();
					}
					return _bw;
				}
			}
			[PrimaryKey]
			public Guid Id {get;set;}
			
			
			public DateTime DateCreated{get;set;}
			public DateTime DateModified{get;set;}
			
			public abstract String RepoDir {get;set;}
			public DBTable()
			{
				this.Id = Guid.NewGuid();
				this.DateCreated = DateTime.Now;
			}
			public bool Insert()
			{
				this.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
				this.DateModified = DateTime.Now;
				int r = _db.Insert(this);
				if(r > 0)
				{
					return true;
				}else{
					return false;
				}
			}
			public bool Update(bool tryInsert = true)
			{
				try{
					this.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
					this.DateModified = DateTime.Now;
					int r = _db.Update(this);
					if(r > 0){
						return true;
					}else if(tryInsert){
						return this.Insert();
					}else{
						return false;
					}
				}
				catch(Exception ex)
				{
					ex.StackTrace.ToLower();
				}
				return false;
			}
			public bool Delete()
			{
				this.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
				int r = _db.Delete(this);
				if(r > 0)
				{
					return true;
				}else{
					return false;
				}
			}
			public void UpdateAsync(bool tryInsert = true)
			{
				try{
					this.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
					this.DateModified = DateTime.Now;
					bw.DoWork += delegate(object sender, DoWorkEventArgs e)
					{
						int r = _db.Update(this);
						if(r > 0){
							e.Result =   true;
						}else if(tryInsert){
							e.Result =  this.Insert();
						}else{
							e.Result =   false;
						}
					};
					bw.RunWorkerAsync();
				}
				catch(Exception ex)
				{
					ex.StackTrace.ToLower();
				}
			}
			public void InsertAsync()
			{
				
				try{
					this.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
					this.DateModified = DateTime.Now;
					bw.DoWork += delegate(object sender, DoWorkEventArgs e)
					{
						int r = _db.Insert(this);
						if(r > 0)
						{
							e.Result =  true;
						}else{
							e.Result =   false;
						}
					};
					bw.RunWorkerAsync();
				}
				catch(Exception ex)
				{
					ex.StackTrace.ToLower();
				}
			}
			public void DeleteAsync()
			{
				try{
					this.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
					this.DateModified = DateTime.Now;
					bw.DoWork += delegate(object sender, DoWorkEventArgs e)
					{
						int r = _db.Delete(this);
						if(r > 0)
						{
							e.Result =  true;
						}else{
							e.Result =   false;
						}
					};
					bw.RunWorkerAsync();
				}
				catch(Exception ex)
				{
					ex.StackTrace.ToLower();
				}
			}
		}
		
		public class App: DBTable
		{
			public App():base()
			{
			}
			
			[ Unique(Name="uk_app_name")]
			public String Name{get;set;}
			[Indexed]
			[ Unique(Name="uk_app_name")]
			public String Arch{get;set;}
			[Indexed]
			[ Unique(Name="uk_app_name")]
			public String OS{get;set;}
			[Indexed]
			[ Unique(Name="uk_app_name")]
			public override String RepoDir {get;set;}
			
			public Boolean SyncLatest{get;set;}
			
			public String Latest{get;set;}
			
			
			
		}
		public class Vers: DBTable
		{
			public Vers():base()
			{
			}
			
			[ Unique(Name="uk_vers_name")]
			public String Name{get;set;}
			[Indexed]
			[ Unique(Name="uk_vers_name")]
			public String App{get;set;}
			[Indexed]
			[ Unique(Name="uk_vers_name")]
			public String Arch{get;set;}
			[Indexed]
			[ Unique(Name="uk_vers_name")]
			public String OS{get;set;}
			[Indexed]
			[ Unique(Name="uk_vers_name")]
			public override String RepoDir {get;set;}
			
			public Boolean Sync{get;set;}
			
			public String File{get;set;}
			
			
		}
		public class Setting:DBTable
		{
			
			public Setting():base()
			{
			}
			
			[ Unique(Name="uk_setting")]
			public String Key{get;set;}
			[Indexed]
			public String Value{get;set;}
			[Indexed]
			[ Unique(Name="uk_setting")]
			public override String RepoDir {get;set;}
		}
		ISettings _settings;
		public ISettings Settings
		{
			get{
				if(_settings == null){
					_settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
				}
				return _settings;
			}
		}
		private Database () : base(
			Path.Combine(
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packet"),
				@"packet_app.db")
		)
		{
			CreateTable<Setting>();
			CreateTable<App> ();
			CreateTable<Vers> ();
			
		}
		public IEnumerable<Setting> QueryAllSettings()
		{
			var t =  this.Table<Setting>().Cast<Setting>().OrderBy(xx => xx.RepoDir).ThenBy(xx => xx.Key);
			return t;
		}
		public Setting QuerySetting(string key)
		{
			
			var t =  this.Table<Setting>().Cast<Setting>().Where(xx => xx.Key != null && xx.Key.Equals(key)
			                                                     && (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY]))
			                                                    );
			if(t == null || t.Count() < 1)
			{
				return null;
			}
			return t.First();
		}
		public  IEnumerable<Vers>  QueryAllVers()
		{
			
			var t =      this.Table<Vers>().Cast<Vers>().Where(xx =>  (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY])));
			if(t == null || t.Count() < 1)
			{
				return null;
			}else{
				t = t.OrderBy(xx => xx.OS).ThenBy(xx => xx.Arch).ThenBy(xx => xx.App).ThenBy(xx => xx.Name);
				return t;
			}
		}
		public  Vers QueryVers (String os, String arch, String app, String name)
		{
			var t =  this.Table<Vers>().Where<Vers>( xx =>
			                                        xx.Name != null && xx.Name.Equals(name) &&
			                                        xx.App != null && xx.App.Equals(app) &&
			                                        xx.Arch != null && xx.Arch.Equals(arch) &&
			                                        xx.OS != null && xx.OS.Equals(os) &&
			                                        (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY]))
			                                       );
			if(t == null || t.Count() < 1)
			{
				return null;
			}
			return t.First();
		}
		public  App QueryApp (String os, String arch, String name)
		{
			var t = this.Table<App>().Where<App>( xx =>
			                                     xx.Name != null && xx.Name.Equals(name) &&
			                                     xx.Arch != null && xx.Arch.Equals(arch) &&
			                                     xx.OS != null && xx.OS.Equals(os)&&
			                                     (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY]))
			                                    );
			if(t == null || t.Count() < 1)
			{
				return null;
			}
			return t.First();
		}
		public  IEnumerable<Vers>  QueryVers (App app)
		{
			return this.Table<Vers>().Where<Vers>( xx =>
			                                      xx.App != null &&
			                                      xx.App.Equals(app.Name) &&
			                                      xx.Arch != null && xx.Arch.Equals(app.Arch) &&
			                                      xx.OS != null && xx.OS.Equals(app.OS) &&
			                                      (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY]))
			                                     ).Cast<Vers>().OrderBy(xx => xx.OS).ThenBy(xx => xx.Arch).ThenBy(xx => xx.App).ThenBy(xx => xx.Name);
		}
		public  IEnumerable<App>  QueryAllApp()
		{
			
			var t =  this.Table<App>().Cast<App>().Where(xx =>
			                                             (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY])) );
			
			if(t == null || t.Count() < 1)
			{
				return null;
			}else{
				t = t.OrderBy(xx => xx.OS).ThenBy(xx => xx.Arch).ThenBy(xx => xx.Name);
				return t;
			}
		}
		public  App  QueryAppByVers (Vers vers)
		{
			var t = this.Table<App>().Where( xx =>
			                                xx.Name != null &&  xx.Name.Equals(vers.App) &&
			                                xx.Arch != null && xx.Arch.Equals(vers.Arch) &&
			                                xx.OS != null && xx.OS.Equals(vers.OS) &&
			                                (xx.RepoDir != null && xx.RepoDir.Equals(Settings[RestApi.LOCAL_REPO_START_DIR_KEY]))
			                               );
			if(t == null || t.Count() < 1)
			{
				return null;
			}
			return t.First();
		}
	}
}
