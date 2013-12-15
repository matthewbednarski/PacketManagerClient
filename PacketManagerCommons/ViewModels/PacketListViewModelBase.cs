/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 24/11/2013
 * Time: 10:48
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace PacketManagerCommons.ViewModels
{
	/// <summary>
	/// Description of PacketListViewModelBase.
	/// </summary>
	public abstract class PacketListViewModelBase:PacketViewModelBase
	{
		public PacketListViewModelBase()
		{
		}
		private string _filter = string.Empty;
		public string Filter
		{
			get{
				return _filter;
			}
			set{
				string set_with = value;
				if(set_with == null)
				{
					set_with = string.Empty;
				}
				if(!_filter.Equals(set_with))
				{
					_filter = set_with;
					this.OnPropertyChanged("Filter");
				}
				
			}
		}
		public abstract ObservableCollection<PacketListViewModelBase> List
		{
			get;
		}
		Regex regex = null;
		public bool IsMatch(string text, bool useRegexp = false)
		{
			if(String.IsNullOrEmpty(this.Filter))
			{
				return true;
			}
			if(text == null)
			{
				text = string.Empty;
			}
			if(useRegexp)
			{
				regex = new Regex(this.Filter, RegexOptions.ECMAScript);
				return regex.IsMatch(text);
			}else{
				return text.Equals(this.Filter, StringComparison.OrdinalIgnoreCase);
			}
		}
		public void FilterList( bool useRegexp = false)
		{
			if(this.Filter == null)
			{
				this._filter = string.Empty;
			}
			if(useRegexp)
			{
				regex = new Regex(this.Filter , RegexOptions.ECMAScript);
				foreach(var p in this.List)
				{
					if(!regex.IsMatch(p.Name))
					{
						this.List.Remove(p);
					}
				}
			}else{
				foreach(var p in this.List)
				{
					if(!this.Filter .Equals(p.Name, StringComparison.OrdinalIgnoreCase))
					{
						this.List.Remove(p);
					}
				}
			}
		}
	}
}
