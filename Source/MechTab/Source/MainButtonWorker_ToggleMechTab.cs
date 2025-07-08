using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SpaceMoth
{
	// Note: This code is based off of the code in MainButtonWorker_ToggleTab within Rimworld itself
	public class SM_MainButtonWorker_ToggleMechWorkTab : MainButtonWorker_ToggleTab
	{
		public override bool Disabled
		{
			get
			{
				if (base.Disabled)
					return true;
				Map currentMap = Find.CurrentMap;
				if (currentMap != null)
				{
					List<Pawn> pawnList1 = currentMap.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					for (int index = 0; index < pawnList1.Count; ++index)
					{
						if (pawnList1[index].RaceProps.IsMechanoid)
							return false;
					}
					List<Pawn> pawnList2 = currentMap.mapPawns.PawnsInFaction(Faction.OfPlayer);
					for (int index = 0; index < pawnList2.Count; ++index)
					{
						if (pawnList2[index].RaceProps.IsMechanoid)
							return false;
					}
				}
				return true;
			}
		}

		public override bool Visible => !this.Disabled;
	}
}
