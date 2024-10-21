using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscapes.TileEditor
{
	///-/////////////////////////////////////////////////////////////////////////
	///
	[CreateAssetMenu(fileName="BiomeManager", menuName="Dreamscapes/BiomeManager")]
	public class SODTEBiomeManager : ScriptableObject
	{
		public Vector2 tileSize;
		public SODTEBiome[] biomes;
	}

}