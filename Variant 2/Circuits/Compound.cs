using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Circuits
{
    internal class Compound : Gate
    {
        /// <summary>
        /// A List to store all gates contained in compound.
        /// </summary>
        private List<Gate> gates;
        /// <summary>
        /// Initialises the Gate.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        public Compound(int x, int y) : base(x, y, 40, 40)
        {
            gates = new List<Gate>();
            Selected = false;
        }

        /// <summary>
        /// Allow read only access of the gates contained in other class,
        /// For comparision purpose.
        /// </summary>
        public List<Gate> CompoundGateList
        {
            get { return gates; }
        }

        /// <summary>
        /// Property set all selected in the list belong to this compound gate to ture/false.
        /// </summary>
        public override bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                foreach (Gate g in gates)
                {
                    g.Selected = value;
                }
            }
        }

        /// <summary>
        /// Add a new gate into the compound gate list.
        /// </summary>
        /// <param name="g"></param>
        public void AddGate(Gate g)
        {
            gates.Add(g);
            if (g is Compound)
            {
                Compound g2 = (Compound)g;
                CompoundGateList.AddRange(g2.CompoundGateList);
            }
        }

        public override void Draw(Graphics paper)
        {
            foreach (Gate g in CompoundGateList)
            {
                g.Draw(paper);
            }
        }

        public override void MoveTo(int x, int y)
        {
            int topmost = 0;
            int buttommost = 0;
            int leftmost = 0;
            int rightmost = 0;
            //Set the top most, buttommost, leftmost, and rightmost to the first non-compound gate.
            foreach (Gate g in CompoundGateList)
            {
                if (!(g is Compound))
                {
                    topmost = g.Top;
                    buttommost = g.Top + g.HEIGHT_Access;
                    leftmost = g.Left;
                    rightmost = g.Left + g.WIDTH_Access;
                    break;
                }
            }           
            for (int i = 0; i < CompoundGateList.Count; i++)
            {
               //Do not consider the compound gate contained in the list, only other gates contains. 
               if (!(CompoundGateList[i] is Compound))
               {
                    if (CompoundGateList[i].Top < topmost)
                    {
                        topmost = CompoundGateList[i].Top;
                    }
                    if (CompoundGateList[i].Left < leftmost)
                    {
                        leftmost = CompoundGateList[i].Left;
                    }
                    if (CompoundGateList[i].Top + CompoundGateList[i].HEIGHT_Access > buttommost)
                    {
                        buttommost = CompoundGateList[i].Top + CompoundGateList[i].HEIGHT_Access;
                    }
                    if (CompoundGateList[i].Left + CompoundGateList[i].HEIGHT_Access > rightmost)
                    {
                        rightmost = CompoundGateList[i].Left + CompoundGateList[i].WIDTH_Access;
                    }
               }                                                                                   
            }
            int CenterX = (rightmost + leftmost) / 2;
            int CenterY = (buttommost + topmost) / 2;
            int changeInX = x - CenterX;
            int changeInY = y - CenterY;
            foreach (Gate g in CompoundGateList)
            {     
                if (!(g is Compound))
                {
                    int newX = g.Left + changeInX;
                    int newY = g.Top + changeInY;
                    g.MoveTo(newX, newY);
                }
            }
        }

        public override bool IsMouseOn(int x, int y)
        {
            bool check = false;
            foreach (Gate g in gates)
            {
                if (left <= x && x < left + WIDTH && top <= y && y < top + HEIGHT)
                {
                    check = true;
                    break;
                }
                else
                {
                    check = false;
                }
            }
            return check;
        }

        public override Gate Clone()
        {
            List<Gate> CloneList = new List<Gate>();
            foreach (Gate g in CompoundGateList)
            {
                if (!(g is Compound))
                {
                    CloneList.Add(g.Clone());
                }               
            }
            Compound cloneCompound = new Compound(Left + WIDTH, Top + HEIGHT);
            cloneCompound.gates = CloneList;
            return cloneCompound;
        }

        /// <summary>
        /// Method below is used for wire cloning during cloning of compound gate. 
        /// </summary>
        /// <param name="Clone"></param>
        /// <param name="Original"></param>
        public static void WireCloning(Compound Clone, Compound Original, ref List<Wire> wiresList)
        {
            List<Gate> newgates = Clone.CompoundGateList;
            List<Gate> oldgates = new List<Gate>(Original.CompoundGateList);
            foreach (Gate g in Clone.CompoundGateList)
            {
                if (g is Compound)
                {
                    newgates.Remove(g);
                }
            }
            foreach (Gate g in Original.CompoundGateList)
            {
                if (g is Compound)
                {
                    oldgates.Remove(g);
                }
            }
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
                        if (oldgates.Contains(wiresList[i].ToPin.Owner) && oldgates.Contains(wiresList[i].FromPin.Owner))
                        {
                            if ((wiresList[i].FromPin == oldgates[j].Pins[k]))
                            {
                                FromPinGateIndex.Add(j);
                                FromPinPinIndex.Add(k);
                            }
                            if ((wiresList[i].ToPin == oldgates[j].Pins[k]))
                            {
                                toPinGateIndex.Add(j);
                                toPinPinIndex.Add(k);
                            }
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


        public static Compound RecursiveReturnCompound(Gate g, List<Gate> gatesList)
        {
            Compound finalgate = null;
            bool check = false;
            foreach (Gate cg in gatesList)
            {
                if (cg is Compound)
                { 
                    Compound c = (Compound)cg;
                    if (c.CompoundGateList.Contains(g))
                    {
                        finalgate = RecursiveReturnCompound(c, gatesList);
                        return finalgate;
                    }
                    else if (!c.CompoundGateList.Contains(g))
                    {
                        check = false;
                    }
                }
            }
            if (check == false && g is Compound)
            {
                finalgate = (Compound)g;
            }
            return finalgate;
        }

        public override bool Evaluate()
        {
            throw new NotImplementedException("Function not implmented for compound gate!");
        }



    }
}
