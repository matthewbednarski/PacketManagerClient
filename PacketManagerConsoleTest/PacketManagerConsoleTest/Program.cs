/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 18:59
 * 
 */
using System;
using System.IO;
using System.Linq;
using System.Threading;
using MVVm.Core;
using PacketManagerCommons.Model;
using PacketManagerCommons.ViewModels;
using StructureMap.Graph;
using StructureMap.TypeRules;
using Plugin;

namespace PacketManagerConsoleTest
{
	class Program
	{
		public static void Main(string[] args)
		{
			Bootstrap();
			
			ISettings Settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
			Settings.Values.Add(RestApi.BASE_URI_KEY, args[0]);
//			Settings.Values.Add(WebRequest.USES_AUTH, args[1]);
//			Settings.Values.Add(WebRequest.USER_NAME, args[2]);
//			Settings.Values.Add(WebRequest.PASSWORD, args[3]);
			Settings.Values.Add(RestApi.LOCAL_REPO_START_DIR_KEY, Path.Combine(Path.GetTempPath(), "Packet"));
			
			if(args != null && args.Length > 0)
			{
				
			}
			
			OSes oses = StructureMap.ObjectFactory.GetInstance<OSes>();
			oses.List.ToArray();
			bool run = true;
			Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
			{
				run = false;
			};
			
			while(run)
			{
				
				Mediator.Instance.NotifyColleagues("OSes.Refresh", "none");
//				GetWindowsX86AdobeAll();
				
				Thread.Sleep(1500);
			}
			Console.WriteLine("Exiting...");
		}

		static void Bootstrap(){
			StructureMap.ObjectFactory.Initialize( xx => {
			                                      	xx.Scan( scan => {
			                                      	        	scan.AssembliesFromApplicationBaseDirectory();
			                                      	        	scan.With(new SingletonConvention<ISettings>());
			                                      	        });
			                                      	
			                                      	xx.For<RestApi>().Singleton();
			                                      	xx.For<OSes>().Singleton();
			                                      });
		}
		internal class SingletonConvention<T>:IRegistrationConvention
		{
			public void Process(Type type, StructureMap.Configuration.DSL.Registry registry)
			{
				
				if (!type.IsConcrete() || !type.CanBeCreated() || !type.AllInterfaces().Contains(typeof(T)))
				{
					return;
				}
				registry.For(typeof(T)).Singleton().Use(type);
			}
		}
	}
}