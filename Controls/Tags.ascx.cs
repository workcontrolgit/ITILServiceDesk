
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITIL.Modules.ServiceDesk.Controls
{
    public partial class Tags : ITILServiceDeskModuleBase
    {
        #region Properties
        private string _DisplayType;
        public string DisplayType
        {
            get { return _DisplayType; }
            set { _DisplayType = value; }
        }

        private int _TagID;
        public int TagID
        {
            get { return _TagID; }
            set { _TagID = value; }
        }

        private bool _Expand;
        public bool Expand
        {
            get { return _Expand; }
            set { _Expand = value; }
        }

        private int?[] _SelectedCategories;
        public int?[] SelectedCategories
        {
            get { return _SelectedCategories; }
            set { _SelectedCategories = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DisplayCatagories();
            }
        }

        #region DisplayCatagories
        private void DisplayCatagories()
        {
            bool RequestorCatagories = (_DisplayType == "Administrator") ? false : true;
            CatagoriesTree colCatagories = new CatagoriesTree(PortalId, RequestorCatagories);
            tvCategories.DataSource = colCatagories;

            TreeNodeBinding RootBinding = new TreeNodeBinding();
            RootBinding.DataMember = "ListItem";
            RootBinding.TextField = "Text";
            RootBinding.ValueField = "Value";

            tvCategories.DataBindings.Add(RootBinding);

            tvCategories.DataBind();
            if (_Expand)
            {
                tvCategories.ExpandAll();
            }
        }
        #endregion

        #region tvCategories_TreeNodeDataBound
        protected void tvCategories_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
        {
            ListItem objListItem = (ListItem)e.Node.DataItem;
            e.Node.SelectAction = TreeNodeSelectAction.None;
            e.Node.ShowCheckBox = Convert.ToBoolean(objListItem.Attributes["Selectable"]);
            if (!Convert.ToBoolean(objListItem.Attributes["Selectable"]))
            {
                e.Node.ImageUrl = "../images/table.png";
                e.Node.ToolTip = e.Node.Text;
            }

            if ((!Convert.ToBoolean(objListItem.Attributes["RequestorVisible"])) & ((_DisplayType != "Administrator")))
            {
                e.Node.ImageUrl = "";
                e.Node.Text = "";
                e.Node.ShowCheckBox = false;
            }

            // Expand Node if it is in the SelectedCategories Array
            if (_SelectedCategories != null)
            {
                if (_SelectedCategories.Contains(Convert.ToInt32(e.Node.Value)))
                {
                    e.Node.ShowCheckBox = true;
                    e.Node.Checked = true;

                    // If the node has a parent then expand it
                    TreeNode TmpTreeNode = e.Node;
                    while (TmpTreeNode.Parent != null)
                    {
                        TmpTreeNode.Parent.Expand();
                        TmpTreeNode = TmpTreeNode.Parent;
                    }
                }
            }
        }
        #endregion
    }
}