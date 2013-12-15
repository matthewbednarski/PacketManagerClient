/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 11/01/2013
 * Time: 08:42
 * 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Plugin;
using Plugin.WPF;
using Plugin.WPF.Http.Client;

namespace PacketManagerCommons.Model
{
	/// <summary>
	/// Description of RestApi.
	/// </summary>
	public class RestApi
	{
		public const string BASE_URI_KEY = "base.uri";
		public const string LOCAL_REPO_START_DIR_KEY = "repo.dir";
		
		
		public const String OS_KEY = "os";
		public const String ARCH_KEY = "arch";
		public const String VERS_KEY = "vers";
		public const String APP_KEY = "name";
		
		WebRequest _webRequest;
		WebRequest WebRequest
		{
			get{
				if(_webRequest == null)
				{
					_webRequest = new WebRequest();
				}
				return _webRequest;
			}
		}
		ISettings _settings;
		ISettings Settings
		{
			get{
				if(_settings == null){
					_settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
					if(String.IsNullOrEmpty(_settings[BASE_URI_KEY]))
					{
						_settings.Values.Add(BASE_URI_KEY, "http://localhost:8080");
					}
					if(string.IsNullOrEmpty(_settings[LOCAL_REPO_START_DIR_KEY]))
					{
						
						_settings.Values.Add(RestApi.LOCAL_REPO_START_DIR_KEY, Path.Combine(Path.GetTempPath(), "Packet"));
					}
				}
				return _settings;
			}
		}
		string baseRepoDir
		{
			get{
				string dir = Settings[RestApi.LOCAL_REPO_START_DIR_KEY];
				if(!Directory.Exists(dir))
				{
					try{
						Directory.CreateDirectory(dir);
					}catch(IOException ex)
					{
						
					}
				}
				return dir;
			}
		}
		string baseUri
		{
			get
			{
				return Settings[BASE_URI_KEY ];
			}
		}
		string osUri
		{
			get
			{
				return Settings[BASE_URI_KEY] + "/packet";
			}
		}
		public packets GetOSs()
		{
			if(WebRequest.Request(osUri, WebRequest.GET, "", false,  System.Threading.Timeout.Infinite, false))
			{
				string response = WebRequest.Output;
				packets p = response.DeserializeObject<packets>(packets.Namespace);
				if(p != null){
					return p;
				}
			}
			return null;
		}
		string archUri{
			get{
				return   osUri + "/{0}";
			}
		}
		public packets GetArchs(String os)
		{
			if(WebRequest.Request(String.Format(archUri, os), WebRequest.GET, "", false,  System.Threading.Timeout.Infinite, false))
			{
				string response = WebRequest.Output;
				packets p = response.DeserializeObject<packets>(packets.Namespace);
				if(p != null){
					return p;
				}
			}
			return null;
		}
		string appsUri
		{
			get{
				return osUri + "/{0}/{1}";
			}
		}
		public packets GetApp(String os, String arch)
		{
			if(WebRequest.Request(String.Format(appsUri, os, arch), WebRequest.GET, "", false,  System.Threading.Timeout.Infinite, false))
			{
				string response = WebRequest.Output;
				packets p = response.DeserializeObject<packets>(packets.Namespace);
				if(p != null){
					return p;
				}
			}
			return null;
		}
		string versionUri{
			get{
				return osUri + "/{0}/{1}/{2}";
			}
		}
		public packets GetVersions(String os, String arch, String app)
		{
			if(WebRequest.Request(String.Format(versionUri, os, arch, app) , WebRequest.GET, "", false,  System.Threading.Timeout.Infinite, false))
			{
				string response = WebRequest.Output;
				packets p = response.DeserializeObject<packets>(packets.Namespace);
				if(p != null){
					return p;
				}
			}
			return null;
		}
		string packetUri{
			get{
				return osUri + "/{0}/{1}/{2}/{3}";
			}
		}
		string packetDirBase{
			get{
				return baseRepoDir + "\\{0}\\{1}\\{2}\\{3}";
			}
		}
		public packets GetPacket(String os, String arch, String app, String version, out String path, String save_dir = "")
		{
			path = string.Empty;
			if(WebRequest.Request(String.Format(packetUri, os, arch, app, version), WebRequest.GET, "", true,  System.Threading.Timeout.Infinite, false))
			{
				string response = WebRequest.Output;
				packets p = response.DeserializeObject<packets>(packets.Namespace);
				if(p != null && p.packet != null && p.packet.First() != null){
					if(String.IsNullOrEmpty(save_dir))
					{
						save_dir = String.Format(packetDirBase, os, arch, app, version);
					}
					if(!Directory.Exists(save_dir))
					{
						try{
							Directory.CreateDirectory(save_dir);
						}catch(IOException ex)
						{
							
						}
					}
					string file = Path.Combine(save_dir, p.packet.First().name);
					if(File.Exists(file))
					{
						try{
							File.Delete(file);
						}catch(IOException ex)
						{
							Debug.WriteLine(ex.Message);
						}
					}
					String tmp_file = Path.GetTempFileName();
					try{
						using(FileStream fs = new FileStream(tmp_file, FileMode.OpenOrCreate))
						{
							fs.Write(p.packet.First().data, 0, p.packet.First().data.Length);
							path = tmp_file;
							try{
								File.Copy(tmp_file, file, true);
								path = file;
							}catch(IOException ex)
							{
								Debug.WriteLine(ex.Message);
							}
						}
					}catch(IOException ex)
					{
						Debug.WriteLine(ex.Message);
					}
					p.packet.First().data = null;
					return p;
					
				}
			}
			return null;
		}
		string addPacketUri
		{
			get{
				return Settings[BASE_URI_KEY] + "/packet/add";
			}
		}

		public message AddPacket(packet p)
		{
			packets packs = new packets();
			
			packs.packet.Add(p);
			if(WebRequest.Request(addPacketUri,  WebRequest.POST, packs.xmlObjectToString(true, "", packets.Namespace), false,  System.Threading.Timeout.Infinite, false ))
			{
				string response = WebRequest.Output;
				message m = response.DeserializeObject<message>(message.Namespace);
				if(m != null){
					return m;
				}
			}
			return null;
		}

	}
}
