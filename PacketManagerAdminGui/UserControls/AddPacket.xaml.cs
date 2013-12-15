/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 15:11
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;
using Plugin.WPF;

namespace PacketManagerAdminGui.UserControls
{
	/// <summary>
	/// Interaction logic for AddPacket.xaml
	/// </summary>
	public partial class AddPacket : UserControlBase
	{
		ViewModels.AddPacket _vm;
		public ViewModels.AddPacket VM
		{
			get{
				if(_vm == null)
				{
					_vm = new ViewModels.AddPacket();
				}
				return _vm;
			}
		}
		public AddPacket()
		{
			try{
				this.Name = "Add Packet";
				
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