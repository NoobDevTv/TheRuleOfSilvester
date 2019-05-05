using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Cells
{
    public class BaseElement
    {
        public int ElementID { get; set; }
        public ConnectionPoint Connections { get; set; }


        public BaseElement(int elementID, ConnectionPoint connections)
        {
            ElementID = elementID;
            Connections = connections;
        }
        
        public static implicit operator BaseElement(char value)
        {
            switch (value)
            {
                case '│': return new BaseElement(1, ConnectionPoint.Up | ConnectionPoint.Down);
                case '║': return new BaseElement(2, ConnectionPoint.Up | ConnectionPoint.Down);
                case '─': return new BaseElement(3, ConnectionPoint.Left | ConnectionPoint.Right);
                case '═': return new BaseElement(4, ConnectionPoint.Left | ConnectionPoint.Right);
                case '┌': return new BaseElement(5, ConnectionPoint.Down | ConnectionPoint.Right);
                case '╔': return new BaseElement(6, ConnectionPoint.Down | ConnectionPoint.Right);
                case '└': return new BaseElement(7, ConnectionPoint.Up | ConnectionPoint.Right);
                case '╚': return new BaseElement(8, ConnectionPoint.Up | ConnectionPoint.Right);
                case '┐': return new BaseElement(9, ConnectionPoint.Left | ConnectionPoint.Down);
                case '╗': return new BaseElement(10, ConnectionPoint.Left | ConnectionPoint.Down);
                case '┘': return new BaseElement(11, ConnectionPoint.Left | ConnectionPoint.Up);
                case '╝': return new BaseElement(12, ConnectionPoint.Left | ConnectionPoint.Up);
                case '┬': return new BaseElement(13, ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Down);
                case '╦': return new BaseElement(14, ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Down);
                case '┴': return new BaseElement(15, ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Up);
                case '╩': return new BaseElement(16, ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Up);
                case '├': return new BaseElement(17, ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Right);
                case '╠': return new BaseElement(18, ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Right);
                case '┤': return new BaseElement(19, ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Left);
                case '╣': return new BaseElement(20, ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Left);
                case '┼': return new BaseElement(21, ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Left | ConnectionPoint.Right);
                case '╬': return new BaseElement(22, ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Left | ConnectionPoint.Right);
                default:
                    return new BaseElement(value, ConnectionPoint.None);
            }
        }

        public static implicit operator BaseElement(ConnectionPoint connectionPoints)
        {
            switch (connectionPoints)
            {
                case ConnectionPoint.Up | ConnectionPoint.Down: return new BaseElement(1, connectionPoints);
                case ConnectionPoint.Left | ConnectionPoint.Right: return new BaseElement(3, connectionPoints);
                case ConnectionPoint.Down | ConnectionPoint.Right: return new BaseElement(5, connectionPoints);
                case ConnectionPoint.Up | ConnectionPoint.Right: return new BaseElement(7, connectionPoints);
                case ConnectionPoint.Left | ConnectionPoint.Down: return new BaseElement(9, connectionPoints);
                case ConnectionPoint.Left | ConnectionPoint.Up: return new BaseElement(11, connectionPoints);
                case ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Down: return new BaseElement(13, connectionPoints);
                case ConnectionPoint.Left | ConnectionPoint.Right | ConnectionPoint.Up: return new BaseElement(15, connectionPoints);
                case ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Right: return new BaseElement(17, connectionPoints);
                case ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Left: return new BaseElement(19, connectionPoints);
                case ConnectionPoint.Down | ConnectionPoint.Up | ConnectionPoint.Left | ConnectionPoint.Right: return new BaseElement(21, connectionPoints);
                default:
                    return new BaseElement(0, connectionPoints);
            }
        }
    }
}
