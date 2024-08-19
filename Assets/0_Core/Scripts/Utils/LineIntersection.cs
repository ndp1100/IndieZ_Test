﻿using System;
using UnityEngine;

public static class LineIntersection
{
    public const float kTolerance = 0.01f;

    public static bool FindIntersection(Vector2 point1, Vector2 point2,
        float x3, float y3, float x4, float y4, bool checkOnInside, out Vector2 point, float tolerance = kTolerance)
    {
        return FindIntersection(point1.x, point1.y, point2.x, point2.y, x3, y3, x4, y4, checkOnInside, out point,
        tolerance);
    }


    public static bool FindIntersection(float x1, float y1, float x2, float y2,
        float x3, float y3, float x4, float y4, bool checkOnInside, out Vector2 point, float tolerance = kTolerance)
    {
        // equations of the form x = c (two vertical lines)
        if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
        {
            //throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
            point = Vector2.zero;
            return false;
        }

        //equations of the form y=c (two horizontal lines)
        if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
        {
            //throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
            point = Vector2.zero;
            return false;
        }

        //equations of the form x=c (two vertical lines)
        if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
        {
            point = Vector2.zero;
            return false;
        }

        //equations of the form y=c (two horizontal lines)
        if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
        {
            point = Vector2.zero;
            return false;
        }

        //general equation of line is y = mx + c where m is the slope
        //assume equation of line 1 as y1 = m1x1 + c1 
        //=> -m1x1 + y1 = c1 ----(1)
        //assume equation of line 2 as y2 = m2x2 + c2
        //=> -m2x2 + y2 = c2 -----(2)
        //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
        //so we will get below two equations 
        //-m1x + y = c1 --------(3)
        //-m2x + y = c2 --------(4)

        float x, y;

        //lineA is vertical x1 = x2
        //slope will be infinity
        //so lets derive another solution
        if (Math.Abs(x1 - x2) < tolerance)
        {
            //compute slope of line 2 (m2) and c2
            float m2 = (y4 - y3) / (x4 - x3);
            float c2 = -m2 * x3 + y3;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x1=c1=x
            //subsitute x=x1 in (4) => -m2x1 + y = c2
            // => y = c2 + m2x1 
            x = x1;
            y = c2 + m2 * x1;
        }
        //lineB is vertical x3 = x4
        //slope will be infinity
        //so lets derive another solution
        else if (Math.Abs(x3 - x4) < tolerance)
        {
            //compute slope of line 1 (m1) and c2
            float m1 = (y2 - y1) / (x2 - x1);
            float c1 = -m1 * x1 + y1;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x3=c3=x
            //subsitute x=x3 in (3) => -m1x3 + y = c1
            // => y = c1 + m1x3 
            x = x3;
            y = c1 + m1 * x3;
        }
        //lineA & lineB are not vertical 
        //(could be horizontal we can handle it with slope = 0)
        else
        {
            //compute slope of line 1 (m1) and c2
            float m1 = (y2 - y1) / (x2 - x1);
            float c1 = -m1 * x1 + y1;

            //compute slope of line 2 (m2) and c2
            float m2 = (y4 - y3) / (x4 - x3);
            float c2 = -m2 * x3 + y3;

            //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
            //plugging x value in equation (4) => y = c2 + m2 * x
            x = (c1 - c2) / (m2 - m1);
            y = c2 + m2 * x;

            //verify by plugging intersection point (x, y)
            //in orginal equations (1) & (2) to see if they intersect
            //otherwise x,y values will not be finite and will fail this check

            if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                && Math.Abs(-m2 * x + y - c2) < tolerance))
            {
                point = Vector2.zero;
                return false;
            }
        }

        //x,y can intersect outside the line segment since line is infinitely long
        //so finally check if x, y is within both the line segments
        if (!checkOnInside || (IsInsideLine(x1, y1, x2, y2, x, y, tolerance) &&
            IsInsideLine(x3, y3, x4, y4, x, y, tolerance)))
        {
            point = new Vector2() { x = x, y = y };
            return true;
        }

        point = Vector2.zero;
        return false;

    }

    // Returns true if given point(x,y) is inside the given line segment
    private static bool IsInsideLine(float x1, float y1, float x2, float y2, float x, float y, float t)
    {
        return (x >= x1 - t && x <= x2 + t
                    || x >= x2 - t && x <= x1 + t)
               && (y >= y1 - t && y <= y2 + t
                    || y >= y2 - t && y <= y1 + t);
    }
}