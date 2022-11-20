using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Circuits
{
    public abstract class Gate
    {
        // left is the left-hand edge of the main part of the gate.
        // So the input pins are further left than left.
        protected int left;//Reserve

        // top is the top of the whole gate
        protected int top;//Reserve

        // width and height of the main part of the gate
        protected int WIDTH;//Reserve
        protected int HEIGHT;//Reserve
        // length of the connector legs sticking out left and right
        protected Color SelectedColor = Color.Red;
        protected Color DeseletedColor = Color.Black;
        /// <summary>
        /// This is the list of all the pins of this gate.
        /// An AND gate always has two input pins (0 and 1)
        /// and one output pin (number 2).
        /// </summary>
        protected List<Pin> pins = new List<Pin>();//Reserve
        //Has the gate been selected
        protected bool selected = false;

        /// <summary>
        /// Initialises the Gate.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        protected Gate(int x, int y, int gatewidth, int gateheight)
        {
            left = x;
            top = y;
            //Allows for different height and width depends on the gate type. 
            WIDTH = gatewidth;
            HEIGHT = gateheight;
            //move the gate and the pins to the position passed in
        }

        /// <summary>
        /// Gets and sets whether the gate is selected or not.
        /// </summary>
        public virtual bool Selected//Reserve
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// Gets the left hand edge of the gate.
        /// </summary>
        public int Left//Reserve
        {
            get { return left; }
        }

        /// <summary>
        /// Gets the top edge of the gate.
        /// </summary>
        public int Top//Reserve
        {
            get { return top; }
        }

        /// <summary>
        /// Return the width of the gate
        /// </summary>
        public int WIDTH_Access
        {
            get { return WIDTH; }
        }

        /// <summary>
        /// Return the height of the gate
        /// </summary>
        public int HEIGHT_Access
        {
            get { return HEIGHT; }
        }
        /// <summary>
        /// Gets the list of pins for the gate.
        /// </summary>
        public List<Pin> Pins//Reserve
        {
            get { return pins; }
        }

        /// <summary>
        /// Checks if the gate has been clicked on.
        /// </summary>
        /// <param name="x">The x position of the mouse click</param>
        /// <param name="y">The y position of the mouse click</param>
        /// <returns>True if the mouse click position is inside the gate</returns>
        public virtual bool IsMouseOn(int x, int y)//Reserve
        {
            if (left <= x && x < left + WIDTH
                && top <= y && y < top + HEIGHT)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Draws the gate in the normal colour or in the selected colour.
        /// </summary>
        /// <param name="paper"></param>
        public abstract void Draw(Graphics paper);//Abstract 

        /// <summary>
        /// Moves the gate to the position specified.
        /// </summary>
        /// <param name="x">The x position to move the gate to</param>
        /// <param name="y">The y position to move the gate to</param>
        public abstract void MoveTo(int x, int y);//Abstract 

        /// <summary>
        /// Evaluate the output value depending on the current input value from wires. 
        /// </summary>
        /// <returns>Return output value as boolean</returns>
        public abstract bool Evaluate();

        /// <summary>
        /// Making a clone of the gate iteself.
        /// </summary>
        /// <returns></returns>
        public abstract Gate Clone();


    }
}
