using System;
using SFML.System;


static class Util {
    public const int WIN_WIDTH = 1200;
    public const int WIN_HEIGHT = 650;
    public const float POINT_RAD = 4.0f;
    public const float PADDING_R = 10.0f; //Padding percentage
    public const int PADDING_X = (int)(PADDING_R / 100.0f * (WIN_WIDTH / 2));
    public const int PADDING_Y = (int)(PADDING_R / 100.0f * WIN_HEIGHT);

    //Function for generating next lexical permutation
    //Algorithm src: https://www.quora.com/How-would-you-explain-an-algorithm-that-generates-permutations-using-lexicographic-ordering
    public static bool NextPerm(int[] arr)
    {
        int x = -1;
        for (int i = 0; i < arr.Length - 1; ++i) {
            if (arr[i] < arr[i + 1]) {
                x = i;
            } 
        }

        if (x == -1) return false;

        int y = 0;

        for (int i = 0; i < arr.Length; ++i) {
            if (arr[i] > arr[x]) {
                y = i;
            }
        }

        int temp = arr[x];
        arr[x] = arr[y];
        arr[y] = temp;

        Array.Reverse(arr, x + 1, arr.Length - (x + 1));

        return true;
    }

    //Compute distance taken in a cycle through points in given order
    public static float TotalDistance(Vector2f[] points, int[] order)
    {
        float distance = 0.0f;

        for (int i = 0; i < points.Length; ++i) {
            int from = order[i];
            int to = order[(i + 1) >= points.Length ? 0 : i + 1];

            float x = points[to].X - points[from].X;
            float y = points[to].Y - points[from].Y;

            distance += x * x + y * y;
        }

        return distance;
    }

    public static int Factorial(int n)
    {
        int product = 1;

        while (n > 1) {
            product *= n;
            --n;
        }
            
        return product;
    }
}