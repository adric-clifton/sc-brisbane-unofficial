using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwordcraftBrisbane.Data
{
    public abstract class HtmlTag : IDisposable
    {
        private readonly StringBuilder writer;

        protected abstract string TagName { get; }

        public HtmlTag(StringBuilder builder, string parameters = "")
        {
            writer = builder;
            writer.Append($"<{TagName} {parameters}>");
        }

        public void Dispose()
        {
            writer.Append($"</{TagName}>");
        }
    }

    public class Div : HtmlTag
    {
        protected override string TagName => "div";

        public Div(StringBuilder builder, string parameters = "") : base(builder, parameters)
        { }
    }

    public class Span : HtmlTag
    {
        protected override string TagName => "span";

        public Span(StringBuilder builder, string parameters = "") : base(builder, parameters)
        { }
    }

    public class Label : HtmlTag
    {
        protected override string TagName => "label";

        public Label(StringBuilder builder, string parameters = "") : base(builder, parameters)
        { }
    }

    public class TableRow : HtmlTag
    {
        protected override string TagName => "tr";

        public TableRow(StringBuilder builder) : base(builder)
        { }
    }

    public class TableDiv : HtmlTag
    {
        protected override string TagName => "td";

        public TableDiv(StringBuilder builder) : base(builder)
        { }
    }

    public class UnorderedList : HtmlTag
    {
        protected override string TagName => "ul";

        public UnorderedList(StringBuilder builder) : base(builder)
        { }
    }

    public class ListItem : HtmlTag
    {
        protected override string TagName => "li";

        public ListItem(StringBuilder builder) : base(builder)
        { }
    }
}