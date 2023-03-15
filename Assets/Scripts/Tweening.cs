using System;

/***
 * @see: https://easings.net/
 * ***/
public static class Tweening {

    public static double EaseInSine(double x) {
        return 1 - Math.Cos((x * Math.PI) / 2);
    }

    public static double EaseInCubic(double x) {
        return x * x * x;
    }

    public static double EaseInQuint(double x) {
        return x * x * x * x * x;
    }
    public static double EaseInCirc(double x) {
        return 1 - Math.Sqrt(1 - Math.Pow(x, 2));
    }

    public static double EaseInElastic(double x) {
        double c4 = (2 * Math.PI) / 3;
        return x == 0 ? 0 : x == 1 ? 1 : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4);
    }

    public static double EaseOutSine(double x) {
        return Math.Sin((x * Math.PI) / 2);
    }

    public static double EaseOutCubic(double x) {
        return 1 - Math.Pow(1 - x, 3);
    }

    public static double EaseOutQuint(double x) {
        return 1 - Math.Pow(1 - x, 5);
    }

    public static double EaseOutCirc(double x) {
        return Math.Sqrt(1 - Math.Pow(x - 1, 2));
    }

    public static double EaseOutElastic(double x) {
        double c4 = (2 * Math.PI) / 3;
        return x == 0 ? 0 : x == 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75d) * c4) + 1;
    }
    public static double EaseOutSince(double x) {
        return -(Math.Cos(Math.PI * x) - 1) / 2;
    }

    public static double EaseInOutCubic(double x) {
        return x < 0.5d ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2;
    }

    public static double EaseInOutQuint(double x) {
        return x < 0.5d ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2;
    }

    public static double EaseInOutCirc(double x) {
        return x < 0.5d ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2
            : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
    }

    public static double EaseInOutElastic(double x) {
        double c5 = (2 * Math.PI) / 4.5d;
        return x == 0 ? 0
            : x == 1 ? 1
            : x < 0.5 ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125d) * c5)) / 2
            : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125d) * c5)) / 2 + 1;
    }

    public static double EaseInQuad(double x) {
        return x * x;
    }

    public static double EaseInQuart(double x) {
        return x * x * x * x;
    }

    public static double EaseInExpo(double x) {
        return x == 0 ? 0 : Math.Pow(2, 10 * x - 10);
    }

    public static double EaseInBack(double x) {
        double c1 = 1.70158d;
        double c3 = c1 + 1;

        return c3 * x * x * x - c1 * x * x;
    }

    public static double EaseInBounce(double x) {
        return 1 - EaseOutBounce(1 - x);
    }

    public static double EaseOutQuad(double x) {
        return 1 - (1 - x) * (1 - x);
    }

    public static double EaseOutQuart(double x) {
        return 1 - Math.Pow(1 - x, 4);
    }

    public static double EaseOutExpo(double x) {
        return x == 1 ? 1 : 1 - Math.Pow(2, -10 * x);
    }

    public static double EaseOutBack(double x) {
        double c1 = 1.70158d;
        double c3 = c1 + 1;

        return 1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2);
    }

    public static double EaseOutBounce(double x) {
        double n1 = 7.5625d;
        double d1 = 2.75d;
        if (x < 1 / d1) {
            return n1 * x * x;
        } else if (x < 2 / d1) {
            return n1 * (x -= 1.5 / d1) * x + 0.75d;
        } else if (x < 2.5 / d1) {
            return n1 * (x -= 2.25 / d1) * x + 0.9375d;
        } else {
            return n1 * (x -= 2.625 / d1) * x + 0.984375d;
        }
    }

    public static double EaseInOutQuad(double x) {
        return x < 0.5d ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2;
    }

    public static double EaseInOutQuart(double x) {
        return x < 0.5d ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2;
    }

    public static double EaseInOutExpo(double x) {
        return x == 0 ? 0
            : x == 1 ? 1
            : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2
            : (2 - Math.Pow(2, -20 * x + 10)) / 2;
    }
    public static double EaseInOutBack(double x) {
        double c1 = 1.70158d;
        double c2 = c1 * 1.525d;

        return x < 0.5d ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
                    : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
    public static double EaseInOutBounce(double x) {
        return x < 0.5d ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2;
    }
}
