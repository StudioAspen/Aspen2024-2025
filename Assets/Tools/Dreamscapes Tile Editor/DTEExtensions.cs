using UnityEngine;

namespace Dreamscapes.TileEditor
{
	///-//////////////////////////////////////////////////////////////////
	///
	public static class DTEExtensions
	{
		///-//////////////////////////////////////////////////////////
		///
		public static float Round(this float f)
		{
			return Mathf.Round(f);
		}

		///-//////////////////////////////////////////////////////////
		///
		/// Rounds to snap to varying grid sizes
		///
		public static float Round(this float f, float size)
		{
			return (f / size).Round() * size;
		}
		
		///-//////////////////////////////////////////////////////////
		///
		/// Round snapping but with an offset
		///
		public static float Round(this float f, float size, float offset)
		{
			return ((f + offset) / size).Round() * size - offset;
		}

		///-//////////////////////////////////////////////////////////////////
		///
		public static void SetLayerInAllChildren(this GameObject g, int layer)
		{
			// Set layer on self
			g.layer = layer;

			foreach (Transform child in g.transform)
			{
				child.gameObject.SetLayerInAllChildren(layer);
			}
		}
	}

}