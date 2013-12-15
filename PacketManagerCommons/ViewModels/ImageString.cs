/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 03/11/2013
 * Time: 11:35
 * 
 */
using System;
using MVVm.Core;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of ImageString.
	/// </summary>
	public class ImageString:StringWrapper
	{
		bool _isToLoad = true;
		public bool IsToLoad
		{
			get{
				return _isToLoad;
			}
			set{
				if(value != _isToLoad)
				{
					_isToLoad = value;
					this.OnPropertyChanged("IsToLoad");
				}
			}
		}
		public ImageString():base()
		{
		}
		public ImageString(string s):base(s)
		{
		}
	}
}
