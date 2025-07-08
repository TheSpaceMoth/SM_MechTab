using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Note: This code is based off of the code in WidgetsWork within Rimworld itself
	[StaticConstructorOnStartup]
	public static class WidgetMechWork
	{
		public static void DrawMechWorkBoxFor(
		  float x,
		  float y,
		  Pawn p,
		  WorkTypeDef wType,
		  bool incapableBecauseOfCapacities)
		{
			if (p.WorkTypeIsDisabled(wType))
			{
				// Return if disabled, dont draw.
				return;
			}
			else
			{
				Rect rect = new Rect(x, y, 25f, 25f);
				if (incapableBecauseOfCapacities)
					GUI.color = new Color(1f, 0.3f, 0.3f);
				WidgetMechWork.DrawMechWorkBoxBackground(rect, p, wType);
				GUI.color = Color.white;
				if (Find.PlaySettings.useWorkPriorities)
				{
					int priority1 = p.workSettings.GetPriority(wType);
					if (priority1 > 0)
					{
						Verse.Text.Anchor = TextAnchor.MiddleCenter;
						GUI.color = WidgetsWork.ColorOfPriority(priority1);
						Widgets.Label(rect.ContractedBy(-3f), priority1.ToStringCached());
						GUI.color = Color.white;
						Verse.Text.Anchor = TextAnchor.UpperLeft;
					}
					if (Event.current.type != EventType.MouseDown || !Mouse.IsOver(rect))
						return;
					int num = p.workSettings.WorkIsActive(wType) ? 1 : 0;
					if (Event.current.button == 0)
					{
						int priority2 = p.workSettings.GetPriority(wType) - 1;
						if (priority2 < 0)
							priority2 = 4;
						p.workSettings.SetPriority(wType, priority2);
						SoundDefOf.DragSlider.PlayOneShotOnCamera();
					}
					if (Event.current.button == 1)
					{
						int priority3 = p.workSettings.GetPriority(wType) + 1;
						if (priority3 > 4)
							priority3 = 0;
						p.workSettings.SetPriority(wType, priority3);
						SoundDefOf.DragSlider.PlayOneShotOnCamera();
					}
					if (num == 0 && p.workSettings.WorkIsActive(wType) && wType.relevantSkills.Any<SkillDef>() && (double)p.RaceProps.mechFixedSkillLevel <= 2.0)
						SoundDefOf.Crunch.PlayOneShotOnCamera();

					Event.current.Use();
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ManualWorkPriorities, KnowledgeAmount.SmallInteraction);
				}
				else
				{
					if (p.workSettings.GetPriority(wType) > 0)
						GUI.DrawTexture(rect, (Texture)WidgetsWork.WorkBoxCheckTex);
					if (!Widgets.ButtonInvisible(rect))
						return;
					if (p.workSettings.GetPriority(wType) > 0)
					{
						p.workSettings.SetPriority(wType, 0);
						SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
					}
					else
					{
						p.workSettings.SetPriority(wType, 3);
						SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
						if (wType.relevantSkills.Any<SkillDef>() && (double)p.RaceProps.mechFixedSkillLevel <= 2.0)
							SoundDefOf.Crunch.PlayOneShotOnCamera();
					}
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
				}
			}
		}

		public static string TipForPawnWorker(
		  Pawn p,
		  WorkTypeDef wDef,
		  bool incapableBecauseOfCapacities)
		{
			if((p.RaceProps.mechEnabledWorkTypes.NullOrEmpty<WorkTypeDef>() == true) || (p.RaceProps.mechEnabledWorkTypes.Contains(wDef) == false))
				return (string)"CannotDoThisWork".Translate((NamedArgument)p.LabelShort, (NamedArgument)(Thing)p);

			StringBuilder stringBuilder = new StringBuilder();
			string str1 = wDef.gerundLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor);
			int priority = p.workSettings.GetPriority(wDef);
			string str2 = str1 + ": " + ((string)("Priority" + (object)priority).Translate()).Colorize(WidgetsWork.ColorOfPriority(priority));
			stringBuilder.AppendLine(str2);
			if (p.WorkTypeIsDisabled(wDef))
			{
				string str3 = (string)"CannotDoThisWork".Translate((NamedArgument)p.LabelShort, (NamedArgument)(Thing)p);
				List<string> disabledWorkType = p.GetReasonsForDisabledWorkType(wDef);
				if (!disabledWorkType.NullOrEmpty<string>())
				{
					string s = "\n\n" + string.Join(". ", (IEnumerable<string>)disabledWorkType);
					if (disabledWorkType.Count == 1)
						s += ".";
					str3 += s.Colorize(ColorLibrary.RedReadable);
				}
				stringBuilder.Append(str3);
			}
			else
			{
				float num = p.RaceProps.mechFixedSkillLevel;
				if (wDef.relevantSkills.Any<SkillDef>())
				{
					string str4 = "";
					foreach (SkillDef relevantSkill in wDef.relevantSkills)
						str4 = str4 + relevantSkill.skillLabel.CapitalizeFirst() + ", ";
					string str5 = str4.Substring(0, str4.Length - 2);
					stringBuilder.AppendLine((string)"RelevantSkills".Translate((NamedArgument)str5, (NamedArgument)num.ToString("0.#"), (NamedArgument)20));
				}
				if (wDef.relevantSkills.Any<SkillDef>() && (double)num <= 2.0 && p.workSettings.WorkIsActive(wDef))
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(((string)"SelectedWorkTypeWithVeryBadSkill".Translate()).Colorize(WidgetsWork.ColorOfPriority(2)));
				}
				stringBuilder.AppendLine();
				stringBuilder.Append(wDef.description);
				if (incapableBecauseOfCapacities)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					stringBuilder.Append((string)"IncapableOfWorkTypeBecauseOfCapacities".Translate());
				}
			}
			return stringBuilder.ToString();
		}

		private static void DrawMechWorkBoxBackground(Rect rect, Pawn p, WorkTypeDef workDef)
		{
			// If no work types just skip
			if (p.RaceProps.mechEnabledWorkTypes.NullOrEmpty<WorkTypeDef>() == true)
				return;

			float num = p.RaceProps.mechFixedSkillLevel;

			Texture2D image1;
			Texture2D image2;
			float a;
			if ((double)num < 4.0)
			{
				image1 = WidgetsWork.WorkBoxBGTex_Awful;
				image2 = WidgetsWork.WorkBoxBGTex_Bad;
				a = num / 4f;
			}
			else if ((double)num <= 14.0)
			{
				image1 = WidgetsWork.WorkBoxBGTex_Bad;
				image2 = WidgetsWork.WorkBoxBGTex_Mid;
				a = (float)(((double)num - 4.0) / 10.0);
			}
			else
			{
				image1 = WidgetsWork.WorkBoxBGTex_Mid;
				image2 = WidgetsWork.WorkBoxBGTex_Excellent;
				a = (float)(((double)num - 14.0) / 6.0);
			}
			GUI.DrawTexture(rect, (Texture)image1);
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
			GUI.DrawTexture(rect, (Texture)image2);
			
			if (workDef.relevantSkills.Any<SkillDef>() && (double)num <= 2.0 && p.workSettings.WorkIsActive(workDef))
			{
				GUI.color = Color.white;
				GUI.DrawTexture(rect.ContractedBy(-2f), (Texture)WidgetsWork.WorkBoxOverlay_Warning);
			}

			GUI.color = Color.white;
		}
	}
}
