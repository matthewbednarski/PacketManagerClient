/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:30
 * 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using MVVm.Core;
using Plugin;
using PacketManagerCommons.Model;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of PacketViewModelBase.
	/// </summary>
	public abstract class PacketViewModelBase: MediatorEnabledViewModelBase<object>
	{
		bool _isBusy = false;
		public bool IsBusy
		{
			get{
				return _isBusy;
			}
			set{
				if(value != _isBusy)
				{
					_isBusy = value;
					OnPropertyChanged("IsBusy");
				}
			}
			
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
		RestApi _restApi;
		public RestApi RestApi
		{
			get{
				if(_restApi == null){
					_restApi = StructureMap.ObjectFactory.GetInstance<RestApi>();
				}
				return _restApi;
			}
		}
		public abstract string Dir{get;}
		public bool DirExists
		{
			get{
				return Directory.Exists(this.Dir);
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
		RelayCommand _openDir;
		public RelayCommand OpenDir
		{
			get{
				if(_openDir == null)
				{
					_openDir = new RelayCommand( xx =>
					                            {
					                            	try{
					                            		this.Mediator.NotifyColleagues("Persist", "AfterRefresh");
					                            	}catch(Exception ex){
					                            		Debug.WriteLine(ex.Message);
					                            	} finally {
					                            		try{
					                            			Process.Start("file:///" + this.Dir);
					                            		}catch(Exception ex){
					                            			Debug.WriteLine(ex.Message);
					                            		}
					                            	}
					                            }, xx => this.DirExists);
				}
				return _openDir;
				
			}
		}
		public abstract String Name
		{
			get;
			set;
		}
		
		Dispatcher _d;
		public Dispatcher Dispatcher
		{
			get{
				if(_d == null)
				{
					_d = StructureMap.ObjectFactory.GetInstance<Dispatcher>();
				}
				return _d;
			}
		}
		public PacketViewModelBase()
		{
		}

	}
}
