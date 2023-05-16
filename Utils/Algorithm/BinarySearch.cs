
namespace Prota
{
    public static partial class Algorithm
    {
        // 找到大于等于给定值的第一个位置.
        // compare: 返回 -1 表示给定值在目标左侧; 返回 1 表示给定值在目标右侧; 返回 0 表示给定值和目标值相等.
        public static int LowerBound(int min, int max, System.Func<int, int> compare)
        {
            while (min < max)
            {
                int mid = (min + max) / 2;
                int result = compare(mid);
                if (result < 0) min = mid + 1;
                else max = mid;
            }
            if(compare(max) < 0) return max + 1;
            return max;
        }
        
        // 找到大于给定值的第一个位置.
        // compare: 返回 -1 表示给定值在目标左侧; 返回 1 表示给定值在目标右侧; 返回 0 表示给定值和目标值相等.
        public static int UpperBound(int min, int max, System.Func<int, int> compare)
        {
            while (min < max)
            {
                int mid = (min + max) / 2;
                int result = compare(mid);
                if (result > 0) max = mid;
                else min = mid + 1;
            }
            if(compare(max) <= 0) return max + 1;
            return max;
        }
        
    }
}
