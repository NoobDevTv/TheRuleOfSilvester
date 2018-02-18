using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class BaseElement
    {
        public int ElementID { get; set; }
        public ConnectionPoints Connections { get; set; }


        public BaseElement(int elementID, ConnectionPoints connections)
        {
            ElementID = elementID;
            Connections = connections;
        }

        public static implicit operator BaseElement(char value)
        {
            switch (value)
            {
                case '│': return new BaseElement(1, ConnectionPoints.Up | ConnectionPoints.Down);
                case '║': return new BaseElement(2, ConnectionPoints.Up | ConnectionPoints.Down);
                case '─': return new BaseElement(3, ConnectionPoints.Left | ConnectionPoints.Right);
                case '═': return new BaseElement(4, ConnectionPoints.Left | ConnectionPoints.Right);
                case '┌': return new BaseElement(5, ConnectionPoints.Down | ConnectionPoints.Right);
                case '╔': return new BaseElement(6, ConnectionPoints.Down | ConnectionPoints.Right);
                case '└': return new BaseElement(7, ConnectionPoints.Up | ConnectionPoints.Right);
                case '╚': return new BaseElement(8, ConnectionPoints.Up | ConnectionPoints.Right);
                case '┐': return new BaseElement(9, ConnectionPoints.Left | ConnectionPoints.Down);
                case '╗': return new BaseElement(10, ConnectionPoints.Left | ConnectionPoints.Down);
                case '┘': return new BaseElement(11, ConnectionPoints.Left | ConnectionPoints.Up);
                case '╝': return new BaseElement(12, ConnectionPoints.Left | ConnectionPoints.Up);
                case '┬': return new BaseElement(13, ConnectionPoints.Left | ConnectionPoints.Right | ConnectionPoints.Down);
                case '╦': return new BaseElement(14, ConnectionPoints.Left | ConnectionPoints.Right | ConnectionPoints.Down);
                case '┴': return new BaseElement(15, ConnectionPoints.Left | ConnectionPoints.Right | ConnectionPoints.Up);
                case '╩': return new BaseElement(16, ConnectionPoints.Left | ConnectionPoints.Right | ConnectionPoints.Up);
                case '├': return new BaseElement(17, ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Right);
                case '╠': return new BaseElement(18, ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Right);
                case '┤': return new BaseElement(19, ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Left);
                case '╣': return new BaseElement(20, ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Left);
                case '┼': return new BaseElement(21, ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Left | ConnectionPoints.Right);
                case '╬': return new BaseElement(22, ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Left | ConnectionPoints.Right);
                default:
                    return new BaseElement(value, ConnectionPoints.None);
            }
        }

        public static implicit operator BaseElement(ConnectionPoints connectionPoints)
        {
            switch (connectionPoints)
            {
                case ConnectionPoints.Up | ConnectionPoints.Down: return new BaseElement(1, connectionPoints);
                case ConnectionPoints.Left | ConnectionPoints.Right: return new BaseElement(3, connectionPoints);
                case ConnectionPoints.Down | ConnectionPoints.Right: return new BaseElement(5, connectionPoints);
                case ConnectionPoints.Up | ConnectionPoints.Right: return new BaseElement(7, connectionPoints);
                case ConnectionPoints.Left | ConnectionPoints.Down: return new BaseElement(9, connectionPoints);
                case ConnectionPoints.Left | ConnectionPoints.Up: return new BaseElement(11, connectionPoints);
                case ConnectionPoints.Left | ConnectionPoints.Right | ConnectionPoints.Down: return new BaseElement(13, connectionPoints);
                case ConnectionPoints.Left | ConnectionPoints.Right | ConnectionPoints.Up: return new BaseElement(15, connectionPoints);
                case ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Right: return new BaseElement(17, connectionPoints);
                case ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Left: return new BaseElement(19, connectionPoints);
                case ConnectionPoints.Down | ConnectionPoints.Up | ConnectionPoints.Left | ConnectionPoints.Right: return new BaseElement(21, connectionPoints);
                default:
                    return new BaseElement(0, connectionPoints);
            }
        }
    }
}
