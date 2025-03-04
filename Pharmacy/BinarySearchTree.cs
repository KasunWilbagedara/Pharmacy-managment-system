using System;

namespace Pharmacy
{
    public class BSTNode
    {
        public object Data { get; set; }
        public BSTNode? Left { get; set; } 
        public BSTNode? Right { get; set; } 

        public BSTNode(object data)
        {
            Data = data;
            Left = null;
            Right = null;
        }
    }

    public class BinarySearchTree
    {
        private BSTNode? root; 

        public void Insert(object data)
        {
            root = InsertRec(root, data);
        }

        private BSTNode InsertRec(BSTNode? node, object data)
        {
            if (node == null)
                return new BSTNode(data);

            int compareResult;
            if (data is Medicine med)
                compareResult = string.Compare(med.Name, ((Medicine)node.Data).Name, StringComparison.OrdinalIgnoreCase);
            else if (data is Buyer buyer)
                compareResult = string.Compare(buyer.Name, ((Buyer)node.Data).Name, StringComparison.OrdinalIgnoreCase);
            else
                throw new ArgumentException("Unsupported data type for BST");

            if (compareResult < 0)
                node.Left = InsertRec(node.Left, data);
            else if (compareResult > 0)
                node.Right = InsertRec(node.Right, data);
            return node;
        }

        public object? Search(string name)
        {
            return SearchRec(root, name ?? string.Empty); 
        }

        private object? SearchRec(BSTNode? node, string name)
        {
            if (node == null)
                return null;

            string nodeName = node.Data is Medicine med ? med.Name : ((Buyer)node.Data).Name;
            int compareResult = string.Compare(name, nodeName, StringComparison.OrdinalIgnoreCase);
            if (compareResult == 0)
                return node.Data;
            else if (compareResult < 0)
                return SearchRec(node.Left, name);
            else
                return SearchRec(node.Right, name);
        }
    }
}