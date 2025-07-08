using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SpaceMoth
{
	// Note: This code is based off of the code in SM_PawnTable_MechsWorkSlim within Rimworld itself
	public class SM_PawnTable_MechsWorkSlim : PawnTable
	{
		protected override IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
		{
			return (IEnumerable<Pawn>)input.OrderBy<Pawn, string>(p => p.KindLabel).ThenBy<Pawn, string>((Func<Pawn, string>)(p => p.Label));
		}

		public SM_PawnTable_MechsWorkSlim(
		  PawnTableDef def,
		  Func<IEnumerable<Pawn>> pawnsGetter,
		  int uiWidth,
		  int uiHeight)
		  : base(def, pawnsGetter, uiWidth, uiHeight)
		{
		}
	}
}
