using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Serialization;
using PostSharp.Aspects ;

namespace SharpCrafting.Aspects
{
    [PSerializable]
    public class SingleEntryMethodAttribute : MethodInterceptionAspect
    {
        ///     Starts at zero and determines number of accesses.
        public int CallCounter;

        public bool Called = false;

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            //  Immediately increment so that a second thread cannot sneak by without touching.
            CallCounter++;

            //  Potential miss if multiple threads access simultaneously.
            if (CallCounter > 1)
            {
                if (!Called)
                    throw new Exception("Two threads simultaneously entered a single-entry method for the first time.");

                args.ReturnValue = null;
                return;
            }

            //  Proceed with the original call this time.
            Called = true;
            args.Proceed();
        }
    }
}
