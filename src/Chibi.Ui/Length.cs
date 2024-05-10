using System;

namespace Chibi.Ui
{
    public readonly struct Length
    {
        private readonly int _value;
        public bool IsAuto { get; }

        public static readonly Length Auto = new(true);

        private Length(bool auto)
        {
            IsAuto = auto;
            _value = 0;
        }

        public Length(int value) : this(false)
        {
            _value = value;
        }

        public static implicit operator int(Length length)
        {
            if (length.IsAuto)
                throw new InvalidCastException(
                    "Length with value of Auto cannot ben cast to int");

            return length._value;
        }

        public static Length operator +(Length left, Length right)
        {
            if (left.IsAuto && right.IsAuto)
                return Auto;

            if (left.IsAuto != right.IsAuto)
            {
                if (left.IsAuto)
                    return right;

                return left;
            }

            return new Length(left._value + right._value);
        }

        public static Length operator -(Length left, Length right)
        {
            if (left.IsAuto && right.IsAuto)
                return Auto;

            if (left.IsAuto != right.IsAuto) return Auto;

            return new Length(left._value - right._value);
        }

        public static implicit operator Length(int value)
        {
            return new Length(value);
        }

        public override string ToString()
        {
            if (IsAuto)
                return "Auto";

            return _value.ToString();
        }
    }
}