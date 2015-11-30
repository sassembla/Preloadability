using System;
using System.Collections.Generic;

[Serializable] public class BundleList {
	public int version;
	public List<BundleData> bundles;
}

[Serializable] public class BundleData {
	public string bundleName;
	public int version;
	public uint crc;
	public long size;
	public List<string> resources;
}

[Serializable] public class PreloadList {
	public List<string> preloadBundleNames;
}