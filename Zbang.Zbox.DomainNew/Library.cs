using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Domain
{
    public class Library
    {

        protected Library()
        {
            Children = new List<Library>();
            Boxes = new HashSet<Box>();
            AmountOfNodes = 0;
        }
        public Library(Guid id, string name, Library parent, University university)
            : this()
        {
            Id = id;
            Name = name;
            Parent = parent;
            University = university;
            GenerateUrl();
        }

        public virtual Guid Id { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual int AmountOfNodes { get; set; }
        public virtual int NoOfBoxes { get; set; }
        public virtual Library Parent { get; protected set; }

        public virtual University University { get; protected set; }

        public virtual ICollection<Library> Children { get; protected set; }

        public virtual ICollection<Box> Boxes { get; protected set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string Url { get; protected set; }




        public virtual void GenerateUrl()
        {
            Url = UrlConsts.BuildLibraryUrl(Id, Name);
        }
        public Library CreateSubLibrary(Guid id, string nodeName)
        {
            if (nodeName == null)
            {
                throw new ArgumentNullException("nodeName");
            }
            nodeName = nodeName.Trim();
            if (CheckIfBoxesExists())
            {
                throw new ArgumentException("Cannot add library to box node");
            }
            if (Children.Any(a => a.Name == nodeName))
            {
                throw new ArgumentException("cannot have node with the same name");
            }
            var libraryNode = new Library(id, nodeName, this, University);

            Children.Add(libraryNode);
            AmountOfNodes = Children.Count;
            return libraryNode;
        }

        public void ChangeName(string newName)
        {
            if (newName == null)
            {
                throw new ArgumentNullException("newName");
            }
            if (newName.Contains('.'))
            {
                throw new ArgumentException(@"name cannot contain dot", "newName");
            }
            newName = newName.Trim();

            if (Parent == null && University.Libraries.Any(a => a.Name == newName))
            {
                throw new ArgumentException("cannot have the same name as siblings");
            }
            if (Parent != null && Parent.Children.Any(a => a.Name == newName))
            {
                throw new ArgumentException("cannot have the same name as siblings");
            }

            Name = newName;
            GenerateUrl();
        }

        public bool CheckIfBoxesExists()
        {
            return Boxes.Count(b => !b.IsDeleted) != 0;
        }



    }
}
