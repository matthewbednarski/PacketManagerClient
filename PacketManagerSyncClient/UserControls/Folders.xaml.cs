/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 11/01/2013
 * Time: 18:10
 * 
 */
using System;
using System.Diagnostics;
using System.Xaml;

using Plugin.WPF;

namespace PacketManagerSyncClient.UserControls
{
	/// <summary>
	/// Interaction logic for Folders.xaml
	/// </summary>
	public partial class Folders : UserControlBase
	{
		ViewModels.Folders _vm;
		public ViewModels.Folders VM
		{
			get{
				if(_vm == null)
				{
					_vm = new ViewModels.Folders();
				}
				return _vm;
			}
		}
		public Folders()
		{
			try{
				this.Name = "Folders";
				this.Activate = true;
				InitializeComponent();
				this.DataContext = this.VM;
			}
			catch (XamlParseException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
	}
}