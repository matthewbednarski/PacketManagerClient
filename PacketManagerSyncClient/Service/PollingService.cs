/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 26/11/2013
 * Time: 12:12
 * 
 */
using System;
using System.Dynamic;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using Plugin.WPF.Service;

namespace PacketManagerSyncClient.Service
{
	/// <summary>
	/// Description of PollingService.
	/// </summary>
	public class PollingService:ServiceBase
	{
		Boolean _canActivate = true;
		public override Boolean CanActivate
		{
			get
			{
				return _canActivate;
			}
		}
		private Boolean _canStart = true;
		public override Boolean CanStart
		{
			get{
				return _canStart;
			}

		}
		static object sync = new object();
		DispatcherTimer _timer;
		DispatcherTimer  CheckForUpdate
		{
			get{
				if(_timer == null)
				{
					_timer = new DispatcherTimer();
					_timer.Tick += delegate(object sender, EventArgs e) 
					{
						_timer.IsEnabled = false;
						GetUpdatesSync();
						_timer.IsEnabled = true;
					};
					_timer.Interval = new TimeSpan(0, 0, 60);
				}
				return _timer;
			}
		}
		static Dispatcher _d;
		Dispatcher Dispatcher
		{
			get{
				lock(sync)
				{
					if(_d == null)
					{
						_d = StructureMap.ObjectFactory.GetInstance<Dispatcher>();
					}
					return _d;
				}
			}
		}
		

		public PollingService():base()
		{
//			ThreadStart ts = new ThreadStart(StartTimer);
//			this.Thrd = new Thread(ts);
		}
		Boolean running = false;
		private void StartTimer()
		{
			this.running = true;
			this.CheckForUpdate.IsEnabled = true;
//			while(running)
//			{
//				;
//			}
			
		}
		public override void Start()
		{
			base.Start();
			this.StartTimer();
		}
		public override void Stop()
		{
			this.running = false;
			this.CheckForUpdate.IsEnabled = false;
			base.Stop();
		}
		int i = 0;
		private void GetUpdatesSync()
		{
			try{
				Dispatcher.Invoke( DispatcherPriority.Background, (Action)(() => {
				                                                           	
				                                                           	this.Mediator.NotifyColleagues("OSes.Reset", "Refresh");
				                                                           }));
				
			}
			catch(Exception ex)
			{
				ex.Message.ToLower();
			}
			
		}
		[MVVm.Core.MediatorMessageSink("CloseWindow")]
		void Closing(string item)
		{
			this.CheckForUpdate.IsEnabled = false;
			this.Stop();
		}
	}
}
