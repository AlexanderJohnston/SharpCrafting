using PostSharp.Extensibility ;
using PostSharp.Patterns.Diagnostics ;

[assembly: Log(AttributePriority = 1,
    AttributeTargetElements = MulticastTargets.Method,
    AttributeTargetMemberAttributes =
        MulticastAttributes.Public | MulticastAttributes.Private | MulticastAttributes.Internal | MulticastAttributes.Protected)]