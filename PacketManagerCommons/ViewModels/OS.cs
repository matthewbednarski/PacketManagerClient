/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:37
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
	/// Description of Archs.
	/// </summary>
	public class OS:PacketListViewModelBase
	{
		string _os;
		public override String Name
		{
			get{
				return _os;
			}
			set{
				if(value != null && !value.Equals(_os))
				{
					this._os = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
		ObservableCollection<PacketListViewModelBase> _archsList;
		public override ObservableCollection<PacketListViewModelBase> List
		{
			get{
				if(_archsList == null)
				{
					_archsList = new ObservableCollection<PacketListViewModelBase>();
					packets ps = this.RestApi.GetArchs(this.Name);
					if(ps != null)
					{
						foreach(packet p in ps.packet)
						{
							Arch a = new Arch();
							a.OS = this.Name;
							a.Name = p.name;
							if(this.IsMatch(a.Name)){
								_archsList.Add(a);
							}
						}
					}
				}
				return _archsList;
			}
			
		}
		
		string packetDirBase
		{
			get{
				return baseRepoDir + "\\{0}";
			}
		}
		public override string Dir
		{
			get{
				return String.Format(packetDirBase, new string[]{  this.Name});
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
					                              		foreach(var archt in this.List)
					                              		{
					                              			if(archt is Arch)
					                              			{
					                              				Arch arch = archt as Arch;
					                              				foreach(var app in arch.List)
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
		
		public OS():base()
		{
			this.Filter = Settings[RestApi.ARCH_KEY];
		}
	}
}
