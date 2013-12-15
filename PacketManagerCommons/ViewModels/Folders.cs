/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:12
 * 
 */
using System;
using System.Windows.Input;
using System.Windows.Threading;
using MVVm.Core;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of AddPacket.
	/// </summary>
	public class Folders:PacketViewModelBase
	{

		public override string Dir {
			get {
				return this.baseRepoDir;
			}
		}
		public override string Name
		{
			get{
				return this.DisplayName;
			}
			set{
				this.DisplayName = value;
				this.OnPropertyChanged("Name");
			}
		}
		OSes _oses;
		public OSes OSes
		{
			get{
				if(_oses == null)
				{
					_oses = StructureMap.ObjectFactory.GetInstance<OSes>();
					
				}
				return _oses;
			}
		}
		OS _os;
		public OS CurrentOS
		{
			get{
				return _os;
			}
			set{
				if(value != null)
				{
					_os = value;
					this.OnPropertyChanged("CurrentOS");
				}
			}
		}
		
		Arch _arch;
		public Arch CurrentArch
		{
			get{
				return _arch;
			}
			set{
				if(value != null)
				{
					_arch = value;
					this.OnPropertyChanged("CurrentArch");
				}
			}
		}
		App  _app;
		public App CurrentApp
		{
			get{
				return _app;
			}
			set{
				if(value != null)
				{
					_app = value;
					this.OnPropertyChanged("CurrentApp");
				}
			}
		}
		Vers _version;
		public Vers CurrentVers
		{
			get{
				return _version;
			}
			set{
				if(value != null)
				{
					_version = value;
					this.OnPropertyChanged("CurrentVers");
				}
			}
		}

		RelayCommand _refresh;
		public RelayCommand Refresh
		{
			get{
				if(_refresh == null){
					_refresh = new RelayCommand(xx => {
					                            	
					                            	this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(()=>{   	this.Mediator.NotifyColleagues("OSes.Reset", "none");}));
					                            }, xx =>
					                            {
					                            	return !this.IsBusy    ;
					                            }
					                           );
				}
				return _refresh ;
			}
		}
//		string cd = String.Empty;
//		public AddPacket()
//		{
//			this.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
//			{
//				if(e.PropertyName.Equals("PacketPath"))
//				{
//					GetPacketData();
//				}
//			};
//		}
//		private void GetPacketData()
//		{
//			if(File.Exists(this.PacketPath))
//			{
//				this.Packet.name =Path.GetFileName(this.PacketPath);
//				this.Packet.arch = this.CurrentArch.Name;
//				this.Packet.os = this.CurrentOS.Name;
//				this.Packet.dateCreated = DateTime.Now;
//				this.Packet.dateModified = DateTime.Now;
//				this.Packet.dateCreatedSpecified = true;
//				this.Packet.dateModifiedSpecified = true;
//				byte[] bytes =  File.ReadAllBytes(this.PacketPath);
//				this.Packet.data = bytes;
//				this.OnPropertyChanged("Load");
//			}
//		}
		
	}
}
