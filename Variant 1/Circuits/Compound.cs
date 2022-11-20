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
            
            int topmost = CompoundGateList[0].Top;
            int buttommost = CompoundGateList[0].Top + CompoundGateList[0].HEIGHT_Access;
            int leftmost = CompoundGateList[0].Left;
            int rightmost = CompoundGateList[0].Left + CompoundGateList[0].WIDTH_Access;
            for (int i = 0; i < CompoundGateList.Count; i++)
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
            int CenterX = (rightmost + leftmost) / 2;
            int CenterY = (buttommost + topmost) / 2;
            int changeInX = x - CenterX;
            int changeInY = y - CenterY;
            foreach (Gate g in CompoundGateList)
            {
                int newX = g.Left + changeInX;
                int newY = g.Top + changeInY;
                g.MoveTo(newX, newY);
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
                CloneList.Add(g.Clone());
            }
            Compound cloneCompound = new Compound(Left + WIDTH, Top + HEIGHT);
            cloneCompound.gates = CloneList;
            return cloneCompound;
        }

        public override bool Evaluate()
        {
            throw new NotImplementedException("Function not implmented for compound gate!");
        }

    }
}
