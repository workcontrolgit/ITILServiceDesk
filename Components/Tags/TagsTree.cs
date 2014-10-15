//
// ServiceDesk.com
// Copyright (c) 2009
// by Michael Washington
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
//

using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;

namespace ITIL.Modules.ServiceDesk
{
    public class CatagoriesTree : HierarchicalDataSourceControl, IHierarchicalDataSource
    {
        private int _PortalID;
        private bool _RequestorCatagories;

        public CatagoriesTree(int PortalID, bool RequestorCatagories)
            : base()
        {
            _PortalID = PortalID;
            _RequestorCatagories = RequestorCatagories;
        }

        // Return a strongly typed view for the current data source control.
        private CatagoriesView view = null;

        #region GetHierarchicalView
        protected override HierarchicalDataSourceView GetHierarchicalView(string viewPath)
        {
            if (null == view)
            {
                view = new CatagoriesView(viewPath, _PortalID, _RequestorCatagories);
            }
            return view;
        }
        #endregion

        #region CreateControlCollection
        // The DataSource can be used declaratively. To enable
        // declarative use, override the default implementation of
        // CreateControlCollection to return a ControlCollection that
        // you can add to.
        protected override ControlCollection CreateControlCollection()
        {
            return new ControlCollection(this);
        }
        #endregion
    }

    #region CatagoriesView
    public class CatagoriesView : HierarchicalDataSourceView
    {
        private string _viewPath;
        private int _PortalID;
        private bool _RequestorCatagories;

        public CatagoriesView(string viewPath, int PortalID, bool RequestorCatagories)
        {
            // This implementation of HierarchicalDataSourceView does not
            // use the viewPath parameter but other implementations
            // could make use of it for retrieving values.
            _viewPath = viewPath;
            _PortalID = PortalID;
            _RequestorCatagories = RequestorCatagories;
        }

        #region Select
        // Starting with the rootNode, recursively build a list of
        // nodes, create objects, add them all to the collection,
        // and return the list.
        public override IHierarchicalEnumerable Select()
        {
            CatagoriesEnumerable CHE = new CatagoriesEnumerable();

            // Get the top level
            var results = from ServiceDeskCategories in CategoriesTable.GetCategoriesTable(_PortalID, _RequestorCatagories)
                          where ServiceDeskCategories.PortalID == _PortalID
                          where ServiceDeskCategories.Level == 1
                          orderby ServiceDeskCategories.CategoryName
                          select ServiceDeskCategories;

            // Loop thru the top level
            foreach (ITILServiceDesk_Category objITILServiceDesk_Category in results)
            {
                // Create a top level item
                ListItem objListItem = new ListItem();
                objListItem.Text = objITILServiceDesk_Category.CategoryName;
                objListItem.Value = objITILServiceDesk_Category.CategoryID.ToString();
                objListItem.Attributes.Add("PortalId", _PortalID.ToString());
                objListItem.Attributes.Add("Selectable", objITILServiceDesk_Category.Selectable.ToString());
                objListItem.Attributes.Add("RequestorVisible", objITILServiceDesk_Category.RequestorVisible.ToString());
                objListItem.Attributes.Add("RequestorCatagories", _RequestorCatagories.ToString());

                // Add a top level item to the final collection
                CHE.Add(new CatagoriesHierarchyData(objListItem));
            }

            return CHE;
        }
        #endregion

        #region CatagoriesEnumerable
        // A collection of CatagoriesHierarchyData objects
        public class CatagoriesEnumerable : ArrayList, IHierarchicalEnumerable
        {
            public CatagoriesEnumerable()
                : base()
            {
            }

            public IHierarchyData GetHierarchyData(object enumeratedItem)
            {
                return enumeratedItem as IHierarchyData;
            }
        }
        #endregion

        #region CatagoriesHierarchyData
        public class CatagoriesHierarchyData : IHierarchyData
        {
            public CatagoriesHierarchyData(ListItem obj)
            {
                objListItem = obj;
            }

            private ListItem objListItem = new ListItem();

            public override string ToString()
            {
                return objListItem.Text;
            }

