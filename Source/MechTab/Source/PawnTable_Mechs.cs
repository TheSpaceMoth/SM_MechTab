using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SM_MechTab
{
	// Note: This code is based off of the code in SM_PawnTable_MechsWork within Rimworld itself
	public class SM_PawnTable_MechsWork : PawnTable
	{
		protected override IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
		{
			return (IEnumerable<Pawn>)input.OrderBy<Pawn, int>(new Func<Pawn, int>(Overseer)).ThenBy<Pawn, int>(new Func<Pawn, int>(ControlGroupIndex)).ThenBy<Pawn, string>((Func<Pawn, string>)(p => p.KindLabel)).ThenBy<Pawn, string>((Func<Pawn, string>)(p => p.Label));

			int Overseer(Pawn pawn)
			{
				Pawn overseer = pawn.GetOverseer();
				return overseer == null ? int.MaxValue : overseer.thingIDNumber;
			}

			int ControlGroupIndex(Pawn pawn)
			{
				MechanitorControlGroup mechControlGroup = pawn.GetMechControlGroup();
				return mechControlGroup == null ? int.MaxValue : mechControlGroup.Index;
			}
		}

		public SM_PawnTable_MechsWork(
		  PawnTableDef def,
		  Func<IEnumerable<Pawn>> pawnsGetter,
		  int uiWidth,
		  int uiHeight)
		  : base(def, pawnsGetter, uiWidth, uiHeight)
		{
		}
	}
}
