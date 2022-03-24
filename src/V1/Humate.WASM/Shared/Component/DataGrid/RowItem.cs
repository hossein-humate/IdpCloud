using System;
using System.Collections.Generic;
using System.Linq;
using Humate.WASM.Shared.Component.Icon;

namespace Humate.WASM.Shared.Component.DataGrid
{
    public class RowItem
    {
        public RowItem()
        {
            ColumnItems = new List<ColumnItem>();
        }

        public IList<ColumnItem> ColumnItems { get; set; }
        public bool ReadOnly { get; set; }
        public bool Checked { get; set; }
        public object BaseObject { get; set; }

        public ColumnItem this[string columnName] => FindByPropertyName(columnName);
        private ColumnItem FindByPropertyName(string columnName) =>
            ColumnItems.FirstOrDefault(columnItem => columnItem.ParameterName == columnName);
    }

    public class ColumnItem
    {
        public ColumnItem()
        {
            ActionButtons = new List<ActionButton>();
        }

        public string ParameterName { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public bool Invisible { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<ActionButton> ActionButtons { get; set; }
    }

    public class ActionButton
    {
        public string DisplayName { get; set; }
        public string Tooltip { get; set; }
        public SvgName? IconName { get; set; }
        public Action<RowItem> Event { get; set; }
    }
}
