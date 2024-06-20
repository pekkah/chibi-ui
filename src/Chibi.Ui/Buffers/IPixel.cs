using System;

namespace Chibi.Ui.Buffers;

public interface IPixel
{
}

public interface IPixel<TSelf> : IPixel, IEquatable<TSelf>
    where TSelf : unmanaged, IPixel<TSelf>
{
}