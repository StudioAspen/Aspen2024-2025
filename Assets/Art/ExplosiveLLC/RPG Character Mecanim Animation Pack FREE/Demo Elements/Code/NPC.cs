using UnityEngine;
using RPGCharacterAnims.Lookups;

namespace RPGCharacterAnims
{
    [RequireComponent(typeof(RPGCharacterController))]
	[RequireComponent(typeof(RPGCharacterNavigationController))]
	public class NPC : MonoBehaviour
    {
        private RPGCharacterController rpgCharacterController;
        private RPGCharacterNavigationController rpgNavigationController;
        private Vector3 targetPosition;
		public float followDistance = 3f;

		void Awake()
		{
            rpgCharacterController = GetComponent<RPGCharacterController>();
            rpgNavigationController = GetComponent<RPGCharacterNavigationController>();
		}

		private void Update()
		{
			targetPosition = rpgCharacterController.target.transform.position;
			if (IsOutOfRange(transform.position, targetPosition))
			{ rpgCharacterController.StartAction(HandlerTypes.Navigation, RandomOffset(targetPosition)); }
		}

		private Vector3 RandomOffset(Vector3 position)
		{ return new Vector3(position.x - Random.Range(1, 2), position.y, position.z - Random.Range(1, 2)); }

		private bool IsOutOfRange(Vector3 npc, Vector3 player)
		{
			if (Vector3.Distance(npc, player) > followDistance) { return true; }
			else { return false; }
		}
	}
}