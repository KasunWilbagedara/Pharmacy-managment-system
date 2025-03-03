using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy
{
    class AVLNode
    {
        public Medicine Data;
        public AVLNode Left, Right;
        public int Height;

        public AVLNode(Medicine med)
        {
            Data = med;
            Height = 1;
        }
    }

    internal class AVL_Tree
    {
        private AVLNode root;

        // Get Height of Node
        private int Height(AVLNode node) => node == null ? 0 : node.Height;

        // Get Balance Factor
        private int GetBalance(AVLNode node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

        // Right Rotate
        private AVLNode RightRotate(AVLNode y)
        {
            AVLNode x = y.Left;
            AVLNode T2 = x.Right;
            x.Right = y;
            y.Left = T2;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            return x;
        }

        // Left Rotate
        private AVLNode LeftRotate(AVLNode x)
        {
            AVLNode y = x.Right;
            AVLNode T2 = y.Left;
            y.Left = x;
            x.Right = T2;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            return y;
        }

        // Insert Medicine in AVL Tree
        public void Insert(Medicine med)
        {
            root = Insert(root, med);
        }

        private AVLNode Insert(AVLNode node, Medicine med)
        {
            if (node == null)
                return new AVLNode(med);

            if (med.Id < node.Data.Id)
                node.Left = Insert(node.Left, med);
            else if (med.Id > node.Data.Id)
                node.Right = Insert(node.Right, med);
            else
                return node; // Duplicates not allowed

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            if (balance > 1 && med.Id < node.Left.Data.Id)
                return RightRotate(node);

            if (balance < -1 && med.Id > node.Right.Data.Id)
                return LeftRotate(node);

            if (balance > 1 && med.Id > node.Left.Data.Id)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && med.Id < node.Right.Data.Id)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        // Search for a medicine by ID
        public Medicine Search(int id)
        {
            return Search(root, id);
        }

        private Medicine Search(AVLNode node, int id)
        {
            if (node == null)
                return null;

            if (id < node.Data.Id)
                return Search(node.Left, id);
            else if (id > node.Data.Id)
                return Search(node.Right, id);
            else
                return node.Data;
        }

        // In-Order Traversal (Sorted Inventory by ID)
        public List<Medicine> GetAllMedicines()
        {
            List<Medicine> result = new List<Medicine>();
            InOrder(root, result);
            return result;
        }

        private void InOrder(AVLNode node, List<Medicine> result)
        {
            if (node == null)
                return;

            InOrder(node.Left, result);
            result.Add(node.Data);
            InOrder(node.Right, result);
        }
    }
}
