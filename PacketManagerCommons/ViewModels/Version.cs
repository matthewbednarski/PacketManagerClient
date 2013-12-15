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
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using MVVm.Core;
using PacketManagerCommons.Data;
using PacketManagerCommons.Model;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of App.
	/// </summary>
	public class Vers:PacketListViewModelBase
	{
		private Database _db
		{
			get{
				return Database.Instance;
			}
		}
		private Data.Database.Vers _dbitem;
		public Data.Database.Vers DBItem
		{
			get{
				if(_dbitem == null)
				{
					_dbitem = _db.QueryVers(this.OS, this.Arch, this.App, this.Name);
					if(_dbitem == null)
					{
						_dbitem = new Database.Vers();
					}else {
						this.Sync = _dbitem.Sync;
						this.File = _dbitem.File;
					}
				}
				_dbitem.Name = this.Name;
				_dbitem.App = this.App;
				_dbitem.Arch = this.Arch;
				_dbitem.OS = this.OS;
				_dbitem.Sync = this.Sync;
				_dbitem.File = this.File;
				_dbitem.RepoDir = this.Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
				return _dbitem;
			}
		}
		[MediatorMessageSink("Persist")]
		void CloseWindow(object dummy)
		{
			this.DBItem.UpdateAsync(true);
		}
		bool _sync;
		public bool Sync
		{
			get{
				
				return _sync;
			}
			set{
				if(_sync != value)
				{
					_sync = value;
					this.OnPropertyChanged("Sync");
				}
			}
		}
		bool _canSync;
		public bool  CanSync
		{
			get{
				return _canSync;
			}
			set{
				if(_canSync != value)
				{
					_canSync = value;
					this.OnPropertyChanged("CanSync");
				}
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
		public String App
		{
			get{
				return _app;
			}
			set{
				if(value != null && !value.Equals(_app))
				{
					this._app = value;
					this.OnPropertyChanged("App");
				}
			}
		}
		string _version;
		public  override String Name
		{
			get{
				return _version;
			}
			set{
				if(value != null && !value.Equals(_version))
				{
					this._version = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
		public override ObservableCollection<PacketListViewModelBase> List
		{
			get{
				return null;
			}
		}
		String cd = string.Empty;
		ICommand _download;
		public ICommand DownloadCommand
		{
			get{
				if(_download == null)
				{
					_download = (ICommand)new RelayCommand( xx => {
					                                       	string path  ="";
					                                       	if(this.Download(out path) && System.IO.File.Exists(path))
					                                       	{
					                                       		SaveFileDialog ofd = new SaveFileDialog();
					                                       		ofd.FileName = Path.GetFileName(path);
					                                       		if(cd.Equals(String.Empty))
					                                       		{
					                                       			cd =  Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
					                                       		}
					                                       		ofd.InitialDirectory = cd;
					                                       		bool? res = ofd.ShowDialog();
					                                       		if(res != null  && res.Equals(true))
					                                       		{
					                                       			cd = Path.GetDirectoryName(ofd.FileName);
					                                       			try{
					                                       				System.IO.File.Copy(path, ofd.FileName);
					                                       			} catch(IOException ex)
					                                       			{
					                                       				Debug.WriteLine(ex.Message);
					                                       			}
					                                       		}
					                                       	}
					                                       }, xx => true);
				}
				return _download;
			}
		}
		public void DownloadAsync()
		{
			this.Mediator.NotifyColleagues("SyncQueue.Version.Enqueue", (Action)(() => {
			                                                                     	Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
			                                                                     	                                                                                 	this.IsDownloading = true;
			                                                                     	                                                                                 	this.Download();
			                                                                     	                                                                                 	this.IsDownloading = false;
			                                                                     	                                                                                 }));
			                                                                     }));
		}
		public bool Download()
		{
			bool r = false;
			string path = string.Empty;
			
			packets ps = RestApi.GetPacket(this.OS, this.Arch, this.App, this.Name, out path);
			if(ps != null && ps.packet != null && ps.packet.First() != null && ps.packet.First().version.Equals(this.Name))
			{
				this.File = path;
				r = true;
			}
			return r;
		}
		public bool Download(out string path)
		{
			bool r = false;
			
			packets ps = RestApi.GetPacket(this.OS, this.Arch, this.App, this.Name, out path);
			if(ps != null && ps.packet != null && ps.packet.First() != null && ps.packet.First().version.Equals(this.Name))
			{
				r = true;
			}
			return r;
		}
		bool isDownloading;
		public bool IsDownloading
		{
			get{
				return isDownloading;
			}
			set{
				if(value != isDownloading)
				{
					isDownloading = value;
					this.OnPropertyChanged("IsDownloading");
				}
			}
		}
		
		public bool FileExists
		{
			get{
				bool fe = false;
				if(String.IsNullOrEmpty(this.File))
				{
					return false;
				} else {
					fe = System.IO.File.Exists(this.File);
				}
				return fe;
			}
		}
		string _sync_file;
		private string _file = string.Empty;
		public string File
		{
			get {
				return _file;
			}
			set{
				if(value != null)
				{
					if(!value.Equals(_file))
					{
						_file = value;
						this.OnPropertyChanged("File");
						this.OnPropertyChanged("FileExists");
					}
				}
			}
		}
		
		
		string packetDirBase
		{
			get{
				return baseRepoDir + "\\{0}\\{1}\\{2}\\{3}";
			}
		}
		public override string Dir
		{
			get{
				return String.Format(packetDirBase, new string[ ]{this.OS, this.Arch, this.App, this.Name});
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
					                              		this.Sync = false;
					                              		this.File = string.Empty;
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

		public Vers():base()
		{
			this.PropertyChanged += new PropertyChangedEventHandler(Vers_PropertyChanged);
		}

		void Vers_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			if(e.PropertyName.Equals("Sync"))
			{
				if(this.Sync)
				{
					DownloadAsync();
				}
			} else if (e.PropertyName.Equals("FileExists"))
			{
				if(this.FileExists)
				{
					this.Mediator.NotifyColleagues("SyncQueue.Version.Dequeue", this);
				}
			}
		}
	}
}
