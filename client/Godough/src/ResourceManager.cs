using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Godough.src
{
	internal class ResourceManager
	{
		public static ResourceManager Ins { get; } = new ResourceManager();

		private readonly Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

		private ResourceManager() { }

		public T Load<T>(string path, bool keep = false) where T : Resource
		{
			Resource resource;
			if (!keep || !resources.TryGetValue(path, out _))
				resource = ResourceLoader.Load<T>(path);
			else
				resource = resources[path];
			return (T)resource;
		}

	}
}
