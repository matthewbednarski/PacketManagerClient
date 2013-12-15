/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 30/10/2013
 * Time: 21:53
 * 
 */
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using System.Xaml;
using Logger;
using PacketManagerCommons.ViewModel;
using Plugin;
using Plugin.WPF.Http.Client;
using StructureMap;
using StructureMap.Graph;
using StructureMap.TypeRules;
using PacketManagerCommons.Model;
using PacketManagerCommons.ViewModels;

namespace PacketManagerAdminGui
{
	/// <summary>
	/// Description of Program.
	/// </summary>
	public class Program
	{
		[STAThread]
		public static void Main(String[] args)
		{
			try{
				Plugin.WPF.Example.DIBootstrapper.Bootstrap();
				Bootstrap();
			}catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			SetupSettings(args);
			OSes oses = ObjectFactory.GetInstance<OSes>();
			oses.List.ToArray();

			try{
				App a = new App();
				
				a.InitializeComponent();
				try{
					ObjectFactory.Configure( xx => {
					                        	xx.For<Dispatcher>().Singleton().TheDefault.Is.IsThis(a.Dispatcher);
					                        });
				}
				catch(XamlParseException ex)
				{
					Debug.WriteLine(ex.Message);
				}
				a.Run();
			}
			catch(XamlParseException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		static void SetupSettings(string[] args)
		{
			ISettings Settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
			Settings.Load();
			if(args.Length > 0){
				if(!Settings.HasSetting(RestApi.BASE_URI_KEY)){
					Settings.Values.Add(RestApi.BASE_URI_KEY, args[0]);
				}else{
					Settings.Values[RestApi.BASE_URI_KEY] =  args[0];
				}
			}else{
				if(!Settings.HasSetting(RestApi.BASE_URI_KEY)){
					Settings.Values[RestApi.BASE_URI_KEY] =   "http://localhost:8080";
				}
			}
			
			if(args.Length > 1){
				if(!Settings.HasSetting(WebRequest.USES_AUTH)){
					Settings.Values.Add(WebRequest.USES_AUTH, args[1]);
				}else{
					Settings.Values[WebRequest.USES_AUTH] =  args[1];
				}
			}else{
				if(!Settings.HasSetting(WebRequest.USES_AUTH)){
					Settings.Values[WebRequest.USES_AUTH] =   false.ToString();
				}
			}
			if(args.Length > 2){
				if(!Settings.HasSetting(WebRequest.USER_NAME)){
					Settings.Values.Add(WebRequest.USER_NAME, args[2]);
				}else{
					Settings.Values[WebRequest.USER_NAME] =  args[2];
				}
			}else{
				if(!Settings.HasSetting(WebRequest.USER_NAME)){
					Settings.Values[WebRequest.USER_NAME] =  "";
				}
			}
			if(args.Length > 3){
				if(!Settings.HasSetting(WebRequest.PASSWORD)){
					Settings.Values.Add(WebRequest.PASSWORD, args[3]);
				}else{
					Settings.Values[WebRequest.PASSWORD] =  args[3];
				}
			}else{
				if(!Settings.HasSetting(WebRequest.PASSWORD)){
					Settings.Values[WebRequest.PASSWORD] =  "";
				}
			}
			if(args.Length > 4){
				if(!Settings.HasSetting(RestApi.OS_KEY)){
					Settings.Values.Add(RestApi.OS_KEY, args[4]);
				}else{
					Settings.Values[RestApi.OS_KEY] =  args[4];
				}
			}else{
				if(!Settings.HasSetting(RestApi.OS_KEY)){
					Settings.Values[RestApi.OS_KEY] =  "";
				}
			}
			
			if(args.Length > 5){
				if(!Settings.HasSetting(RestApi.ARCH_KEY)){
					Settings.Values.Add(RestApi.ARCH_KEY, args[5]);
				}else{
					Settings.Values[RestApi.ARCH_KEY] =  args[5];
				}
			}else{
				if(!Settings.HasSetting(RestApi.ARCH_KEY)){
					Settings.Values[RestApi.ARCH_KEY] =  "";
				}
			}
		}
		static void Bootstrap(){
			ObjectFactory.Configure( xx => {
			                        	xx.For<ILogWrapper>().Singleton().TheDefault.Is.ConstructedBy(() => new DebugConsolLogWrapper());
			                        	
			                        	xx.For<ISettings>().Singleton().Use<Settings>();
			                        	xx.For<SyncQueue>().Singleton();
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
