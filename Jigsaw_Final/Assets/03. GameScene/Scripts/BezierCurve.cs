using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{

    // a look up table for factorials. Capped to 18.
    private static float[] Factorial = new float[]
    {
        1.0f,
        1.0f,
        2.0f,
        6.0f,
        24.0f,
        120.0f,
        720.0f,
        5040.0f,
        40320.0f,
        362880.0f,
        3628800.0f,
        39916800.0f,
        479001600.0f,
        6227020800.0f,
        87178291200.0f,
        1307674368000.0f,
        20922789888000.0f,
        355687428096000.0f,
        6402373705728000.0f,
    };

    private static float Binomial(int n, int i)
    {
        float ni;
        float a1 = Factorial[n];
        float a2 = Factorial[i];
        float a3 = Factorial[n - i];
        ni = a1 / (a2 * a3);
        return ni;
    }

    private static float Bernstein(int n, int i, float t)
    {
        float t_i = Mathf.Pow(t, i);
        float t_n_minus_i = Mathf.Pow((1 - t), (n - i));

        float basis = Binomial(n, i) * t_i * t_n_minus_i;
        return basis;
    }


    /// <summary>
    /// x축 양수 방향으로 뻗어나가는 BezierCurve를 생성합니다.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static List<Vector2> PointList2(List<Vector2> controlPoints, float interval)
    {
        int N = controlPoints.Count - 1;

        List<Vector2> points = new List<Vector2>();

        for (float t = 0.0f; t <= 1.0f + interval; t += interval)
        {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }

            points.Add(p);
        }

        return points;
    }

}