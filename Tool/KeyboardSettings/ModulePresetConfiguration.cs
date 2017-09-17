using System.Collections.Generic;

namespace SupportTool.Tool.KeyboardSettings
{
    public class ModulePresetConfiguration
    {
        public string Name;
        public string Primary;
        public string Secondary;
        public string Perimeter;
        public string Internal;

        public ModulePresetConfiguration(string name, string primary, string secondary, string perimeter, string internalModule)
        {
            Name = name;
            Primary = primary;
            Secondary = secondary;
            Perimeter = perimeter;
            Internal = internalModule;
        }

        public ModulePresetConfiguration(string name, Dictionary<string, string> settings)
        {
            Name = name;
            Primary = settings["TriggerAbilityOne"];
            Secondary = settings["TriggerAbilityTwo"];
            Perimeter = settings["TriggerAbilityThree"];
            Internal = settings["TriggerAbilityFour"];
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>(4)
            {
                { "TriggerAbilityOne", Primary },
                { "TriggerAbilityTwo", Secondary },
                { "TriggerAbilityThree", Perimeter },
                { "TriggerAbilityFour", Internal },
            };
        }

        public static bool operator ==(ModulePresetConfiguration presetA, ModulePresetConfiguration presetB)
        {
            if (ReferenceEquals(presetA, null))
            {
                return ReferenceEquals(presetB, null);

            }
            return presetA.Primary == presetB.Primary
                && presetA.Secondary == presetB.Secondary
                && presetA.Perimeter == presetB.Perimeter
                && presetA.Internal == presetB.Internal;
        }

        public static bool operator !=(ModulePresetConfiguration presetA, ModulePresetConfiguration presetB)
        {
            return !(presetA == presetB);
        }
    }
}
