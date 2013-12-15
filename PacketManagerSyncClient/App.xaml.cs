using System;
using System.Diagnostics;
using System.Windows;
using System.Data;
using System.Xaml;
using System.Xml;
using System.Configuration;
using PacketManagerCommons.ViewModel;
using StructureMap;

namespace PacketManagerSyncClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		Plugin.WPF.Example.MainWindow mw;
		SyncQueue syncer; 
		void Application_Startup(object sender, StartupEventArgs e)
		{
			try{
				syncer = ObjectFactory.GetInstance<SyncQueue>();
				
				mw = new Plugin.WPF.Example.MainWindow();
				mw.ShowDialog();
			}catch(XamlParseException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		public App()
		{
			try{
				this.InitializeComponent();
			}catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
	}
}