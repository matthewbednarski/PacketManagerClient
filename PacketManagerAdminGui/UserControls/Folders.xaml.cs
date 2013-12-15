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
using PacketManagerCommons.ViewModels;
using Plugin.WPF;

namespace PacketManagerAdminGui.UserControls
{
	/// <summary>
	/// Interaction logic for Folders.xaml
	/// </summary>
	public partial class Folders : UserControlBase
	{
		PacketManagerCommons.ViewModels.Folders _vm;
		public PacketManagerCommons.ViewModels.Folders VM
		{
			get{
				if(_vm == null)
				{
					_vm = new PacketManagerCommons.ViewModels.Folders();
				}
				return _vm;
			}
		}
		public Folders()
		{
			try{
				this.Name = "Folders";
				
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