namespace Categories
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;

    public class Base
    {
        public virtual void PublishToSelf(Permutation permutation)
        {
            CheckInPermutation(permutation);
            Console.WriteLine("PublishToSelf");
        }

        public virtual void SendLocal(Permutation permutation)
        {
            CheckInPermutation(permutation);
            Console.WriteLine("SendLocal");
        }

        public virtual void SendToSelf(Permutation permutation)
        {
            CheckInPermutation(permutation);
            Console.WriteLine("SendToSelf");
        }

        void CheckInPermutation(Permutation permutation, [CallerMemberName]string memberName = "")
        {
            if (!permutation.Tests.Contains(memberName)) Assert.Inconclusive("Not in category" + memberName);
        }
    }
}