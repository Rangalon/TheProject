


using OpenTK;
using System;
using CiliaElements.Managers;
namespace CiliaElements
{
    public enum EState
    {
        Black,
        Blue,
        Green,
        Turquoise,
        Red,
        Violet,
        Yellow,
        White
    }

    public class TState : TSolidElement  
    {
        public TState(string iName)
        {
            this.Fi = new System.IO.FileInfo(iName);
            PartNumber = iName;
            State = EElementState.Pushed ;
            Clouds = null;
            DataColors = null;
            DataNormals = null;
            Defs = null;
            FacesIndexes = null;
            Groups = null;
            LinesIndexes = null;
            PointsIndexes = null;
            Textures = null;
            RootNode = TModelsManager.Internals.StateBox.RootNode;
            Shapes = TModelsManager.Internals.StateBox.Shapes; 
            PositionsNumber = TModelsManager.Internals.StateBox.PositionsNumber;
            handleColors = TModelsManager.Internals.StateBox.handleColors;
            handleNormals = TModelsManager.Internals.StateBox.handleNormals;
            handlePositions = TModelsManager.Internals.StateBox.handlePositions;
            LastFace = TModelsManager.Internals.StateBox.LastFace;
            FacesStart = TModelsManager.Internals.StateBox.FacesStart;
            LinesStart = TModelsManager.Internals.StateBox.LinesStart;
            PointsStart = TModelsManager.Internals.StateBox.PointsStart;
            FacesNumber = TModelsManager.Internals.StateBox.FacesNumber;
            LinesNumber = TModelsManager.Internals.StateBox.LinesNumber;
            PointsNumber = TModelsManager.Internals.StateBox.PointsNumber;
            DataPositions = TModelsManager.Internals.StateBox.DataPositions;
            DataIndices = TModelsManager.Internals.StateBox.DataIndices;
            HandleVAO = TModelsManager.Internals.StateBox.HandleVAO;
            handleIndices  = TModelsManager.Internals.StateBox.handleIndices;
            BoundingBox = TModelsManager.Internals.StateBox.BoundingBox;
        }
    }

}
