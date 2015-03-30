using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp; 
using LeagueSharp.Common;
using AIO.Helpers;
using SharpDX; 

namespace AIO.Common.DX
{
    public class DXButton : DXItem
    {
        /// <summary>
        ///     Internal enable variable
        /// </summary>
        private bool Enabled = false;

        /// <summary>
        ///     Validation of the DXItem, always true 
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        ///     Simple get/set for an enabled bool, on set attaches DirectX Rendering or removes it
        /// </summary>
        public override bool IsEnabled
        {
            get
            {
                return Enabled; 
            }
            set
            {
                if (value)
                {
                    Add(); 
                }
                else
                {
                    Remove(); 
                }

                Enabled = value; 
            }
        }

        /// <summary>
        ///     Allows the button to become a toggle button
        /// </summary>
        public bool CanToggle
        {
            get;
            set;
        }

        /// <summary>
        ///     Stores the variable if the button is toggled
        /// </summary>
        public bool IsToggled
        {
            get;
            set;
        }

        /// <summary>
        ///     Color for Toggled button
        /// </summary>
        public SharpDX.ColorBGRA ToggleColor
        {
            get;
            set;
        }

        /// <summary>
        ///     Default button color
        /// </summary>
        public SharpDX.ColorBGRA DefaultColor
        {
            get;
            set;
        }

        /// <summary>
        ///     Drawing#Size of text 
        /// </summary>
        public System.Drawing.Size ContentSize
        {
            get
            {
                return Drawing.GetTextExtent(TextUpdate());
            }
        }
        
        /// <summary>
        ///     Content rectangle position
        /// </summary>
        public override Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public delegate void OnClickH(Vector2 v);
        public delegate void OnHoverH(Vector2 v);
        public delegate string TextUpdateH(); 

        /// <summary>
        ///     Text retrieval delegate for live text
        /// </summary>
        public TextUpdateH TextUpdate;

        /// <summary>
        ///     On click event
        /// </summary>
        public event OnClickH OnClick;

        /// <summary>
        ///     On hover event
        /// </summary>
        public event OnHoverH OnHover; 
        
        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int X, Y, Width, Height;
        
        private Render.Rectangle ContentRectangle;
        private Render.Text RenderText;
        private Vector2 _Position; 

        /// <summary>
        ///     A DirectX rendered button 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="TextUpdate"></param>
        /// <param name="CanToggle"></param>
        /// <param name="IsToggled"></param>
        public DXButton(int X, int Y, int Width, int Height, TextUpdateH TextUpdate, bool CanToggle = false, bool IsToggled = false)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.TextUpdate = TextUpdate;
            this.CanToggle = CanToggle;
            this.IsToggled = IsToggled; 


            ContentRectangle = new Render.Rectangle(X, Y, Width, Height, DefaultColor);
            Position = new Vector2(X, Y); 

            var size = ContentSize; 
            RenderText = new Render.Text("", new Vector2(0, 0), 0, new ColorBGRA(0));
            RenderText.TextUpdate = () => { return TextUpdate(); };
            RenderText.PositionUpdate = () => { return new Vector2((ContentRectangle.Width - size.Width) / 2, (ContentRectangle.Height - size.Height) / 2); };


            OnClick += (Vector2 v) =>
            {
                if (CanToggle)
                {
                    IsToggled = !IsToggled;
                    ContentRectangle.Color = IsToggled ? ToggleColor : DefaultColor; 
                }
            };

            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnUpdate;
        }

        /// <summary>
        ///     Deconstructor
        /// </summary>
        ~DXButton()
        {
            ContentRectangle.Remove();
            RenderText.Remove(); 
            Game.OnWndProc -= Game_OnWndProc;
            Game.OnUpdate -= Game_OnUpdate; 
        }

        public override void Add()
        {
            ContentRectangle.Add();
            RenderText.Add();
        }

        public override void Remove()
        {
            ContentRectangle.Remove();
            RenderText.Remove(); 
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (ContentRectangle.Contains(Utils.GetCursorPos()))
            {
                if (OnHover != null)
                {
                    OnHover(Utils.GetCursorPos()); 
                }
            }
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDBLCLCK)
            {
                return; 
            }

            if (ContentRectangle.Contains(Utils.GetCursorPos()))
            {
                if (OnClick != null)
                {
                    OnClick(Utils.GetCursorPos()); 
                }
            }
        }
    }
}
