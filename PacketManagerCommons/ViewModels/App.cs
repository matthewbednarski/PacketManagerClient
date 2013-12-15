/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:55
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MVVm.Core;
using PacketManagerCommons.Data;
using PacketManagerCommons.Model;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of App.
	/// </summary>
	public class App:PacketListViewModelBase
	{
		private Database _db
		{
			get{
				return Database.Instance;
			}
		}
		private Data.Database.App _dbitem;
		public Data.Database.App DBItem
		{
			get{
				if(_dbitem == null)
				{
					_dbitem = _db.QueryApp(this.OS, this.Arch, this.Name);
					if(_dbitem == null)
					{
						_dbitem = new Database.App();
					}else {
						this.SyncLatest = _dbitem.SyncLatest;
					}
				}
				_dbitem.Name = this.Name;
				_dbitem.Arch = this.Arch;
				_dbitem.OS = this.OS;
				_dbitem.SyncLatest =this.SyncLatest;
				if(this.Latest != null)
				{
					_dbitem.Latest = this.Latest.Name;
				}
				return _dbitem;
			}
		}

		[MediatorMessageSink("Persist")]
		void CloseWindow(object dummy)
		{
			this.DBItem.UpdateAsync(true);
		}
		
		string packetDirBase
		{
			get{
				return baseRepoDir + "\\{0}\\{1}\\{2}";
			}
		}
		public override string Dir
		{
			get{
				return String.Format(packetDirBase, new string[]{ this.OS, this.Arch, this.Name} );
			}
		}
		RelayCommand _deleteDir;
		public RelayCommand DeleteDir
		{
			get{
				if(_deleteDir == null)
				{
					_deleteDir = new RelayCommand( xx =>
					                              {
					                              	try{
					                              		this.SyncLatest = false;
					                              		foreach(var vers in this.List)
					                              		{
					                              			if(vers is Vers)
					                              			{
					                              				Vers v = vers as Vers;
					                              				v.Sync = false;
					                              				v.File = string.Empty;
					                              				v.DBItem.Update(true);
					                              			}
					                              		}
					                              		this.DBItem.Update(true);
					                              		Directory.Delete(this.Dir, true);
					                              		this.Mediator.NotifyColleagues("OSes.Reset", "Refresh");
					                              	}catch(Exception ex){
					                              		Debug.WriteLine(ex.Message);
					                              	}
					                              }, xx => this.DirExists);
				}
				return _deleteDir;
				
			}
		}
		
		
		bool _syncLatest;
		public bool SyncLatest
		{
			get{
				return _syncLatest;
			}
			set{
				if(_syncLatest != value)
				{
					_syncLatest = value;
					this.OnPropertyChanged("SyncLatest");
				}
			}
		}

		public Vers Latest
		{
			get{
				Vers latest = null;
				if(this.List != null && this.List.Count > 0)
				{
					latest = this.List.Last() as Vers;
				}
				return latest;
			}
		}
		string _os;
		public String OS
		{
			get{
				return _os;
			}
			set{
				if(value != null && !value.Equals(_os))
				{
					this._os = value;
					this.OnPropertyChanged("OS");
				}
			}
		}
		string _arch;
		public String Arch
		{
			get{
				return _arch;
			}
			set{
				if(value != null && !value.Equals(_arch))
				{
					this._arch = value;
					this.OnPropertyChanged("Arch");
				}
			}
		}
		
		string _app;
		public override String Name
		{
			get{
				return _app;
			}
			set{
				if(value != null && !value.Equals(_app))
				{
					this._app = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
		ObservableCollection<PacketListViewModelBase> _versionsList;
		public override ObservableCollection<PacketListViewModelBase> List
		{
			get{
				if(_versionsList == null)
				{
					_versionsList = new ObservableCollection<PacketListViewModelBase>();
					packets ps = this.RestApi.GetVersions(this.OS, this.Arch, this.Name);
					if(ps != null)
					{
						foreach(packet p in ps.packet)
						{
							Vers a = new Vers();
							a.OS = this.OS;
							a.Arch = this.Arch;
							a.App = this.Name;
							a.Name = p.name;
							if(this.IsMatch(a.Name)){
								_versionsList.Add(a);
								if(a.DBItem != null)
								{
									a.DBItem.ToString();
								}
								if(this.SyncLatest)
								{
									a.CanSync = false;
								} else{
									a.CanSync = true;
								}
							}
						}
					}
				}
				return _versionsList;
			}
		}
		public App():base()
		{
			this.Filter = Settings[RestApi.VERS_KEY];
			this.PropertyChanged += new PropertyChangedEventHandler(App_PropertyChanged);
		}

		void App_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName.Equals("SyncLatest"))
			{
				if(this.SyncLatest)
				{
					foreach(var p in this.List)
					{
						(p as Vers).CanSync = false;
						(p as Vers).Sync = false;
					}
					this.Latest.DownloadAsync();
				} else{
					foreach(var p in this.List)
					{
						(p as Vers).CanSync = true;
					}
				}
			} else if (e.PropertyName.Equals("List"))
			{
				this.OnPropertyChanged("Latest");
			}
		}
	}
}
