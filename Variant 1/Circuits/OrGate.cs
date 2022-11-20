using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Circuits
{
    /// <summary>
    /// This class implements an Or gate with two inputs
    /// and one output.
    /// </summary>
    internal class OrGate : Gate
    {
        private const int xGAP = 2;
        private const int yGAP = 10;
        /// <summary>
        /// Initialises the Gate.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        public OrGate(int x, int y) : base(x, y, 40, 40)
        {
            //Add the two input pins to the gate
            pins.Add(new Pin(this, true, 20));
            pins.Add(new Pin(this, true, 20));
            //Add the output pin to the gate
            pins.Add(new Pin(this, false, 20));
            //move the gate and the pins to the position passed in
            MoveTo(x, y);
        }

        /// <summary>
        /// Draws the gate in the normal colour or in the selected colour.
        /// </summary>
        /// <param name="paper"></param>
        public override void Draw(Graphics paper)
        {
            //Drawing each pin
            foreach (Pin p in pins)
                p.Draw(paper);

            //Check if the gate has been selected
            if (selected)
            {
                paper.DrawImage(Properties.Resources.OrGateAllRed, Left, Top);
            }
            else
            {
                paper.DrawImage(Properties.Resources.OrGate, Left, Top);
            }
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
            pins[0].Y = y + yGAP;
            pins[1].X = x - xGAP;
            pins[1].Y = y + HEIGHT - yGAP + 12;
            pins[2].X = x + WIDTH + xGAP + 33;
            pins[2].Y = (y + HEIGHT / 2) + 6;
        }

        public override bool Evaluate()
        {
            if (pins[0].InputWire == null || pins[1].InputWire == null)
            {
                Console.WriteLine("No input wire goes into the pins.");
                return false;
            }
            else
            {
                Gate gateA = pins[0].InputWire.FromPin.Owner;
                Gate gateB = pins[1].InputWire.FromPin.Owner;
                return gateA.Evaluate() || gateB.Evaluate();
            }
        }

        public override Gate Clone()
        {
            Gate newGate = new OrGate(Left + WIDTH, Top + HEIGHT);
            return newGate;
        }
    }
}
