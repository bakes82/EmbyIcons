using System;

namespace EmbyIcons;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DontSaveAttribute : Attribute
{
}