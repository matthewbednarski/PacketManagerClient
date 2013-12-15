/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:40
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using MVVm.Core;
using PacketManagerCommons.Model;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of Apps.
	/// </summary>
	public class Arch: PacketListViewModelBase
	{
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
		public override String Name
		{
			get{
				return _arch;
			}
			set{
				if(value != null && !value.Equals(_arch))
				{
					this._arch = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
		ObservableCollection<PacketListViewModelBase> _appsList;
		public override ObservableCollection<PacketListViewModelBase> List
		{
			get{
				if(_appsList == null)
				{
					_appsList = new ObservableCollection<PacketListViewModelBase>();
					packets ps = this.RestApi.GetApp(this.OS, this.Name);
					if(ps != null)
					{
						foreach(packet p in ps.packet)
						{
							App a = new App();
							a.OS = this.OS;
							a.Arch = this.Name;
							a.Name = p.name;
							if(this.IsMatch(a.Name)){
								_appsList.Add(a);
								
								if(a.DBItem != null)
								{
									a.DBItem.ToString();
								}
							}
						}
					}
				}
				return _appsList;
			}
		}
		
		string packetDirBase
		{
			get{
				return baseRepoDir + "\\{0}\\{1}";
			}
		}
		public override string Dir
		{
			get{
				return String.Format(packetDirBase, new string[]{ this.OS, this.Name});
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
					                              		foreach(var app in this.List)
					                              		{
					                              			if(app is App)
					                              			{
					                              				App a = app as App;
					                              				if(a.SyncLatest)
					                              				{
					                              					a.SyncLatest = false;
					                              					a.DBItem.Update(true);
					                              				}else{
					                              					foreach(var vers in a.List)
					                              					{
					                              						if(vers is Vers)
					                              						{
					                              							Vers v = vers as Vers;
					                              							v.Sync = false;
					                              							v.File = string.Empty;
					                              							v.DBItem.Update(true);
					                              						}
					                              					}
					                              				}
					                              			}
					                              		}
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
		
		public Arch():base()
		{
			this.Filter = Settings[RestApi.APP_KEY];
		}
	}
}
