using System.Collections.Generic;

namespace nl.ChessOnline3
{
    public class PieceActionList : List<PieceAction>
    {
        public bool isTestAction;

        public PieceActionList(int _capacity, bool _isTestAction)
        : base(_capacity)
        {
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
    }
}