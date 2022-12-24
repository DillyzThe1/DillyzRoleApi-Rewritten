using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace DillyzRoleApi_Rewritten
{
    public class CustomSetting {
        public CustomSettingType settingType = CustomSettingType.None;
        public string title = "Invalid";
        public string curRole = "Jester";
    }

    public class CustomNumberSetting : CustomSetting
    {
        private int _value;
        private int _defaultValue;
        private int _minimumValue;
        private int _maximumValue;
        private int _incremete;
        public int settingValue {
            get {
                return _value;
            }
            set {
                this._value = Math.Min(Math.Max(value, _minimumValue), _maximumValue);

                if (_onChanged != null)
                    _onChanged(_value);
            }
        }

        private Action<int> _onChanged;

        public CustomNumberSetting(string name, string rolename, int defaultValue, int min, int max, int increment, Action<int> onChanged) {
            this.title = name;
            this.curRole = rolename;
            this.settingType = CustomSettingType.Integer;

            this._defaultValue = defaultValue;
            this._minimumValue = min;
            this._maximumValue = max;
            this._incremete = increment;

            this.settingValue = defaultValue;

            this._onChanged = onChanged;
        }

        public void SetToDefault() {
            this._value = _defaultValue;
        }
    }

    public class CustomStringSetting : CustomSetting
    {
        private int _curIndex;
        private string _defaultValue;
        private List<string> _allValues;

        private Action<string> _onChanged;

        public string settingValue
        {
            get
            {
                return _allValues[_curIndex];
            }
            set
            {
                if (_allValues.Contains(value))
                    this._curIndex = _allValues.IndexOf(value);
                else
                    HarmonyMain.Instance.Log.LogError($"New string value \"{value}\" cannot be found! (Did you check the capitalization?)");

                if (_onChanged != null)
                    _onChanged(_allValues[_curIndex]);
            }
        }

        public CustomStringSetting(string name, string rolename, string defaultValue, List<string> allValues, Action<string> onChanged)
        {
            this.title = name;
            this.curRole = rolename;
            this.settingType = CustomSettingType.String;

            this._curIndex = 0;
            if (allValues.Contains(defaultValue))
                this._defaultValue = defaultValue;
            else
            {
                this._defaultValue = allValues[0];
                HarmonyMain.Instance.Log.LogError($"The default string value \"{defaultValue}\" could not be found! (Did you check the capitalization?)");
            }    
            this._allValues = allValues;

            this.settingValue = defaultValue;

            this._onChanged = onChanged;
        }

        public void SetToDefault()
        {
            this._curIndex = _allValues.IndexOf(_defaultValue);
        }
    }

    public class CustomBooleanSetting : CustomSetting
    {
        private bool _value;
        private bool _defaultValue;
        public bool settingValue { 
            get
            {
                return _value;
            }
            set {
                _value = value;
                if (_onChanged != null)
                    _onChanged(_value);
            }
        }

        private Action<bool> _onChanged;

        public CustomBooleanSetting(string name, string rolename, bool defaultValue, Action<bool> onChanged) {
            this.title = name;
            this.curRole = rolename;
            this._defaultValue = defaultValue;
            this.settingValue = defaultValue;
            this.settingType = CustomSettingType.Boolean;

            this._onChanged = onChanged;
        }

        public void SetToDefault()
        {
            this.settingValue = this._defaultValue;
        }
    }

    public enum CustomSettingType { 
        None,
        Integer,
        String,
        Boolean
    }
}
