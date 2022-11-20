using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Circuits
{
    internal class OutputLamp : Gate
    {
        private const int xGAP = 10;
        private const int yGAP = 10;
        private Color HighValt = Color.Red;
        private Color LowValt = Color.Black;
        private bool value = false;
        /// <summary>
        /// Initialises the OutputLamp.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        public OutputLamp(int x, int y) : base(x, y, 40, 40)
        {
            pins.Add(new Pin(this, true, 10));
            //move the gate and the pins to the position passed in
            MoveTo(x, y);
        }


        /// <summary>
        /// Draws the gate in the normal colour or in the selected colour.
        /// </summary>
        /// <param name="paper"></param>
        public override void Draw(Graphics paper)
        {
            Pen pen = new Pen(Color.Black, 3);
            SolidBrush brush = new SolidBrush(HighValt);
            //Drawing each pin
            foreach (Pin p in pins)
                p.Draw(paper);
            if (Selected)
            {
                pen.Color = SelectedColor;
            }
            else
            {
                pen.Color = DeseletedColor;
            }
            paper.DrawEllipse(pen, Left, Top, WIDTH, HEIGHT);
            if (value)
            {
                brush.Color = HighValt;
            }
            else if (!value)
            {
                brush.Color = LowValt;
            }
            paper.FillEllipse(brush, Left + 5, Top + 5, WIDTH - 10, HEIGHT - 10);
        }

        /// <summary>
        /// Moves the gate to the position specified.
        /// </summary>
        /// <param name="x">The x position to move the gate to</param>
        /// <param name="y">The y position to move the gate to</param>
        public override void MoveTo(int x, int y)
        {
            //Debugging message
            Console.WriteLine("pins = " + pins.Count);
            //Set the position of the gate to the values passed in
            left = x;
            top = y;
            // must move the pins too
            pins[0].X = x - xGAP;
            pins[0].Y = (y + HEIGHT / 2) + 2;
        }

        public override bool Evaluate()
        {
            if (pins[0].InputWire == null)
            {
                Console.WriteLine("No input wire connects to the gate");
                value = false;
                return false;
            }
            else
            {
                Gate gateA = pins[0].InputWire.FromPin.Owner;
                value = gateA.Evaluate();
                return gateA.Evaluate();
            }
        }

        public override Gate Clone()
        {
            Gate newGate = new OutputLamp(Left + WIDTH, Top + HEIGHT);
            return newGate;
        }
    }
}
