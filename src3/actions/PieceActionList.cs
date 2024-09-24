using System.Collections.Generic;

namespace nl.ChessOnline3
{
    public class PieceActionList : List<PieceAction>
    {
        public IntVector2 reperesentPosition;
        public bool isTestAction;

        public PieceActionList(IntVector2 _representPosition, int _capacity, bool _isTestAction)
        : base(_capacity)
        {
            reperesentPosition = _representPosition;
            isTestAction = _isTestAction;
        }

        public void Next()
        {
            for(int i = 0; i < base.Count; ++i)
            {
                base[i].isTestAction = this.isTestAction;
                base[i].Next();
            }
        }

        public void Undo()
        {
            for(int i = base.Count - 1; i >= 0; --i)
            {
                base[i].isTestAction = this.isTestAction;
                base[i].Undo();
            }
        }

        public override string ToString()
        {
            return string.Format("rep position == ({0}, {1})", reperesentPosition.x, reperesentPosition.y);
        }
    }
}