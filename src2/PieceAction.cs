namespace nl.ChessOnline2
{
    public abstract class PieceAction
    {
        public abstract void Next();
        public abstract void Undo();
    }

    public class PieceMoveAction : PieceAction
    {
        public override void Next()
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PieceCatchAction : PieceAction
    {
        public PieceCatchAction()
        {

        }
        
        public override void Next()
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}