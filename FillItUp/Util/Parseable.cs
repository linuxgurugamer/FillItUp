using System;


namespace FillItUp.Util
{
    public class Parseable<T> where T : IConvertible
    {
        public Parseable(T initialValue)
        {
            _value = initialValue;
            _text = _value.ToString();
        }

        public Parseable(string initialValue)
        {
            _text = initialValue;

            Parse();
        }

        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                Parse();
            }
        }

        private T _value = default(T);
        public T Value
        {
            get
            {
                return _value;
            }
        }

        public bool Success;

        private void Parse()
        {
            try
            {
                if (String.IsNullOrEmpty(_text))
                {
                    Success = false; return;
                }

                _value = (T)Convert.ChangeType(_text, typeof(T));

                Success = true;
            }
            catch { Success = false; }
        }
    }
}
