using System;

namespace Pharmacy
{
    public class AVLNode
    {
        public object Data { get; set; } 
        public AVLNode? Left { get; set; } 
        public AVLNode? Right { get; set; } 
        public int Height { get; set; }

        public AVLNode(object data)
        {
            Data = data;
            Left = null;
            Right = null;
            Height = 1;
        }
    }

    public class AVL_Tree
    {
        private AVLNode? root; 

        private int Height(AVLNode? node) => node == null ? 0 : node.Height;
        private int GetBalance(AVLNode? node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

        private AVLNode RightRotate(AVLNode y)
        {
            AVLNode? x = y.Left;
            AVLNode? T2 = x?.Right;
            if (x != null) x.Right = y;
            y.Left = T2;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            if (x != null) x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            return x ?? y;
        }

        private AVLNode LeftRotate(AVLNode x)
        {
            AVLNode? y = x.Right;
            AVLNode? T2 = y?.Left;
            if (y != null) y.Left = x;
            x.Right = T2;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            if (y != null) y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            return y ?? x; 
        }

        public void Insert(object data)
        {
            root = Insert(root, data);
        }

        private AVLNode Insert(AVLNode? node, object data)
        {
            if (node == null)
                return new AVLNode(data);

            int compareResult;
            if (data is Medicine med)
                compareResult = med.Id.CompareTo(((Medicine)node.Data).Id);
            else if (data is Buyer buyer)
                compareResult = buyer.BuyerId.CompareTo(((Buyer)node.Data).BuyerId);
            else
                throw new ArgumentException("Unsupported data type for AVL Tree");

            if (compareResult < 0)
                node.Left = Insert(node.Left, data);
            else if (compareResult > 0)
                node.Right = Insert(node.Right, data);
            else
                return node; 

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
            int balance = GetBalance(node);

            if (balance > 1 && Compare(data, node.Left!.Data) < 0)
                return RightRotate(node);
            if (balance < -1 && Compare(data, node.Right!.Data) > 0)
                return LeftRotate(node);
            if (balance > 1 && Compare(data, node.Left!.Data) > 0)
            {
                node.Left = LeftRotate(node.Left!);
                return RightRotate(node);
            }
            if (balance < -1 && Compare(data, node.Right!.Data) < 0)
            {
                node.Right = RightRotate(node.Right!);
                return LeftRotate(node);
            }

            return node;
        }

        private int Compare(object a, object b)
        {
            if (a is Medicine medA && b is Medicine medB)
                return medA.Id.CompareTo(medB.Id);
            if (a is Buyer buyerA && b is Buyer buyerB)
                return buyerA.BuyerId.CompareTo(buyerB.BuyerId);
            throw new ArgumentException("Incompatible types for comparison");
        }

        public object? Search(int id)
        {
            return Search(root, id); 
        }

        private object? Search(AVLNode? node, int id)
        {
            if (node == null)
                return null;

            int nodeId = node.Data is Medicine med ? med.Id : ((Buyer)node.Data).BuyerId;
            if (id < nodeId)
                return Search(node.Left, id);
            else if (id > nodeId)
                return Search(node.Right, id);
            else
                return node.Data;
        }
    }
}