            #region HasChildren
            // IHierarchyData implementation.
            public bool HasChildren
            {
                get
                {
                    // Get the children of the current item
                    AttributeCollection objAttributeCollection = objListItem.Attributes;
                    int intPortalID = Convert.ToInt32(objAttributeCollection["PortalID"]);
                    bool boolRequestorCatagories = Convert.ToBoolean(objAttributeCollection["RequestorCatagories"]);

                    var ChildResults = from ServiceDeskCategories in CategoriesTable.GetCategoriesTable(intPortalID, boolRequestorCatagories)
                                       where ServiceDeskCategories.ParentCategoryID == Convert.ToInt32(objListItem.Value)
                                       select ServiceDeskCategories;

                    return ChildResults.Count() > 0;
                }
            }
            #endregion

            #region string Path
            public string Path
            {
                get
                {
                    return objListItem.Value;
                }
            }
            #endregion

            #region object Item
            public object Item
            {
                get
                {
                    return objListItem;
                }
            }
            #endregion

            #region string Type
            public string Type
            {
                get
                {
                    return "ListItem";
                }
            }
            #endregion

            #region string Text
            public string Text
            {
                get
                {
                    return objListItem.Text;
                }
            }
            #endregion

            #region string Value
            public string Value
            {
                get
                {
                    return objListItem.Value;
                }
            }
            #endregion

            #region GetChildren
            public IHierarchicalEnumerable GetChildren()
            {
                AttributeCollection objAttributeCollection = objListItem.Attributes;
                int intPortalID = Convert.ToInt32(objAttributeCollection["PortalID"]);
                bool boolRequestorCatagories = Convert.ToBoolean(objAttributeCollection["RequestorCatagories"]);

                var ChildResults = from ServiceDeskCategories in CategoriesTable.GetCategoriesTable(intPortalID, boolRequestorCatagories)
                                   where ServiceDeskCategories.ParentCategoryID == Convert.ToInt32(objListItem.Value)
                                   orderby ServiceDeskCategories.CategoryName
                                   select ServiceDeskCategories;

                CatagoriesEnumerable children = new CatagoriesEnumerable();

                // Loop thru each item
                foreach (ITILServiceDesk_Category objCategory in ChildResults)
                {
                    // Create a new list item to add to the collection
                    ListItem objChildListItem = new ListItem();
                    // AddDots method is used to add the dots to indicate the item is a sub item
                    objChildListItem.Text = String.Format("{0}", objCategory.CategoryName);
                    objChildListItem.Value = objCategory.CategoryID.ToString();
                    objChildListItem.Attributes.Add("PortalID", intPortalID.ToString());
                    objChildListItem.Attributes.Add("Selectable", objCategory.Selectable.ToString());
                    objChildListItem.Attributes.Add("RequestorVisible", objCategory.RequestorVisible.ToString());
                    objListItem.Attributes.Add("RequestorCatagories", boolRequestorCatagories.ToString());

                    children.Add(new CatagoriesHierarchyData(objChildListItem));
                }

                return children;
            }
            #endregion

            #region GetParent
            public IHierarchyData GetParent()
            {
                CatagoriesEnumerable parentContainer = new CatagoriesEnumerable();

                AttributeCollection objAttributeCollection = objListItem.Attributes;
                int intPortalID = Convert.ToInt32(objAttributeCollection["PortalID"]);
                bool boolRequestorCatagories = Convert.ToBoolean(objAttributeCollection["RequestorCatagories"]);

                var ParentResult = (from ServiceDeskCategories in CategoriesTable.GetCategoriesTable(intPortalID, boolRequestorCatagories)
                                    where ServiceDeskCategories.ParentCategoryID == Convert.ToInt32(objListItem.Value)
                                    select ServiceDeskCategories).FirstOrDefault();

                // Create a new list item to add to the collection
                ListItem objChildListItem = new ListItem();
                // AddDots method is used to add the dots to indicate the item is a sub item
                objChildListItem.Text = String.Format("{0}", ParentResult.CategoryName);
                objChildListItem.Value = ParentResult.CategoryID.ToString();
                objChildListItem.Attributes.Add("PortalID", intPortalID.ToString());
                objChildListItem.Attributes.Add("Selectable", ParentResult.Selectable.ToString());
                objChildListItem.Attributes.Add("RequestorVisible", ParentResult.RequestorVisible.ToString());

                return new CatagoriesHierarchyData(objChildListItem);
            }
            #endregion
        }
        #endregion
    }
    #endregion
}