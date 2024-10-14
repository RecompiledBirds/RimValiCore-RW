using System.Runtime.CompilerServices;

namespace RVCRestructured;
public static class ArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RandomElement<T>(this T[] array) => array.AsSpan().RandomElement();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RandomElement<T>(this Span<T> span) => span[Rand.RangeInclusive(0, span.Length)];
}
