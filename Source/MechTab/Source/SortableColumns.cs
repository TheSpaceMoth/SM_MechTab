using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpaceMoth
{
	public class SM_PawnColumnWorker_Overseer_Sortable : PawnColumnWorker_Overseer
	{
		public override int Compare(Pawn a, Pawn b) => string.Compare(this.GetValueToCompare(a), this.GetValueToCompare(b), StringComparison.Ordinal);

		private string GetValueToCompare(Pawn pawn) => (string)GetLabel(pawn.GetOverseer());

		private TaggedString GetLabel(Pawn pawn) => pawn.RaceProps.Humanlike || pawn.RaceProps.Animal || pawn.Name == null || pawn.Name.Numerical ? (!this.def.useLabelShort ? (TaggedString)pawn.LabelNoCount.CapitalizeFirst() : (TaggedString)pawn.LabelShortCap) : (TaggedString)(pawn.Name.ToStringShort.CapitalizeFirst() + ", " + pawn.KindLabel.Colorize(ColoredText.SubtleGrayColor));
	}

	public class SM_PawnColumnWorker_WorkMode_Sortable : PawnColumnWorker_WorkMode
	{
		public override int Compare(Pawn a, Pawn b) => string.Compare(this.GetValueToCompare(a), this.GetValueToCompare(b), StringComparison.Ordinal);

		private string GetValueToCompare(Pawn pawn) => (string)GetGroupLabel(pawn);

		private string GetGroupLabel(Pawn pawn) => pawn.GetMechWorkMode().label;
	}

	public class SM_PawnColumnWorker_ControlGroup_Sortable : PawnColumnWorker_ControlGroup
	{
		public override int Compare(Pawn a, Pawn b) => string.Compare(this.GetValueToCompare(a), this.GetValueToCompare(b), StringComparison.Ordinal);

		private string GetValueToCompare(Pawn pawn) => (string)GetGroupLabel(pawn);

		private string GetGroupLabel(Pawn pawn) => pawn.GetMechControlGroup().Index.ToString();
	}

	public class SM_PawnColumnWorker_MechLabelWithIcon_Sortable : PawnColumnWorker_Label
	{
		public override int Compare(Pawn a, Pawn b) => string.Compare(this.GetValueToCompare(a), this.GetValueToCompare(b), StringComparison.Ordinal);

		private string GetValueToCompare(Pawn pawn) => (string)GetLabel(pawn);

		private TaggedString GetLabel(Pawn pawn) => pawn.RaceProps.Humanlike || pawn.RaceProps.Animal || pawn.Name == null || pawn.Name.Numerical ? (!this.def.useLabelShort ? (TaggedString)pawn.LabelNoCount.CapitalizeFirst() : (TaggedString)pawn.LabelShortCap) : (TaggedString)(pawn.Name.ToStringShort.CapitalizeFirst() + ", " + pawn.KindLabel.Colorize(ColoredText.SubtleGrayColor));
		//private string GetGroupLabel(Pawn pawn) => pawn.GetMechControlGroup().Index.ToString();
	}
}
