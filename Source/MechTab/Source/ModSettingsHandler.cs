using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SM_MechTab
{
	[StaticConstructorOnStartup]
	class ModSettingsHandler : Mod
	{
		//public static MechTabModSettings m_Settings;

		public ModSettingsHandler(ModContentPack content) : base(content)
		{
			base.GetSettings<MechTabModSettings>();
		}

		public void Save()
		{
			try
			{
				LoadedModManager.GetMod<ModSettingsHandler>().GetSettings<MechTabModSettings>().Write();
			}
			catch (Exception)
			{

			}
		}

		public override string SettingsCategory()
		{
			string CatString = "Mech Tab";

			try
			{
				CatString = "SM_MechWork.OptionCat".Translate();
			}
			catch (Exception)
			{
				CatString = "Mech Tab";
			}

			return CatString;
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);
			MechTabModSettings.DoWindowContents(inRect);
		}
	}
}
