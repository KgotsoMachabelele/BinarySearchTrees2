using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySearchTrees
{
    class BST
    {
        #region Private embedded class for individual nodes in the list
        private class Node
        {
            internal dynamic Value { get; set; }            
            internal Node Parent { get; set; } //Recursive definition - Only needed for removal of nodes.
            internal Node Left { get; set; }   //Recursive definition
            internal Node Right { get; set; }  //Recursive definition

            //Constructor
            public Node(dynamic value)
            {
                this.Value = value;
                Left = Right = null;
                Parent = null;
            }

            // Works, but the tree is read from left to right (not from top to bottom)
            #region Visualise tree

            internal int nSpaces { get; set; }
            
            //public string DisplayNode()
            //{
            //    StringBuilder output = new StringBuilder();
            //    DisplayNode(output, 0);
            //    return output.ToString();
            //}

            private void DisplayNode(StringBuilder output, int depth)
            {
                if (this.Right != null)
                    this.Right.DisplayNode(output, depth + 1);

                output.Append('\t', depth);
                output.AppendLine(Value.ToString());

                if (Left != null)
                    Left.DisplayNode(output, depth + 1);
            }
            
            public override String ToString()
            {
                return this.Value + "{" + (Left == null ? "" : Left.ToString()) + ";" + (Right == null ? "" : Right.ToString()) + "}";
            }

            #endregion Visualise tree

        } //embedded class Node
        #endregion Embedded class

        #region private data members of the tree
        private Node Root;
        #endregion
        
        #region Constructor
        public BST()
        {
            this.Root = null;
        } //Constructor
        #endregion

        #region Add
    
        public void Add(dynamic value)
        {
            this.Root = Add(value, null, Root); //value, parent, node
        } //Insert

        private Node Add(dynamic value, Node parent, Node child)
        {
            if (child == null) //Occurs when we reach the bottom of the tree
            {
                child = new Node(value); //The pointer, which was previously null, now becomes something. Thus, parent.Left/Right becomes something.
                child.Parent = parent; //Only needed for removal
            }
            else //Still not at the bottom - decide which way to go 
            {
                int compareTo = value.CompareTo(child.Value);
                if (compareTo < 0)
                    child.Left = Add(value, child, child.Left); //value, parent, node
                else
                    if (compareTo > 0)
                        child.Right = Add(value, child, child.Right); //value, parent, node
                    else //values are equal - don't insert
                    {
                        int dummy = 0; //Dummy to enable breakpoint
                    }
            }
            return child;
        } //Add

        #endregion Add

        #region Traversing

        public List<dynamic> VisitTreeInOrder() 
        {
            List<dynamic> lstNodes = new List<dynamic>();
            VisitTreeInOrder(this.Root, lstNodes);
            return lstNodes;
        } //PrintTreeDFS

        private void VisitTreeInOrder(Node node, List<dynamic> lstNodes) 
        {
            if (node != null)
            {
                VisitTreeInOrder(node.Left, lstNodes);
                lstNodes.Add(node.Value);
                VisitTreeInOrder(node.Right, lstNodes);
            }
        } //VisitTreeDFS

        #endregion Traversing

        #region Searching
        
        //NB: Not recursive
        private Node Find(dynamic value)
        {
            Node node = this.Root;
            while (node != null)
            {
                int compareTo = value.CompareTo(node.Value);
                if (compareTo < 0)
                    node = node.Left;
                else
                    if (compareTo > 0)
                        node = node.Right;
                    else //values are equal - node is found, break out of loop
                        break;
            } //while

            return node;
        } //Find
        
        public bool Contains(dynamic value)
        {
            bool found = this.Find(value) != null;
            return found;
        } //Contains

        #endregion       

        //Not for tests or exam
        #region PrintVisual

        public void PrintVisual()
        {
            int depth = GetDepth(Root) - 1;
            dynamic prev = null;
            Root.nSpaces = 10 + (int)(Math.Pow(2, depth));
            Queue<Node> Q = new Queue<Node>();
            Q.Enqueue(Root);
            while (Q.Count > 0)
            {
                Node current = Q.Dequeue();

                //Start new line if necessary
                if (current.Value < prev)
                {
                    Console.WriteLine(); Console.WriteLine();
                    depth--;
                }
                if (current.nSpaces >= 0 && current.nSpaces < Console.BufferWidth)
                {
                    Console.SetCursorPosition(current.nSpaces, Console.CursorTop);
                    Console.Write(current.Value.ToString().PadLeft(2));
                }
                prev = current.Value;

                if (current.Left != null)
                {
                    current.Left.nSpaces = current.nSpaces - (int)Math.Pow(2, depth);
                    Q.Enqueue(current.Left);
                }
                if (current.Right != null)
                {
                    current.Right.nSpaces = current.nSpaces + (int)Math.Pow(2, depth);
                    Q.Enqueue(current.Right);
                }
            } //while

            Console.WriteLine();
            //Console.WriteLine(Root.ToString());
        } //PrintVisual

        private int GetDepth(Node node)
        {
            int depthR = 0, depthL = 0;
            if (node.Right != null) depthR = GetDepth(node.Right);
            if (node.Left != null) depthL = GetDepth(node.Left);
            return Math.Max(depthR, depthL) + 1;
        } //GetDepth

        private void PrintVisual(Node node)
        {
            if (node == null)
                return;
            Console.Write(node.Value);
            PrintVisual(node.Left);
            PrintVisual(node.Right);
        } //PrintVisual

        #endregion PrintVisual

        //Not for tests or exam
        #region Removing an element

        public void Remove(dynamic value)
        {
            Node nodeToDelete = Find(value);
            if (nodeToDelete != null)
                Remove(nodeToDelete);
        } //Remove

        private void Remove(Node node)
        {
            // Case 3: If the node has two children.
            // Note that if we get here at the end, the node will be with at most one child
            if (node.Left != null && node.Right != null)
            {
                //Node replacement = node.Right; //Go right and then left, left, left, ... until we reach the bottom
                //while (replacement.Left != null)
                Node replacement = node.Left; //Or go left and then right, right, right, ... until we reach the bottom
                while (replacement.Right != null)
                    replacement = replacement.Right;
                node.Value = replacement.Value; //Dont' swap the nodes, just replace the value
                node = replacement;
            }

            // Case 1 and 2: If the node has at most one child
            Node theChild = node.Left != null ? node.Left : node.Right;

            // If the element to be deleted has one child
            if (theChild != null)
            {
                theChild.Parent = node.Parent;

                // Handle the case when the element is the root
                if (node.Parent == null)
                    Root = theChild;
                else
                    // Replace the element with its child sub-tree
                    if (node.Parent.Left == node)
                        node.Parent.Left = theChild;
                    else
                        node.Parent.Right = theChild;
            }
            else
            {
                // Handle the case when the element is the root
                if (node.Parent == null)
                    Root = null;
                else
                {
                    // Remove the element - it is a leaf
                    if (node.Parent.Left == node)
                        node.Parent.Left = null;
                    else
                        node.Parent.Right = null;
                }
            }
        } //Remove

        #endregion

    } //class BST
} //namespace
