using System;
using Microsoft.Xna.Framework;

namespace SystemX.GUI.Helpers {
    public static class LayoutHelper {
        public static Point DoLayout(HorizontalAlignment align,
                                     VerticalAlignment valign,
                                     ref Point ownerPos,
                                     ref Point ownerSize,
                                     ref Point pos,
                                     ref Point size) {
            Point finalPos = Point.Zero;
            
            if (ownerSize == Point.Zero) ownerSize = size;

            if (size.X > ownerSize.X) size.X = ownerSize.X;
            if (size.Y > ownerSize.Y) size.Y = ownerSize.Y;

            switch (align) {
                case HorizontalAlignment.None:
                    finalPos.X = ownerPos.X + pos.X;
                    if ((finalPos.X + size.X) > (ownerPos.X + ownerSize.X))
                        size.X = size.X - ((finalPos.X + size.X) - (ownerPos.X + ownerSize.X));
                    break;

                case HorizontalAlignment.Left:
                    finalPos.X = ownerPos.X + pos.X;
                    if ((finalPos.X + size.X) > (ownerPos.X + ownerSize.X))
                        size.X = size.X - ((finalPos.X + size.X) - (ownerPos.X + ownerSize.X));
                    break;

                case HorizontalAlignment.Center:
                    finalPos.X = CenterAlign(ownerPos.X, ownerSize.X, pos.X, size.X);
                    if (finalPos.X < ownerPos.X) {
                        finalPos.X = ownerPos.X;
                        size.X = size.X - ((finalPos.X + size.X) - (ownerPos.X + ownerSize.X));
                    }
                    break;
                case HorizontalAlignment.Right:
                    finalPos.X = RightBottomAlign(ownerPos.X, ownerSize.X, pos.X, size.X);
                    if (finalPos.X < ownerPos.X)
                        finalPos.X = ownerPos.X;
                    if ((finalPos.X + size.X) > (ownerPos.X + ownerSize.X))
                        size.X = size.X - ((finalPos.X + size.X) - (ownerPos.X + ownerSize.X));
                    break;
            }

            switch (valign) {
                case VerticalAlignment.None:
                    finalPos.Y = ownerPos.Y + pos.Y;
                    if ((finalPos.Y + size.Y) > (ownerPos.Y + ownerSize.Y))
                        size.Y = size.Y - ((finalPos.Y + size.Y) - (ownerPos.Y + ownerSize.Y));
                    break;

                case VerticalAlignment.Top:
                    finalPos.Y = ownerPos.Y + pos.Y;
                    if ((finalPos.Y + size.Y) > (ownerPos.Y + ownerSize.Y))
                        size.Y = size.Y - ((finalPos.Y + size.Y) - (ownerPos.Y + ownerSize.Y));
                    break;

                case VerticalAlignment.Middle:
                    finalPos.Y = CenterAlign(ownerPos.Y, ownerSize.Y, pos.Y, size.Y);
                    if (finalPos.Y < ownerPos.Y) {
                        finalPos.Y = ownerPos.Y;
                        size.Y = size.Y - ((finalPos.Y + size.Y) - (ownerPos.Y + ownerSize.Y));
                    }
                    break;
                case VerticalAlignment.Bottom:
                    finalPos.Y = RightBottomAlign(ownerPos.Y, ownerSize.Y, pos.Y, size.Y);

                    if (finalPos.Y < ownerPos.Y)
                        finalPos.Y = ownerPos.Y;

                    if ((finalPos.Y + size.Y) > (ownerPos.Y + ownerSize.Y))
                        size.Y = size.Y - ((finalPos.Y + size.Y) - (ownerPos.Y + ownerSize.Y));
                    break;
            }

            return finalPos;
        }

        private static int CenterAlign(int ownerLoc, int ownerSize, int thisLoc, int thisSize) {
            if (thisLoc != 0)
                return (ownerLoc + thisLoc) - (thisSize >> 1);

            return (ownerLoc + (ownerSize >> 1)) - (thisSize >> 1);
        }

        private static int RightBottomAlign(int ownerLoc, int ownerSize, int thisLoc, int thisSize) {
            if (thisLoc != 0)
                return (ownerLoc + thisLoc) - thisSize;

            return ownerLoc + ownerSize - thisSize;
        }

        public static Point ParsePoint(string str, Point relativeTo) {
            if (string.IsNullOrWhiteSpace(str))
                return Point.Zero;

            string[] parts = str.Split(',');
            if (parts.Length != 2)
                throw new FormatException("Size or Location is not in the correct format. Expected: \"x,y\"");

            Point result = Point.Zero;
            result.X = (int)HandleFloat(parts[0], relativeTo.X / 100.0f);
            result.Y = (int)HandleFloat(parts[1], relativeTo.Y / 100.0f);

            return result;
        }

        public static Vector2 ParseVector2(string str, Vector2 relativeTo) {
            if (string.IsNullOrWhiteSpace(str))
                return Vector2.Zero;

            string[] parts = str.Split(',');
            if (parts.Length != 2)
                throw new FormatException("Size/Location is not in the correct format. Expected: \"x,y\"");

            Vector2 result = Vector2.Zero;
            result.X = HandleFloat(parts[0], relativeTo.X / 100);
            result.Y = HandleFloat(parts[1], relativeTo.Y / 100);

            return result;
        }

        public static float HandleFloat(string str, float percentMult) {
            str = str.Trim();

            return str.Contains("%") ? float.Parse(str.Replace("%", "")) * percentMult : float.Parse(str);
        }
    }
}