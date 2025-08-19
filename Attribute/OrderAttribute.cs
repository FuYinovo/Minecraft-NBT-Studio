using System;

namespace NBT_Studio.Attribute;

[AttributeUsage(AttributeTargets.Field)]
public class OrderAttribute(int order) : System.Attribute
{
    public int Order = order;
}