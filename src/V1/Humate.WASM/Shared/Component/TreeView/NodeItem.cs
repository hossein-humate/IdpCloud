using Humate.WASM.Shared.Component.Icon;
using System.Collections.Generic;

namespace Humate.WASM.Shared.Component.TreeView 
{
    public class NodeItem
    {
        public string Text { get; set; }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Tag { get; set; }

        public bool ShowIcon { get; set; } = false;

        public bool ShowCheckBox { get; set; } = false;

        public SvgName Icon { get; set; } = SvgName.HashTag;

        public IList<NodeItem>  Childrens { get; set; }=new List<NodeItem>();

        public bool Checked { get; set; }

        public bool Selected { get; set; }

        public bool Expanded { get; set; }
    }
}
