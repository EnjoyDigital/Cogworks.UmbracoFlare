using System.Linq;
using Cogworks.UmbracoFlare.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
namespace Cogworks.UmbracoFlare.Core.Controllers
{
    [Tree("settings", "fileSystemTree")]
    [PluginController("FileSystemPicker")]
    public class FolderSystemTreeController : TreeController
    {
        private readonly IHostingEnvironment hostingEnvionment;
        private readonly IUmbracoFlareFileService fileService;
        private readonly IMenuItemCollectionFactory menuItemCollectionFactory;

        public FolderSystemTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator, 
            IHostingEnvironment hostingEnvionment, IUmbracoFlareFileService fileService, IMenuItemCollectionFactory menuItemCollectionFactory) : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            this.hostingEnvionment = hostingEnvionment;
            this.fileService = fileService;
            this.menuItemCollectionFactory = menuItemCollectionFactory;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            var rootPath = id != "-1" ? $"/{id}" : string.Empty;

            var tempTree = AddFolders(rootPath, queryStrings);
            tempTree.AddRange(AddFiles(rootPath, queryStrings));

            return tempTree;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            var menu = menuItemCollectionFactory.Create();

            if (id == Constants.System.Root.ToInvariantString())
            {
                // root actions, perhaps users can create new items in this tree, or perhaps it's not a content tree, it might be a read only tree, or each node item might represent something entirely different...
                // add your menu item actions or custom ActionMenuItems
                menu.Items.Add(new CreateChildEntity(LocalizedTextService));
                // add refresh menu item (note no dialog)
                menu.Items.Add(new RefreshNode(LocalizedTextService, true));
            }
            else
            {
                // add a delete action to each individual item
                menu.Items.Add<ActionDelete>(LocalizedTextService, true, opensDialog: true);
            }

            return menu;
        }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var node = base.CreateTreeNode("-1", null, null, "fileSystemTree");

            node.Icon = "icon-hearts";
            node.HasChildren = false;
            node.MenuUrl = null;
            node.Path = null;
            node.CssClasses.Add("hidden");
            
            return node;
        }

        private TreeNodeCollection AddFolders(string parent, FormCollection queryStrings)
        {
            var treeNodeCollection = new TreeNodeCollection();
            var rootFolderPath = hostingEnvionment.MapPathWebRoot("~");
            var folders = fileService.GetFolders(parent);

            foreach (var folder in folders)
            {
                var folderFullName = folder.FullName.Replace(rootFolderPath, "").Replace("\\", "/");
                var folderHasFiles = folder.EnumerateDirectories().Any() || fileService.GetFiles(folderFullName).Any();
                var treeNode = CreateTreeNode(folderFullName, parent, queryStrings, folder.Name, "icon-folder", folderHasFiles);

                treeNodeCollection.Add(treeNode);
            }

            return treeNodeCollection;
        }

        private TreeNodeCollection AddFiles(string folder, FormCollection queryStrings)
        {
            var path = hostingEnvionment.MapPathWebRoot(folder);
            var rootPath = hostingEnvionment.MapPathWebRoot("~");
            var treeNodeCollection = new TreeNodeCollection();
            var files = fileService.GetFiles(folder);

            foreach (var file in files)
            {
                var nodeTitle = file.Name;
                var filePath = file.FullName.Replace(rootPath, "").Replace("\\", "/");
                var treeNode = CreateTreeNode(filePath, path, queryStrings, nodeTitle, "icon-document", false);

                treeNodeCollection.Add(treeNode);
            }

            return treeNodeCollection;
        }
    }
}