using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEngine;
#endif // UNITY_EDITOR

namespace Dreamscapes.TileEditor
{
	///-/////////////////////////////////////////////////////////////////////////
	///
	[CreateAssetMenu(fileName="Biome", menuName="Dreamscapes/Biome")]
	public class SODTEBiome : ScriptableObject
	{
		[SerializeField] private string biomeAssetsFolderPath;
		[SerializeField] private string tileVariationFolderPath;
		[SerializeField] private string overrideTilesFolderPath;
		
		public Material previewHighlightShader;

#if UNITY_EDITOR

		///-/////////////////////////////////////////////////////////////////////////
		///
		public GameObject[] GetBiomeAssets()
		{
			return GetPrefabsFromFolder(biomeAssetsFolderPath);
		}
		
		///-/////////////////////////////////////////////////////////////////////////
		///
		public GameObject[] GetTileVariations()
		{
			return GetPrefabsFromFolder(tileVariationFolderPath);
		}
		
		///-/////////////////////////////////////////////////////////////////////////
		///
		public GameObject[] GetOverrideTiles()
		{
			return GetPrefabsFromFolder(overrideTilesFolderPath);
		}

		///-/////////////////////////////////////////////////////////////////////////
		///
		public GameObject[] GetPrefabsFromFolder(string path)
		{
			string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { path });

			GameObject[] prefabs = new GameObject[prefabGuids.Length];
			
			for (int i = 0; i < prefabs.Length; i++)
			{
				// Get the prefab and get images for ui button
				string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
				prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
			}

			return prefabs;
		}
		
#endif //UNITY_EDITOR
		
	}
}
