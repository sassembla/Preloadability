using System;
using System.Collections.Generic;

[Serializable] public class AssetList {
	public int version;
	public List<AssetData> bundles;
}

[Serializable] public class AssetData {
	public string bundleName;
	public int version;
	public uint crc;
	public long size;
	public List<string> resources;
}

[Serializable] public class PreloadList {
	public string version;
	public List<string> preloadBundleNames;
}