using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace SM_MechTab
{
	// Note: This code is based off of the code in PawnColumnWorker_WorkPriority within Rimworld itself
	public class SM_PawnColumnWorker_WorkPriority : PawnColumnWorker_WorkPriority
	{
		private const int LabelRowHeight = 50;
		private Vector2 cachedWorkLabelSize;
		
		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			if (pawn.Dead || pawn.workSettings == null || !pawn.workSettings.EverWork)
				return;
			Verse.Text.Font = GameFont.Medium;
			float x = rect.x + (float)(((double)rect.width - 25.0) / 2.0);
			float y = rect.y + 2.5f;
			bool incapable = this.IsIncapableOfWholeWorkType(pawn, this.def.workType);
			WidgetMechWork.DrawMechWorkBoxFor(x, y, pawn, this.def.workType, incapable);
			Rect rect1 = new Rect(x, y, 25f, 25f);
			if (Mouse.IsOver(rect1))
				TooltipHandler.TipRegion(rect1, (Func<string>)(() => WidgetMechWork.TipForPawnWorker(pawn, this.def.workType, incapable)), pawn.thingIDNumber ^ this.def.workType.GetHashCode());
			Verse.Text.Font = GameFont.Small;
		}

		public override void DoHeader(Rect rect, PawnTable table)
		{
			base.DoHeader(rect, table);
		}

		private bool IsIncapableOfWholeWorkType(Pawn p, WorkTypeDef work)
		{
			for (int index1 = 0; index1 < work.workGiversByPriority.Count; ++index1)
			{
				bool flag = true;
				for (int index2 = 0; index2 < work.workGiversByPriority[index1].requiredCapacities.Count; ++index2)
				{
					PawnCapacityDef requiredCapacity = work.workGiversByPriority[index1].requiredCapacities[index2];
					if (!p.health.capacities.CapableOf(requiredCapacity))
					{
						flag = false;
						break;
					}
				}
				if (flag)
					return false;
			}
			return true;
		}
		
		public override int Compare(Pawn a, Pawn b) => this.GetValueToCompare(a).CompareTo(this.GetValueToCompare(b));

		private float GetValueToCompare(Pawn pawn)
		{
			if (pawn.workSettings == null || !pawn.workSettings.EverWork)
				return -2f;
			return pawn.WorkTypeIsDisabled(this.def.workType) ? -1f : (float)pawn.RaceProps.mechFixedSkillLevel;
		}

		private Rect GetLabelRect(Rect headerRect)
		{
			Rect labelRect = new Rect(headerRect.center.x - this.cachedWorkLabelSize.x / 2f, headerRect.y, this.cachedWorkLabelSize.x, this.cachedWorkLabelSize.y);
			if (this.def.moveWorkTypeLabelDown)
				labelRect.y += 20f;
			return labelRect;
		}

		protected override string GetHeaderTip(PawnTable table)
		{
			TaggedString taggedString = (TaggedString)(this.def.workType.gerundLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + this.def.workType.description + "\n\n" + SM_PawnColumnWorker_WorkPriority.SpecificWorkListString(this.def.workType)) + "\n";
			if (this.def.sortable)
				taggedString += "\n" + "ClickToSortByThisColumn".Translate().Colorize(ColoredText.SubtleGrayColor);
			if (!SteamDeck.IsSteamDeckInNonKeyboardMode)
			{
				if (Current.Game.playSettings.useWorkPriorities)
					taggedString += "\n" + "WorkPriorityShiftClickTip".Translate().Colorize(ColoredText.SubtleGrayColor);
				else
					taggedString += "\n" + "WorkPriorityShiftClickEnableDisableTip".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			return taggedString.Resolve();
		}

		private static string SpecificWorkListString(WorkTypeDef def)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int index = 0; index < def.workGiversByPriority.Count; ++index)
			{
				if (def.workGiversByPriority[index].canBeDoneByMechs == true)
				{
					stringBuilder.Append((string)(" - " + def.workGiversByPriority[index].LabelCap));
					if (def.workGiversByPriority[index].emergency)
						stringBuilder.Append((string)(" (" + "EmergencyWorkMarker".Translate() + ")"));
					if (index < def.workGiversByPriority.Count - 1)
						stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		protected override void HeaderClicked(Rect headerRect, PawnTable table)
		{
			base.HeaderClicked(headerRect, table);
			if (!Event.current.shift)
				return;
			List<Pawn> pawnsListForReading = table.PawnsListForReading;
			for (int index = 0; index < pawnsListForReading.Count; ++index)
			{
				Pawn pawn = pawnsListForReading[index];
				if (pawn.workSettings != null && pawn.workSettings.EverWork && !pawn.WorkTypeIsDisabled(this.def.workType))
				{
					if (Current.Game.playSettings.useWorkPriorities)
					{
						int priority1 = pawn.workSettings.GetPriority(this.def.workType);
						if (Event.current.button == 0 && priority1 != 1)
						{
							int priority2 = priority1 - 1;
							if (priority2 < 0)
								priority2 = MechTabModSettings.maxPriority;
							pawn.workSettings.SetPriority(this.def.workType, priority2);
						}
						if (Event.current.button == 1 && priority1 != 0)
						{
							int priority3 = priority1 + 1;
							if (priority3 > MechTabModSettings.maxPriority)
								priority3 = 0;
							pawn.workSettings.SetPriority(this.def.workType, priority3);
						}
					}
					else if (pawn.workSettings.GetPriority(this.def.workType) > 0)
					{
						if (Event.current.button == 1)
							pawn.workSettings.SetPriority(this.def.workType, 0);
					}
					else if (Event.current.button == 0)
						pawn.workSettings.SetPriority(this.def.workType, 3);
				}
			}
			if (Current.Game.playSettings.useWorkPriorities)
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			else if (Event.current.button == 0)
			{
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			else
			{
				if (Event.current.button != 1)
					return;
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
		}
	}
}
