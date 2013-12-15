/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:12
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using MVVm.Core;
using PacketManagerCommons.Model;
using PacketManagerCommons.ViewModels;
using PacketManagerAdminGui.Utils;

namespace PacketManagerAdminGui.ViewModels
{
	/// <summary>
	/// Description of AddPacket.
	/// </summary>
	public class AddPacket:PacketViewModelBase
	{

		
		public override string Dir {
			get {
				return this.baseRepoDir;
			}
		}
		internal string baseRepoDir
		{
			get{
				string dir = Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
				if(!Directory.Exists(dir))
				{
					try{
						Directory.CreateDirectory(dir);
					}catch(IOException ex)
					{
						
					}
				}
				return dir;
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

		packet _packet;
		public packet Packet
		{
			get{
				if(_packet == null)
				{
					_packet = new packet();
				}
				return _packet;
			}
		}
		string _packetPath = String.Empty;
		public String PacketPath
		{
			get{
				return _packetPath;
			}
			set{
				if(value != null && !value.Equals(_packetPath))
				{
					if(!String.IsNullOrEmpty(_packetPath)){
						this.Images.Clear();
					}
					this._packetPath = value;
					this.OnPropertyChanged("PacketPath");
				}
			}
		}
		RelayCommand _browse;
		public ICommand Browse
		{
			get{
				if(_browse == null){
					_browse = new RelayCommand(xx => {
					                           	
					                           	OpenFileDialog ofd = new OpenFileDialog();
					                           	if(cd.Equals(String.Empty))
					                           	{
					                           		cd =  Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
					                           	}
					                           	ofd.InitialDirectory = cd;
					                           	bool? res = ofd.ShowDialog();
					                           	if(res != null  && res.Equals(true))
					                           	{
					                           		cd = Path.GetDirectoryName(ofd.FileName);
					                           		this.PacketPath = ofd.FileName;
					                           	}
					                           }, xx => {
					                           	return this.CurrentOS != null && this.CurrentArch != null;
					                           });
				}
				return _browse as ICommand;
			}
		}
		RelayCommand _load;
		public ICommand Load
		{
			get{
				if(_load == null){
					_load = new RelayCommand(xx => {
					                         	this.LoadPacket();
					                         }, xx => {
					                         	bool canLoad =  !String.IsNullOrEmpty(this.Packet.name);
					                         	if(canLoad)
					                         	{
					                         		canLoad = !String.IsNullOrEmpty(this.Packet.version);
					                         	}
					                         	if(canLoad)
					                         	{
					                         		canLoad = !String.IsNullOrEmpty(this.Packet.arch);
					                         	}
					                         	if(canLoad)
					                         	{
					                         		canLoad = !String.IsNullOrEmpty(this.Packet.os);
					                         	}
					                         	if(canLoad)
					                         	{
					                         		canLoad = this.Packet.data != null && this.Packet.data.Length > 0;
					                         	}
					                         	return canLoad;
					                         });
				}
				return _load as ICommand;
			}
		}
		
		RelayCommand _addImage;
		public ICommand AddImageCommand
		{
			get{
				if(_addImage == null){
					_addImage = new RelayCommand(xx => {
					                             	
					                             	this.AddImage();
					                             }, xx => {
					                             	return this.CurrentOS != null && this.CurrentArch != null;
					                             });
				}
				return _addImage as ICommand;
			}
		}
		RelayCommand _removeImage;
		public ICommand RemoveImageCommand
		{
			get{
				if(_removeImage == null){
					_removeImage = new RelayCommand(xx => {
					                                	this.Images.Remove(xx as ImageString);
					                                }, xx => {
					                                	return xx != null && xx is ImageString;
					                                });
				}
				return _removeImage as ICommand;
			}
		}
		string cd = String.Empty;
		public AddPacket()
		{
			this.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				if(e.PropertyName.Equals("PacketPath"))
				{
					GetPacketData();
				}
			};
		}
		private void GetPacketData()
		{
			if(File.Exists(this.PacketPath))
			{
				this.Packet.name =Path.GetFileName(this.PacketPath);
				this.Packet.arch = this.CurrentArch.Name;
				this.Packet.os = this.CurrentOS.Name;
				this.Packet.dateCreated = DateTime.Now;
				this.Packet.dateModified = DateTime.Now;
				this.Packet.dateCreatedSpecified = true;
				this.Packet.dateModifiedSpecified = true;
				byte[] bytes =  File.ReadAllBytes(this.PacketPath);
				AddImage(this.PacketPath.SaveIconFromFilePath());
				this.Packet.data = bytes;
				FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(this.PacketPath);
				if(fvi != null){
					this.Packet.version = fvi.ProductVersion;
				}
				this.OnPropertyChanged("Load");
			}
		}
		ObservableCollection<ImageString> _images;
		public ObservableCollection<ImageString> Images
		{
			get{
				if(_images == null)
				{
					_images = new ObservableCollection<ImageString>();
				}
				return _images;
			}
		}
		private void AddImage(string image)
		{
			ImageString sw = new ImageString(image);
			this.Images.Add(sw);
		}
		int _lastFilterIndex = 1;
		private void AddImage()
		{
			string imagePath = Plugin.WPF.Extensions.OpenFileDialogImages( this._lastFilterIndex, out this._lastFilterIndex,this.cd, out this.cd);
			if(!String.IsNullOrEmpty(imagePath))
			{
				string ext = Path.GetExtension(imagePath);
				if(ext.ToLower().Equals(".exe"))
				{
					imagePath = imagePath.SaveIconFromFilePath();
				}
				this.AddImage(imagePath);
			}
			
		}
		private void LoadPacket(){
			this.Packet.images.Clear();
			foreach(ImageString imgStr in this.Images){
				if(imgStr.IsToLoad){
					imagesTypeImage img = new imagesTypeImage();
					img.name = Path.GetFileName(imgStr.Value);
					img.ignore = false;
					img.data = File.ReadAllBytes(imgStr.Value);
					this.Packet.images.Add(img);
				}
			}
			this.RestApi.AddPacket(this.Packet);
			this.Mediator.NotifyColleagues("OSes.Reset", "none");
			this.Settings.Save();
		}

	}
}
