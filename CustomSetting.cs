using System;
using System.Collections.Generic;

namespace DillyzRoleApi_Rewritten
{
    public class CustomSetting {
        public CustomSettingType settingType = CustomSettingType.None;
        public string title = "Invalid";
    }

    public class CustomNumberSetting : CustomSetting
    {
        private float _value;
        private float _defaultValue;
        private float _minimumValue;
        private float _maximumValue;
        private float _increment;
        public float increment => _increment;
        public float settingValue {
            get {
                return _value;
            }
            set {
                this._value = Math.Min(Math.Max(value, _minimumValue), _maximumValue);

                if (_onChanged != null)
                    _onChanged(_value);
            }
        }
        public string suffix = "";

        private Action<float> _onChanged;

        public CustomNumberSetting(string name, float defaultValue, float min, float max, float increment, Action<float> onChanged) {
            this.title = name;
            this.settingType = CustomSettingType.Float;

            this._defaultValue = defaultValue;
            this._minimumValue = min;
            this._maximumValue = max;
            this._increment = increment;

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
        public int curIndex { get { return _curIndex; } set { this._curIndex = (value >= 0) ? ((value >= _allValues.Count - 1) ? 0 : value) : (_allValues.Count - 1); } }
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
                    DillyzRoleApiMain.Instance.Log.LogError($"New string value \"{value}\" cannot be found! (Did you check the capitalization?)");

                if (_onChanged != null)
                    _onChanged(_allValues[_curIndex]);
            }
        }

        public CustomStringSetting(string name, string defaultValue, List<string> allValues, Action<string> onChanged)
        {
            this.title = name;
            this.settingType = CustomSettingType.String;

            this._curIndex = 0;
            if (allValues.Contains(defaultValue))
                this._defaultValue = defaultValue;
            else
            {
                this._defaultValue = allValues[0];
                DillyzRoleApiMain.Instance.Log.LogError($"The default string value \"{defaultValue}\" could not be found! (Did you check the capitalization?)");
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

        public CustomBooleanSetting(string name, bool defaultValue, Action<bool> onChanged) {
            this.title = name;
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
        Float,
        String,
        Boolean
    }
}
