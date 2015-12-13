using UnityEngine;
using UnityEditor;

namespace Preloadability {
	public class Menu {
		[MenuItem("Preloadability/Buid AssetBundles")]
		public static void Open () {
			BuildPipeline.BuildAssetBundles("Assets/BundledResources/Editor/Bundles", BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		}
	}
}