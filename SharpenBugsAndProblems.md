While working with default Sharpen source and default Sharpen configuration from the https://github.com/slluis/sharpen I faced with some problems, some of which was fixed in Sharpen code - other with manual editing of conversion result.
- can't control via options data annotaitons conversion
- wrong conversion of some instance fields
- far from ideal support for generics conversion
- can't reorder inheritance of class and interface
- Iterator always replaced by IEnumerator (in our case we need to keep Iterator) and this can't be changed without writing your own config class
- wrong conversion of this code.
Source: 
```
for (Iterator an = aliasNode.iterateChildren(), bn = baseNode.iterateChildren();an.hasNext() && bn.hasNext();)
{
    XMPNode aliasChild = (XMPNode) an.next();
    XMPNode baseChild =  (XMPNode) bn.next();
    compareAliasedSubtrees (aliasChild, baseChild, false);
}
```

Result:
```
for (Iterator<XMPNode> an = aliasNode.IterateChildren(); an.HasNext() && bn.HasNext(); )
{
    XMPNode aliasChild = (XMPNode)an.Next();
    XMPNode baseChild = (XMPNode)bn.Next();
    CompareAliasedSubtrees(aliasChild, baseChild, false);
}
```

Here Sharpen lost initialization of bn variable.

- string.format conversions not supported
- value types in C# can't be null and can't have references
- some times int used where we need long, short where we need int, ... 
- junit methods with messages has another params order than NUnit. For example: assertFalse, assertTrue .. and params order can't be changed while conversion.
- lost method calls while some conversions. Intvalue(), DoubleValue() ... - could be solved using custom configuration class.
- java byte is signed. c# byte unsigned. But by default byte replaced by byte.
- loses file header comments (for example with license text)