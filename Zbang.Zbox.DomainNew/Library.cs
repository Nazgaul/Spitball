using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class Library
    {
        protected Library()
        {
            Children = new List<Library>();
            Boxes = new HashSet<Box>();
            UserLibraryRelationship = new HashSet<UserLibraryRel>();
            AmountOfNodes = 0;
        }

        public Library(Guid id, string name, University university, Library parent, User user)
            : this()
        {
            Id = id;
            Name = name;
            Parent = parent;
            University = university;
            //GenerateUrl();
            CreatedUser = user;
        }

        public Library(Guid id, string name, University university, User user)
            : this(id, name, university, null, user)
        {
        }

        public Library(Guid id, string name, Library parent, University university, User user)
            : this(id, name, university, parent, user)
        {
            Settings = parent.Settings;
        }

        public virtual Guid Id { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual int AmountOfNodes { get; set; }
        public virtual int NoOfBoxes { get; set; }
        public virtual Library Parent { get; protected set; }

        public virtual University University { get; protected set; }

        public virtual User CreatedUser { get; set; }

        public virtual LibraryNodeSetting Settings { get; protected set; }

        public virtual ICollection<Library> Children { get; protected set; }

        public virtual ICollection<Box> Boxes { get; protected set; }

        public virtual ICollection<UserLibraryRel> UserLibraryRelationship { get; protected set; }

        public Library CreateSubLibrary(Guid id, string nodeName, User user)
        {
            if (nodeName == null)
            {
                throw new ArgumentNullException(nameof(nodeName));
            }
            if (this.Settings == LibraryNodeSetting.Open)
            {
                throw new ArgumentException("node is not closed");
            }
            nodeName = nodeName.Trim();
            if (CheckIfBoxesExists())
            {
                throw new BoxesInDepartmentNodeException();
            }
            if (Children.Any(a => a.Name == nodeName))
            {
                throw new DuplicateDepartmentNameException();
            }
            var libraryNode = new Library(id, nodeName, this, University, user);

            Children.Add(libraryNode);
            AmountOfNodes = Children.Count;
            return libraryNode;
        }

        public void ChangeName(string newName)
        {
            if (Name == newName)
            {
                return;
            }
            if (newName == null)
            {
                throw new ArgumentNullException(nameof(newName));
            }
            newName = newName.Trim();

            if (Parent == null && University.Libraries.Any(a => a.Name == newName))
            {
                throw new DuplicateDepartmentNameException();
            }
            if (Parent != null && Parent.Children.Any(a => a.Name == newName))
            {
                throw new DuplicateDepartmentNameException();
            }

            Name = newName;
            //GenerateUrl();
        }

        public void UpdateSettings(LibraryNodeSetting settings, User user, Guid id)
        {
            if (Settings == settings)
            {
                return;
            }
            if (Settings == LibraryNodeSetting.Closed)
            {
                throw new ArgumentException();
            }
            if (Parent != null)
            {
                throw new ArgumentException();
            }
            if (AmountOfNodes > 0 || NoOfBoxes > 0)
            {
                throw new ArgumentException();
            }
            Settings = settings;
            var rel = new UserLibraryRel(id, user, this, UserLibraryRelationType.Owner);
            UserLibraryRelationship.Add(rel);
        }

        public bool CheckIfBoxesExists()
        {
            return Boxes.Count(b => !b.IsDeleted) != 0;
        }

        /// <summary>
        /// Update courses count in box
        /// </summary>
        /// <returns>the library item to save</returns>
        public IEnumerable<Library> UpdateNumberOfBoxes()
        {
            var listOfDepartments = new List<Library>();

            var currentNode = this;
            while (currentNode != null)
            {
                currentNode.NoOfBoxes = currentNode.Boxes.Count(b => !b.IsDeleted) + currentNode.Children.Sum(s => s.NoOfBoxes);
                listOfDepartments.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            return listOfDepartments;
        }

        public IEnumerable<Library> UpdateNumberOfNodes()
        {
            var listOfDepartments = new List<Library>();

            var currentNode = this;
            while (currentNode != null)
            {
                currentNode.AmountOfNodes = currentNode.Children.Sum(s => s.AmountOfNodes);//  currentNode.Children.Sum(s => s.NoOfBoxes);
                listOfDepartments.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            return listOfDepartments;
        }
    }
}
