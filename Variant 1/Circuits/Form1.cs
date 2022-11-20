using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Circuits
{
    /// <summary>
    /// The main GUI for the COMP104 digital circuits editor.
    /// This has a toolbar, containing buttons called buttonAnd, buttonOr, etc.
    /// The contents of the circuit are drawn directly onto the form.
    /// 
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// The (x,y) mouse position of the last MouseDown event.
        /// </summary>
        protected int startX, startY;

        /// <summary>
        /// If this is non-null, we are inserting a wire by
        /// dragging the mouse from startPin to some output Pin.
        /// </summary>
        protected Pin startPin = null;

        /// <summary>
        /// The (x,y) position of the current gate, just before we started dragging it.
        /// </summary>
        protected int currentX, currentY;

        /// <summary>
        /// Indicate if the move has moved since the click
        /// </summary>

        /// <summary>
        /// The set of gates in the circuit
        /// </summary>
        protected List<Gate> gatesList = new List<Gate>();

        /// <summary>
        /// The set of connector wires in the circuit
        /// </summary>
        protected List<Wire> wiresList = new List<Wire>();

        /// <summary>
        /// The currently selected gate, or null if no gate is selected.
        /// </summary>
        protected Gate current = null;

        /// <summary>
        /// The new gate that is about to be inserted into the circuit
        /// </summary>
        protected Gate newGate = null;

        /// <summary>
        /// The new compound gate that is about to be created.
        /// </summary>
        protected Gate newCompound = null;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        /// <summary>
        /// Handles all events when a mouse is clicked in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (current != null)
            {
                //Check if the gate is an inputsource/outputlamp, if so toggle the gate and change the value.
                if (current.IsMouseOn(e.X, e.Y))
                {
                    if (current is InputSource)
                    {
                        InputSource inputSource = (InputSource)current;
                        inputSource.Toggle();
                        this.Invalidate();
                    }
                }
                current.Selected = false;
                if (current is Compound)
                {
                    Compound com = (Compound)current;
                    com.Selected = false;
                }
                current = null;
                this.Invalidate();

            }
            // See if we are inserting a new gate
            if (newGate != null)
            {
                newGate.MoveTo(e.X, e.Y);
                gatesList.Add(newGate);
                newGate.Selected = false;
                newGate = null;
                this.Invalidate();
            }
            else
            {
                // search for the first gate under the mouse position
                foreach (Gate g in gatesList)
                {
                    if (g.IsMouseOn(startX, startY))
                    {
                        current = g;
                        current.Selected = true;
                        if (newCompound != null)
                        {
                            Compound c = (Compound)newCompound;
                            c.AddGate(g);
                        }
                        else
                        {
                            List<Gate> CompoundGate = new List<Gate>();
                            foreach (Gate cg in gatesList)
                            {
                                if (cg is Compound)
                                {
                                    Compound c = (Compound)cg;
                                    if (c.CompoundGateList.Contains(g))
                                    {
                                        CompoundGate.Add(c);
                                    }
                                }
                            }
                            if (CompoundGate.Count > 0)
                            {
                                current = CompoundGate[CompoundGate.Count - 1];
                                CompoundGate[CompoundGate.Count - 1].Selected = true;
                            }
                        }                                 
                    }
                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles events while the mouse button is pressed down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (current == null)
            {
                // try to start adding a wire
                startPin = findPin(e.X, e.Y);
            }
            else if (current.IsMouseOn(e.X, e.Y))
            {
                // start dragging the current object around
                currentX = current.Left;
                currentY = current.Top;
            }
            startX = e.X;
            startY = e.Y;
        }

        /// <summary>
        /// Handles all events when the mouse is moving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPin != null)
            {
                Console.WriteLine("wire from " + startPin + " to " + e.X + "," + e.Y);
                currentX = e.X;
                currentY = e.Y;
                this.Invalidate();  // this will draw the line
            }
            else if (startX >= 0 && startY >= 0 && current != null)
            {
                Console.WriteLine("mouse move to " + e.X + "," + e.Y);
                current.MoveTo(e.X, e.Y);
                this.Invalidate();
            }
            else if (newGate != null)
            {
                currentX = e.X;
                currentY = e.Y;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles all events when the mouse button is released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (startPin != null)
            {
                // see if we can insert a wire
                Pin endPin = findPin(e.X, e.Y);
                if (endPin != null)
                {
                    Console.WriteLine("Trying to connect " + startPin + " to " + endPin);
                    Pin input, output;
                    if (startPin.IsOutput)
                    {
                        input = endPin;
                        output = startPin;
                    }
                    else
                    {
                        input = startPin;
                        output = endPin;
                    }
                    if (input.IsInput && output.IsOutput)
                    {
                        bool selfwire = false;
                        //Additional check for if the input and output pin is on a single gate. i.e. tried to connect to itself.
                        foreach (Gate g in gatesList)
                        {
                            //If a gate contains both the input pin & output pin, then it must had been self wired together.
                            if (g.Pins.Contains(input) && g.Pins.Contains(output))
                            {
                                selfwire = true;
                                MessageBox.Show(new ArgumentException("Error: could not conect the input to the output pin on the gate itself!").ToString());
                                break;
                            }
                        }
                        //Only link the gates, not pins on a gate!
                        if (!selfwire && input.InputWire == null)
                        {
                            Wire newWire = new Wire(output, input);
                            input.InputWire = newWire;
                            wiresList.Add(newWire);
                        }
                        else if (!selfwire)//where as an inputwire already exist for the particular pin.
                        {
                            MessageBox.Show("That input is already used.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: you must connect an output pin to an input pin.");
                    }
                }
                startPin = null;
                this.Invalidate();
            }
            // We have finished moving/dragging
            startX = -1;
            startY = -1;
            currentX = 0;
            currentY = 0;
        }

        /// <summary>
        /// This will create a new and gate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAnd_Click(object sender, EventArgs e)
        {
            newGate = new AndGate(0, 0);
        }

        private void toolStripButtonOr_Click(object sender, EventArgs e)
        {
            newGate = new OrGate(0, 0);
        }

        private void toolStripButtonNot_Click(object sender, EventArgs e)
        {
            newGate = new NotGate(0, 0);
        }

        private void toolStripButtonInputSource_Click(object sender, EventArgs e)
        {
            newGate = new InputSource(0, 0);
        }

        private void toolStripButtonOutputLamp_Click(object sender, EventArgs e)
        {
            newGate = new OutputLamp(0, 0);
        }

        private void toolStripButtonEvaluate_Click(object sender, EventArgs e)
        {
            foreach (Gate g in gatesList)
            {
                if (g is OutputLamp)
                {
                    g.Evaluate();
                }
            }
            this.Invalidate();
        }

        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            if (current != null)
            {
                Gate newG = current.Clone();
                gatesList.Add(newG);
                //If the cloned gate is a compound, then all gates present in gates list of the compound
                //must also be cloned;
                if (current is Compound)
                {
                    Compound Clone = (Compound)newG;
                    Compound Original = (Compound)current;
                    gatesList.AddRange(Clone.CompoundGateList);
                    //Wirecloning method is called, throwing the new and old gate list for binary search. 
                    WireCloning(Clone, Original);
                }
                current.Selected = false;
                current = null;
            }
            this.Invalidate();
        }

        private void toolStripButtonStartGroup_Click(object sender, EventArgs e)
        {
            newCompound = new Compound(0, 0);
        }

        private void toolStripButtonEndGroup_Click(object sender, EventArgs e)
        {
            if (newCompound != null)
            {
                newGate = newCompound;
                Compound c = (Compound)newGate;
                c.Selected = true;
                newCompound = null;
            }
            else
            {
                MessageBox.Show("Please create a new compound gate by pressing start!");
            }
            this.Invalidate();
        }
        /// <summary>
        /// Finds the pin that is close to (x,y), or returns
        /// null if there are no pins close to the position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Pin findPin(int x, int y)
        {
            foreach (Gate g in gatesList)
            {
                foreach (Pin p in g.Pins)
                {
                    if (p.isMouseOn(x, y))
                        return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Method below is used for wire cloning during cloning of compound gate. 
        /// </summary>
        /// <param name="Clone"></param>
        /// <param name="Original"></param>
        private void WireCloning(Compound Clone, Compound Original)
        {
            List<Gate> newgates = Clone.CompoundGateList;
            List<Gate> oldgates = Original.CompoundGateList;
            //Require to record the gate and pin index of each end of the wire.
            //Which will be used to make a new wire for new gates cloned. 
            List<int> FromPinGateIndex = new List<int>();
            List<int> FromPinPinIndex = new List<int>();
            List<int> toPinGateIndex = new List<int>();
            List<int> toPinPinIndex = new List<int>();
            for (int i = 0; i < wiresList.Count; i++)
            {
                for (int j = 0; j < oldgates.Count; j++)
                {
                    for (int k = 0; k < oldgates[j].Pins.Count; k++)
                    {
                        if (wiresList[i].FromPin == oldgates[j].Pins[k])
                        {
                            FromPinGateIndex.Add(j);
                            FromPinPinIndex.Add(k);
                        }
                        if (wiresList[i].ToPin == oldgates[j].Pins[k])
                        {
                            toPinGateIndex.Add(j);
                            toPinPinIndex.Add(k);
                        }
                    }
                }
            }
            for (int i = 0; i < FromPinGateIndex.Count; i++)
            {
                Wire clonewire = new Wire(newgates[FromPinGateIndex[i]].Pins[FromPinPinIndex[i]], newgates[toPinGateIndex[i]].Pins[toPinPinIndex[i]]);
                newgates[toPinGateIndex[i]].Pins[toPinPinIndex[i]].InputWire = clonewire;
                wiresList.Add(clonewire);
            }
        }
        /// <summary>
        /// Redraws all the graphics for the current circuit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Draw all of the gates
            foreach (Gate g in gatesList)
            {
                g.Draw(e.Graphics);
            }
            //Draw all of the wires
            foreach (Wire w in wiresList)
            {
                w.Draw(e.Graphics);
            }

            if (startPin != null)
            {
                e.Graphics.DrawLine(Pens.White,
                    startPin.X, startPin.Y,
                    currentX, currentY);
            }
            if (newGate != null)
            {
                // show the gate that we are dragging into the circuit
                newGate.MoveTo(currentX, currentY);
                newGate.Draw(e.Graphics);
            }
        }
    }
}
// 1. Is it a better idea to fully document the Gate class or the AndGate subclass? Can you inherit comments?
/* It would be a better idea to fully document the Gate class, because that's the supera class of all gate, and 
you would want all the children class to inherit the comments in gate class insteaded. However only XML comments
will be inherited.
*/
  

// 2. What is the advantage of making a method abstract in the superclass rather than just writing a virtual method 
//with no code in the body of the method? Is therefore any disadvantage to an abstract method?
/*
The advantage is that with abstract method, it has to be override/implmement first in the children class
in order the children class to function (Compulsary). While this isn't necessary for a virtual method. 

How in some situation this can be seen as a disadvantage. i.e. when the children class does not require 
the method, such as the evaluate method in the compound class. It still have to implemented and return
an exception when called. 
*/

// 3. If a class has an abstract method in it, does the class have to be abstract?
/*
Yes, it has to be. The other way is to use a interface instead of abstract class.
*/

// 4. What would happen in your program if one of the gates added to your compound Gate is another Compound Gate?
//Is your design robust enough to cope with this situation?

/*
When adding selection into a compound gate, only each individual gate will be considered, not the 
compound it belongs to. This way both the orignal compound and new compound gate are reserved at the
same time. 
*/
 