using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class Library
    {

        //private readonly string[] colorSchema = { "#62328F", "#464646", "#146EB5", "#111", "#17A099", "#83B641", "#E5A13D", "#DC552A", "#952262", "#D91C7A" };
        protected Library()
        {
            Children = new List<Library>();
            Boxes = new Iesi.Collections.Generic.HashedSet<Box>();
            AmountOfNodes = 0;
        }
        public Library(Guid id, string name, Library parent, University university)
            : this()
        {
            Id = id;
            if (name.Contains('.'))
            {
                throw new ArgumentException("name cannot contain dot", "name");
            }
            Name = name;
            Parent = parent;
            University = university;
            //Color = color;
        }

        public virtual Guid Id { get; protected set; }
        public virtual string Name { get; protected set; }
        //public virtual string Color { get; protected set; }
        public virtual int AmountOfNodes { get; set; } //TODO: do we need this
        public virtual Library Parent { get; protected set; }

        public virtual University University { get; protected set; }

        public virtual ICollection<Library> Children { get; protected set; }

        public virtual ICollection<Box> Boxes { get; protected set; }


        public Library CreateSubLibrary(Guid id, string nodeName)
        {
            nodeName = nodeName.Trim();
            Throw.OnNull(nodeName, "nodeName");
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
            if (newName.Contains('.'))
            {
                throw new ArgumentException("name cannot contain dot", "name");
            }
            newName = newName.Trim();
            Throw.OnNull(newName, "nodeName");

            if (Parent == null && University.Libraries.Any(a => a.Name == newName))
            {
                 throw new ArgumentException("cannot have the same name as siblings");
            }
            if (Parent != null && Parent.Children.Any(a => a.Name == newName))
            {
                throw new ArgumentException("cannot have the same name as siblings");
            }

            Name = newName;
        }

        private bool CheckIfBoxesExists()
        {
            return Boxes.Count != 0;
        }



    }
}
