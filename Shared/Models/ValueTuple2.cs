namespace Shared.Models
{
    [RPInternalUseOnly]
    internal class ValueTuple2<T1, T2>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        
        public ValueTuple2(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = Item1;
            item2 = Item2;
        }

        public override string ToString() => $"({Item1}, {Item2})";
    }
}