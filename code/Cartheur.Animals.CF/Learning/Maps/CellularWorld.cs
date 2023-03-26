using System.Drawing;
using System.Windows.Forms;

namespace Cartheur.Animals.CF.Learning.Maps
{
    public partial class CellularWorld : Control
    {
        private readonly Pen _blackPen = new Pen( Color.Black );
        private readonly Brush _whiteBrush = new SolidBrush( Color.White );

        private int[,] _map;
        private Color[] _coloring;

        /// <summary>
        /// World's map
        /// </summary>
        /// 
        public int[,] Map
        {
            get { return _map; }
            set
            {
                _map = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// World's coloring
        /// </summary>
        /// 
        public Color[] Coloring
        {
            get { return _coloring; }
            set
            {
                _coloring = value;
                Invalidate( );
            }
        }

        // Control's constructor
        public CellularWorld( )
        {
            InitializeComponent( );

            // update control's style (cannot use these styles in CF.
            //SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                //ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

		// Paint the control
        protected override void OnPaint( PaintEventArgs pe )
        {
            Graphics g = pe.Graphics;
            int clientWidth = ClientRectangle.Width;
            int clientHeight = ClientRectangle.Height;

            // fill with white background
            g.FillRectangle( _whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1 );

            // draw a black rectangle
            g.DrawRectangle( _blackPen, 0, 0, clientWidth - 1, clientHeight - 1 );

            if ( ( _map != null ) && ( _coloring != null ) )
            {
                int brushesCount = _coloring.Length;
                int cellWidth = clientWidth / _map.GetLength( 1 );
                int cellHeight = clientHeight / _map.GetLength( 0 );

                // create brushes
                Brush[] brushes = new Brush[brushesCount];
                for ( int i = 0; i < brushesCount; i++ )
                {
                    brushes[i] = new SolidBrush( _coloring[i] );
                }

                // draw the world
                for ( int i = 0, n = _map.GetLength( 0 ); i < n; i++ )
                {
                    int ch = ( i < n - 1 ) ? cellHeight : clientHeight - i * cellHeight - 1;

                    for ( int j = 0, k = _map.GetLength( 1 ); j < k; j++ )
                    {
                        int cw = ( j < k - 1 ) ? cellWidth : clientWidth - j * cellWidth - 1;

                        // check if we have appropriate brush
                        if ( _map[i, j] < brushesCount )
                        {
                            g.FillRectangle( brushes[_map[i, j]], j * cellWidth, i * cellHeight, cw, ch );
                            g.DrawRectangle( _blackPen, j * cellWidth, i * cellHeight, cw, ch );
                        }
                    }
                }

                // release brushes
                for ( int i = 0; i < brushesCount; i++ )
                {
                    brushes[i].Dispose( );
                }
            }

            // Calling the base class OnPaint
            base.OnPaint( pe );
        }
    }
}
