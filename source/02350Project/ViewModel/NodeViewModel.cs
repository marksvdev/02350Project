﻿using _02350Project.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _02350Project.ViewModel
{
    public class NodeViewModel : ViewModelBase
    {
        #region Fields and Properties
        private Node node;

        public int Id { get { return node.Id; } set { node.Id = value; } }

        private int orientation;
        public int Orientation { get { return orientation; } set { orientation = value; } }

        /*
         * Coordinates and Dimensions
         */
        private double height;
        private double width;

        public double X { get { return node.X; } set { node.X = (int)value; RaisePropertyChanged("X"); } }
        public double Y { get { return node.Y; } set { node.Y = (int)value; RaisePropertyChanged("Y"); } }
        public double Height { get { return height; } set { height = value; RaisePropertyChanged("Height"); } }
        public double Width { get { return width; } set { width = value; RaisePropertyChanged("Width"); } }

        public double CanvasCenterX { get { return node.X + Width / 2; } set { node.X = (int)value - (int)Width / 2; RaisePropertyChanged("X"); } }
        public double CanvasCenterY { get { return node.Y + Height / 2; } set { node.Y = (int)value - (int)Height / 2; RaisePropertyChanged("Y"); } }
        public double MaxX { get { return node.X + Width; } }
        public double MaxY { get { return node.Y + Height; } }

        /*
         * Edge Anchor Points
         */
        private Point north;
        private Point south;
        private Point east;
        private Point west;

        public Point North { get { north.X = node.X + Width / 2; north.Y = node.Y; return north; } set { north.X = node.X + Width / 2; north.Y = node.Y; RaisePropertyChanged("North"); } }
        public Point South { get { south.X = node.X + Width / 2; south.Y = node.Y + Height; return south; } set { south.X = node.X + Width / 2; south.Y = node.Y + Height; RaisePropertyChanged("South"); } }
        public Point East { get { east.X = node.X + Width; east.Y = node.Y + Height / 2; return east; } set { east.X = node.X + Width; east.Y = node.Y + Height / 2; RaisePropertyChanged("East"); } }
        public Point West { get { west.X = node.X; west.Y = node.Y + Height / 2; return west; } set { west.X = node.X; west.Y = node.Y + Height / 2; RaisePropertyChanged("West"); } }

        /*
         * Content
         */
        private string nodeSubText;

        public string Name { get { return node.Name; } set { node.Name = value; RaisePropertyChanged("Name"); } }
        public string NodeSubText
        {
            get
            {
                switch (NodeType)
                {
                    case NodeType.ABSTRACT:
                        nodeSubText = "Abstract";
                        break;
                    case NodeType.INTERFACE:
                        nodeSubText = "Interface";
                        break;
                    case NodeType.CLASS:
                        nodeSubText = "Class";
                        break;
                }
                return nodeSubText;
            }
        }
        public NodeType NodeType { get { return node.NodeType; } set { node.NodeType = value; RaisePropertyChanged("NodeType"); RaisePropertyChanged("NodeSubText"); } }
        public List<string> Attributes { get { return node.Attributes; } set { node.Attributes = value; RaisePropertyChanged("Attributes"); } }
        public List<string> Methods { get { return node.Methods; } set { node.Methods = value; RaisePropertyChanged("Methods"); } }

        /*
         * Visuals
         */
        private bool nodeCollapsed;
        private bool attCollapsed;
        private bool metCollapsed;
        private bool isSelected;

        public bool NodeCollapsed { get { return nodeCollapsed; } set { nodeCollapsed = value; RaisePropertyChanged("NodeCollapsed"); } }
        public bool AttCollapsed { get { return attCollapsed; } set { attCollapsed = value; RaisePropertyChanged("AttCollapsed"); } }
        public bool MetCollapsed { get { return metCollapsed; } set { metCollapsed = value; RaisePropertyChanged("MetCollapsed"); } }
        public bool IsSelected { get { return isSelected; } set { isSelected = value; RaisePropertyChanged("IsSelected"); RaisePropertyChanged("Opacity"); } }
        public double Opacity { get { return IsSelected ? 0.7 : 1; } }
        #endregion

        /// <summary>
        /// Returns a new EdgeViewModel with 'this' NodeViewModel and 'this' Node as ending Node, 
        /// fromNode as starting node and type as the edge type.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public EdgeViewModel newEdge(NodeViewModel fromNode, string type)
        {
            EdgeViewModel newEdge = new EdgeViewModel(fromNode.node, this.node, fromNode, this, type);
            return newEdge;
        }

        #region Constructor
        public NodeViewModel(int id, Node node)
        {
            this.node = node;
            Id = id;
            NodeCollapsed = true;
            AttCollapsed = true;
            MetCollapsed = true;
        }
        #endregion

        #region Anchorpoint Calculations
        public enum ANCHOR { NORTH, SOUTH, EAST, WEST };
        private double northEast = -1.0 * Math.PI / 4.0;
        private double northWest = -3.0 * Math.PI / 4.0;
        private double southEast = Math.PI / 4.0;
        private double southWest = 3.0 * Math.PI / 4.0;

        public void setEnds(EdgeViewModel e)
        {
            ANCHOR A = findAnchor(e.VMEndA.CanvasCenterX, e.VMEndA.CanvasCenterY, e.VMEndB.CanvasCenterX, e.VMEndB.CanvasCenterY);
            ANCHOR B = findAnchor(e.VMEndB.CanvasCenterX, e.VMEndB.CanvasCenterY, e.VMEndA.CanvasCenterX, e.VMEndA.CanvasCenterY);
            switch (A)
            {
                case ANCHOR.NORTH:
                    e.AX = e.VMEndA.North.X;
                    e.AY = e.VMEndA.North.Y;
                    Orientation = 0;
                    break;
                case ANCHOR.EAST:
                    e.AX = e.VMEndA.East.X;
                    e.AY = e.VMEndA.East.Y;
                    Orientation = 1;
                    break;
                case ANCHOR.SOUTH:
                    e.AX = e.VMEndA.South.X;
                    e.AY = e.VMEndA.South.Y;
                    Orientation = 0;
                    break;
                case ANCHOR.WEST:
                    e.AX = e.VMEndA.West.X;
                    e.AY = e.VMEndA.West.Y;
                    Orientation = 1;
                    break;
            }
            switch (B)
            {
                case ANCHOR.NORTH:
                    e.BX = e.VMEndB.North.X;
                    e.BY = e.VMEndB.North.Y;
                    Orientation = 0;
                    break;
                case ANCHOR.EAST:
                    e.BX = e.VMEndB.East.X;
                    e.BY = e.VMEndB.East.Y;
                    Orientation = 1;

                    break;
                case ANCHOR.SOUTH:
                    e.BX = e.VMEndB.South.X;
                    e.BY = e.VMEndB.South.Y;
                    Orientation = 0;

                    break;
                case ANCHOR.WEST:
                    e.BX = e.VMEndB.West.X;
                    e.BY = e.VMEndB.West.Y;
                    Orientation = 1;
                    break;
            }
        }

        private ANCHOR findAnchor(double x1, double y1, double x2, double y2)
        {
            double deltaX = x2 - x1;
            double deltaY = y2 - y1;


            double angle = Math.Atan2(deltaY, deltaX);


            if (northEast > angle && angle >= northWest)
            {
                return ANCHOR.NORTH;

            }
            else if (southWest > angle && angle >= southEast)
            {
                return ANCHOR.SOUTH;

            }
            else if (southEast > angle && angle >= northEast)
            {
                return ANCHOR.EAST;

            }
            else if (northWest > angle || angle >= southWest)
            {
                return ANCHOR.WEST;

            }
            return ANCHOR.NORTH;
        }

        /// <summary>
        /// Gets the Node for the ViewModel. Should only be used for serializing.
        /// </summary>
        /// <returns></returns>
        public Node getNode() { return node; }

        #endregion

    }
}
