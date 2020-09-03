using System;
using System.Collections.Generic;
using System.Linq;

namespace AjaxMVCReader
{
    /// <summary>
    /// This little project should really help with working with the key value pairs that ajax spits out on MVC pages.
    /// </summary>
    /// <seealso cref="AjaxMVCReader.AjaxFieldNode" />
    public class AjaxFieldTree : AjaxFieldNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AjaxFieldTree"/> class.
        /// </summary>
        protected AjaxFieldTree()
            :base("Root")
        {
           /*Create the root of the tree*/
        }

        /// <summary>
        /// Builds the tree from ajax array key value pairs.
        /// An example would be something like Key:"[Object1].[Object2].[Object3].FieldName" - Value:"FieldValue"
        /// </summary>
        /// <param name="pairs">The pairs.</param>
        /// <returns></returns>
        public static AjaxFieldTree BuildTreeFromAjaxArray(List<KeyValuePair<string, string>> pairs)
        {
            //new tree
            AjaxFieldTree tree = new AjaxFieldTree();

            //itearte over each pair and add to tree
            foreach(KeyValuePair<string, string> pair in pairs.Distinct())
            {
                //split key and value
                string key = pair.Key;
                string value = pair.Value;

                //check whether this key has a parent node, if not then just add to fields list
                if (!key.Contains('.'))
                {
                    tree.Fields.Add(new KeyValuePair<string, string>(key, value));
                    continue;   
                }

                //split key into parts
                string []parts = key.Split('.');

                //get the field name
                string field = parts[parts.Length - 1];

                //get the field parent name
                string fieldParent = parts[parts.Length - 2];

                //find the node that this field should be added too, this may create a branch to find it.
                AjaxFieldNode node = tree.TraverseNode(fieldParent, parts);

                //add the field and value
                node.Fields.Add(new KeyValuePair<string, string>(field, value));
            }

            return tree;
        }

        /// <summary>
        /// Traverses the node.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        protected AjaxFieldNode TraverseNode(string name, string[] parts, AjaxFieldNode parent = null)
        {
            //get the node name
            string nodeName = parts[0];

            //where are we looking for this node?
            IList<AjaxFieldNode> Nodes = parent == null ? Children : parent.Children;

            //get the node if we can by name
            AjaxFieldNode n = Nodes.SingleOrDefault(d => d.Name == nodeName);

            //if it doesnt exist then create it and add it to the node list
            if (n == null)
            {
                n = new AjaxFieldNode(nodeName);
                Nodes.Add(n);
            }

            //if this was the node we are looking for then return it 
            if (nodeName == name)
                return n;

            //otherwise keep traversing nodes
            return TraverseNode(name, parts.Skip(1).ToArray(), n);

        }
    }


    public class AjaxFieldNode
    {
        //children nodes
        public List<AjaxFieldNode> Children = new List<AjaxFieldNode>();

        //fields on this node
        public List<KeyValuePair<string, string>> Fields = new List<KeyValuePair<string,string>>();

        //name of the node
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AjaxFieldNode"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public AjaxFieldNode(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name + " (" + Children.Count + " Children, " + Fields.Count + " Fields)" + (this is AjaxFieldTree ? " [ROOT]" : String.Empty);
        }

        
    }
}