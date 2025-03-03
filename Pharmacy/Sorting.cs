using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy
{
    public static class Sorting
    {
        // Quick Sort for sorting medicines by name
        public static void QuickSortByName(List<Medicine> medicines, int low, int high)
        {
            if (low < high)
            {
                int pi = PartitionByName(medicines, low, high);
                QuickSortByName(medicines, low, pi - 1);
                QuickSortByName(medicines, pi + 1, high);
            }
        }

        private static int PartitionByName(List<Medicine> medicines, int low, int high)
        {
            Medicine pivot = medicines[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (string.Compare(medicines[j].Name, pivot.Name) < 0)
                {
                    i++;
                    Swap(medicines, i, j);
                }
            }
            Swap(medicines, i + 1, high);
            return i + 1;
        }

        // Merge Sort for sorting medicines by expiry date
        public static void MergeSortByExpiry(List<Medicine> medicines)
        {
            if (medicines.Count > 1)
            {
                int mid = medicines.Count / 2;
                List<Medicine> left = medicines.GetRange(0, mid);
                List<Medicine> right = medicines.GetRange(mid, medicines.Count - mid);

                MergeSortByExpiry(left);
                MergeSortByExpiry(right);

                Merge(medicines, left, right);
            }
        }

        private static void Merge(List<Medicine> medicines, List<Medicine> left, List<Medicine> right)
        {
            int i = 0, j = 0, k = 0;

            while (i < left.Count && j < right.Count)
            {
                if (left[i].ExpiryDate.CompareTo(right[j].ExpiryDate) < 0)
                {
                    medicines[k++] = left[i++];
                }
                else
                {
                    medicines[k++] = right[j++];
                }
            }

            while (i < left.Count)
            {
                medicines[k++] = left[i++];
            }

            while (j < right.Count)
            {
                medicines[k++] = right[j++];
            }
        }

        // Swap helper method
        private static void Swap(List<Medicine> medicines, int i, int j)
        {
            Medicine temp = medicines[i];
            medicines[i] = medicines[j];
            medicines[j] = temp;
        }
    }
}
