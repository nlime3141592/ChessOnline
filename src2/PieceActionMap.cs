namespace nl.ChessOnline2
{
    public class PieceActionMap
    {
        public int[,,] actionMap;

        public PieceActionMap()
        {
            actionMap = new int[8, 8, 17];
        }

        public PieceActionMap(PieceActionMap _src)
        {
            actionMap = new int[8, 8, 17];

            for(int x = 0; x < 8; ++x)
            for(int y = 0; y < 8; ++y)
            for(int c = 0; c < _src.actionMap[x, y, 0]; ++c)
            {
                actionMap[x, y, c] = _src.actionMap[x, y, c];
            }
        }

        public void Add(int _x, int _y, int _idxPieceData)
        {
            actionMap[_x, _y, actionMap[_x, _y, 0]++] = _idxPieceData;
        }
    }
}