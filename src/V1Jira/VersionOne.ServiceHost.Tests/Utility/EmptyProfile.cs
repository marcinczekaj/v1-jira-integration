using VersionOne.Profile;

namespace VersionOne.ServiceHost.Tests
{
	internal class EmptyProfile : IProfile
	{
		public string Path { get { return null; } }
		public string Name { get { return null; } }
		public string Value { get { return null; } set { } }
		public IProfile Parent { get { return null; } }
		public IProfile this[string childpath] { get { return this; } }
	}
}