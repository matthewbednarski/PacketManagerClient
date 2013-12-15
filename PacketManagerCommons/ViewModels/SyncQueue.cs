/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 24/11/2013
 * Time: 19:35
 * 
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MVVm.Core;
using PacketManagerCommons.ViewModels;
using Plugin;

namespace PacketManagerCommons.ViewModel
{
	/// <summary>
	/// Description of SyncQueue.
	/// </summary>
	public class SyncQueue : PacketViewModelBase
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
		private static object sync = new object();
		
		SpecialQueue<Action> _queue;
		public SpecialQueue<Action> ActionQueue
		{
			get{
				if(_queue == null)
				{
					_queue = new SpecialQueue<Action>();
				}
				return _queue;
			}
		}
		
		
		public SyncQueue():base()
		{
			
		}
		
		[MediatorMessageSink("SyncQueue.Version.Enqueue")]
		void EnqueueVersion(object action)
		{
			lock(sync)
			{
				if(action is Action)
				{
					Action obj = action as Action;
					this.ActionQueue.Enqueue(obj);
					this.Mediator.NotifyColleagues("SyncQueue.Version.Invoke", "");
				}
			}
		}
		[MediatorMessageSink("SyncQueue.Version.Invoke")]
		void InvokeVersion(object action)
		{
			lock(sync)
			{
				while(this.ActionQueue.Count > 0)
				{
					this.ActionQueue.Dequeue().Invoke();
				}
			}
		}
//
//		[MediatorMessageSink("SyncQueue.Version.Dequeue")]
//		void DequeueVersion(object version)
//		{
//			lock(sync)
//			{
//				if(version is Vers)
//				{
//					Vers obj = version as Vers;
//					if(!this.WorkQueue.Contains(obj))
//					{
//						this.WorkQueue.TryAdd(
//					}
//				}
//			}
//		}
		

	}
}
