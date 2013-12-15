/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:31
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Threading;
using MVVm.Core;
using PacketManagerCommons.Model;
using Plugin;
using StructureMap;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of OSes.
	/// </summary>
	public class OSes:PacketListViewModelBase
	{
		static object sync = new object();
		
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
		ObservableCollection<PacketListViewModelBase> _osesList;
		public override ObservableCollection<PacketListViewModelBase> List
		{
			get{
				if(_osesList == null)
				{
					_osesList = new ObservableCollection<PacketListViewModelBase>();
					packets ps = this.RestApi.GetOSs();
					if(ps != null)
					{
						foreach(packet p in ps.packet){
							OS os = new OS();
							os.Name = p.name;
							if(this.IsMatch(os.Name)){
								_osesList.Add(os);
							}
						}
					}
				}
				return _osesList;
			}
		}
		
		public OSes():base()
		{
			this.Filter = Settings[RestApi.OS_KEY];
		}



		BackgroundWorker bw = null;
		[MediatorMessageSink("OSes.Reset")]
		private void Reset(object dummy)
		{
			if(!this.IsBusy)
			{
				this.IsBusy = true;
				bw = new BackgroundWorker();
				bw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
				{
					if(!e.Cancelled && e.Error == null)
					{
						if(e.Result is ObservableCollection<PacketListViewModelBase>)
						{
							try{
								foreach(var os in this.List)
								{
									foreach(var arch in os.List)
									{
										foreach(var app in arch.List)
										{
											app.List.Clear();
										}
										arch.List.Clear();
									}
									os.List.Clear();
								}
								this._osesList.Clear();
								this._osesList = null;
								this._osesList = e.Result as ObservableCollection<PacketListViewModelBase>;
								this.OnPropertyChanged("List");
							} catch(Exception ex)
							{
								Debug.WriteLine(ex.Message);
							}
						}
					}
					this.IsBusy = false;

				};
				bw.DoWork += delegate(object sender, DoWorkEventArgs e)
				{
//					Dispatcher.Invoke( DispatcherPriority.Background, (Action)(() => {
					try{
						this.Mediator.NotifyColleagues("Persist", "AfterRefresh");
						
						packets ps = this.RestApi.GetOSs();
						var retlist = new ObservableCollection<PacketListViewModelBase>();
						if(ps != null)
						{
							foreach(packet p in ps.packet){
								OS os = new OS();
								os.Name = p.name;
								if(this.IsMatch(os.Name)){
									retlist.Add(os);
								}
							}
						}
						e.Result = retlist;
					} catch(Exception ex)
					{
						Debug.WriteLine(ex.Message);
					}
//					                                                           }));
				};
				bw.RunWorkerAsync();
			}
		}
	}
}
