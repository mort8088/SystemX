// -----------------------------------------------------------------------
// <copyright file="SpriteBatchExtended.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using SystemX.Fonts;
using SystemX.SpriteSheets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.Extensions {
    public class SpriteBatchExtended : SpriteBatch {
        private readonly I_SpriteSheetLibrary _spriteBank;
        private string _pageName = "Main";

        public SpriteBatchExtended(Game game)
            : base(game.GraphicsDevice) {
            try {
                _spriteBank = (I_SpriteSheetLibrary)game.Services.GetService(typeof(I_SpriteSheetLibrary));

                I_Fonts fontServ = (I_Fonts)game.Services.GetService(typeof(I_Fonts));

                FntButton = fontServ["xboxControllerSpriteFont"];
            }
            catch {}
        }

        public SpriteFont FntButton { get; set; }

        public SpriteSheet SpriteBank {
            get {
                return _spriteBank[_pageName];
            }
        }

        public virtual void GlifString(SpriteFont spriteFont, string text, Vector2 position, Color color) {
            if (FntButton != null) {
                Vector2 outPos = position;
                StringBuilder strBuffer = new StringBuilder(0, text.Length);
                bool buttonTxt = false;

                float butScale = spriteFont.MeasureString("|").Y / (FntButton.MeasureString("$").Y / 2);
                float offY = ((FntButton.MeasureString("$").Y * butScale) / 4);

                foreach (char letter in text) {
                    if (buttonTxt) // We are building a button string.
                    {
                        strBuffer.Append(letter);

                        if (letter == ']') {
                            string str = ChangeButtonType(strBuffer.ToString());
                            if (!string.IsNullOrEmpty(str)) {
                                outPos.Y -= offY;
                                DrawString(FntButton, str, outPos, Color.White, 0f, Vector2.Zero, butScale, SpriteEffects.None, 0f);
                                outPos.X += (FntButton.MeasureString(str).X * butScale);
                                outPos.Y += offY;
                                strBuffer.Clear();
                            }
                            buttonTxt = false;
                        }
                    } else // We are building a text string.
                    {
                        switch (letter) {
                            case '[':
                                if (!string.IsNullOrEmpty(strBuffer.ToString())) {
                                    DrawString(spriteFont, strBuffer, outPos, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                                    outPos.X += spriteFont.MeasureString(strBuffer).X;
                                    strBuffer.Clear();
                                }
                                buttonTxt = true;
                                strBuffer.Append(letter);
                                break;
                            case '\n':
                                if (!string.IsNullOrEmpty(strBuffer.ToString())) {
                                    DrawString(spriteFont, strBuffer, outPos, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                                    strBuffer.Clear();
                                }
                                outPos.X = position.X;
                                outPos.Y += spriteFont.LineSpacing;
                                break;
                            default:
                                strBuffer.Append(letter);
                                break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strBuffer.ToString()))
                    DrawString(spriteFont, strBuffer, outPos, color);
            } else DrawString(spriteFont, text, position, color);
        }

        public string ChangeButtonType(string theButton) {
            switch (theButton.ToUpper()) {
                case XButton: {
                    return "&";
                }
                case YButton: {
                    return "(";
                }
                case AButton: {
                    return "'";
                }
                case BButton: {
                    return ")";
                }
                case RightTrigger: {
                    return "+";
                }
                case LeftTrigger: {
                    return ",";
                }
                case Back: {
                    return "#";
                }
                case DPad: {
                    return "!";
                }
                case Guide: {
                    return "$";
                }
                case LeftShoulder: {
                    return "-";
                }
                case LeftThumb: {
                    return " ";
                }
                case RightShoulder: {
                    return "*";
                }
                case RightThumb: {
                    return "\"";
                }
                case Start: {
                    return "%";
                }
            }
            return "";
        }

        public void ChangePage(string newPage) {
            _pageName = newPage;
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     destination rectangle, and color. Reference page contains links to related
        //     code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     A rectangle that specifies (in screen coordinates) the destination for drawing
        //     the sprite.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(string texture, Rectangle destinationRectangle, Color color) {
            Draw(_spriteBank.Sheets[_pageName].Page, destinationRectangle, _spriteBank.Sheets[_pageName][texture].Source, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position and color. Reference page contains links to related code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(string texture, Vector2 position, Color color) {
            Draw(_spriteBank.Sheets[_pageName].Page, position, _spriteBank.Sheets[_pageName][texture].Source, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     destination rectangle, source rectangle, and color.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     A rectangle that specifies (in screen coordinates) the destination for drawing
        //     the sprite. If this rectangle is not the same size as the source rectangle,
        //     the sprite will be scaled to fit.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(string texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, destinationRectangle, tmp, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position, source rectangle, and color.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(string texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, position, tmp, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     destination rectangle, source rectangle, color, rotation, origin, effects
        //     and layer.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     A rectangle that specifies (in screen coordinates) the destination for drawing
        //     the sprite. If this rectangle is not the same size as the source rectangle,
        //     the sprite will be scaled to fit.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        //
        //   rotation:
        //     Specifies the angle (in radians) to rotate the sprite about its center.
        //
        //   origin:
        //     The sprite origin; the default is (0,0) which represents the upper-left corner.
        //
        //   effects:
        //     Effects to apply.
        //
        //   layerDepth:
        //     The depth of a layer. By default, 0 represents the front layer and 1 represents
        //     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        //     drawing.
        public void DrawSprite(string texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, destinationRectangle, tmp, color, rotation, origin, effects, layerDepth);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position, source rectangle, color, rotation, origin, scale, effects, and
        //     layer. Reference page contains links to related code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        //
        //   rotation:
        //     Specifies the angle (in radians) to rotate the sprite about its center.
        //
        //   origin:
        //     The sprite origin; the default is (0,0) which represents the upper-left corner.
        //
        //   scale:
        //     Scale factor.
        //
        //   effects:
        //     Effects to apply.
        //
        //   layerDepth:
        //     The depth of a layer. By default, 0 represents the front layer and 1 represents
        //     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        //     drawing.
        public void DrawSprite(string texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, position, tmp, color, rotation, origin, scale, effects, layerDepth);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position, source rectangle, color, rotation, origin, scale, effects and layer.
        //     Reference page contains links to related code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        //
        //   rotation:
        //     Specifies the angle (in radians) to rotate the sprite about its center.
        //
        //   origin:
        //     The sprite origin; the default is (0,0) which represents the upper-left corner.
        //
        //   scale:
        //     Scale factor.
        //
        //   effects:
        //     Effects to apply.
        //
        //   layerDepth:
        //     The depth of a layer. By default, 0 represents the front layer and 1 represents
        //     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        //     drawing.
        public void DrawSprite(string texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, position, tmp, color, rotation, origin, scale, effects, layerDepth);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     destination rectangle, and color. Reference page contains links to related
        //     code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     A rectangle that specifies (in screen coordinates) the destination for drawing
        //     the sprite.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(int texture, Rectangle destinationRectangle, Color color) {
            Draw(_spriteBank.Sheets[_pageName].Page, destinationRectangle, _spriteBank.Sheets[_pageName][texture].Source, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position and color. Reference page contains links to related code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(int texture, Vector2 position, Color color) {
            Draw(_spriteBank.Sheets[_pageName].Page, position, _spriteBank.Sheets[_pageName][texture].Source, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     destination rectangle, source rectangle, and color.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     A rectangle that specifies (in screen coordinates) the destination for drawing
        //     the sprite. If this rectangle is not the same size as the source rectangle,
        //     the sprite will be scaled to fit.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(int texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, destinationRectangle, tmp, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position, source rectangle, and color.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        public void DrawSprite(int texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, position, tmp, color);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     destination rectangle, source rectangle, color, rotation, origin, effects
        //     and layer.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   destinationRectangle:
        //     A rectangle that specifies (in screen coordinates) the destination for drawing
        //     the sprite. If this rectangle is not the same size as the source rectangle,
        //     the sprite will be scaled to fit.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        //
        //   rotation:
        //     Specifies the angle (in radians) to rotate the sprite about its center.
        //
        //   origin:
        //     The sprite origin; the default is (0,0) which represents the upper-left corner.
        //
        //   effects:
        //     Effects to apply.
        //
        //   layerDepth:
        //     The depth of a layer. By default, 0 represents the front layer and 1 represents
        //     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        //     drawing.
        public void DrawSprite(int texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, destinationRectangle, tmp, color, rotation, origin, effects, layerDepth);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position, source rectangle, color, rotation, origin, scale, effects, and
        //     layer. Reference page contains links to related code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        //
        //   rotation:
        //     Specifies the angle (in radians) to rotate the sprite about its center.
        //
        //   origin:
        //     The sprite origin; the default is (0,0) which represents the upper-left corner.
        //
        //   scale:
        //     Scale factor.
        //
        //   effects:
        //     Effects to apply.
        //
        //   layerDepth:
        //     The depth of a layer. By default, 0 represents the front layer and 1 represents
        //     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        //     drawing.
        public void DrawSprite(int texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, position, tmp, color, rotation, origin, scale, effects, layerDepth);
        }

        //
        // Summary:
        //     Adds a sprite to a batch of sprites for rendering using the specified texture,
        //     position, source rectangle, color, rotation, origin, scale, effects and layer.
        //     Reference page contains links to related code samples.
        //
        // Parameters:
        //   texture:
        //     A texture.
        //
        //   position:
        //     The location (in screen coordinates) to draw the sprite.
        //
        //   sourceRectangle:
        //     A rectangle that specifies (in texels) the source texels from a texture.
        //     Use null to draw the entire texture.
        //
        //   color:
        //     The color to tint a sprite. Use Color.White for full color with no tinting.
        //
        //   rotation:
        //     Specifies the angle (in radians) to rotate the sprite about its center.
        //
        //   origin:
        //     The sprite origin; the default is (0,0) which represents the upper-left corner.
        //
        //   scale:
        //     Scale factor.
        //
        //   effects:
        //     Effects to apply.
        //
        //   layerDepth:
        //     The depth of a layer. By default, 0 represents the front layer and 1 represents
        //     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        //     drawing.
        public void DrawSprite(int texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            Rectangle tmp;
            if (sourceRectangle.HasValue) 
                tmp = sourceRectangle.Value;
            else 
                tmp = _spriteBank.Sheets[_pageName][texture].Source;
            Draw(_spriteBank.Sheets[_pageName].Page, position, tmp, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// </summary>
        /// <param name="spriteFont"></param>
        /// <param name="text"></param>
        /// <param name="finalLoc"></param>
        /// <param name="color"></param>
        internal void DrawString(SpriteFont spriteFont, string text, Point finalLoc, Color color) {
            DrawString(spriteFont, text, new Vector2(finalLoc.X, finalLoc.Y), color);
        }

        #region consts
        private const string XButton = "[X]";
        private const string YButton = "[Y]";
        private const string AButton = "[A]";
        private const string BButton = "[B]";
        private const string RightTrigger = "[RTRIGGER]";
        private const string LeftTrigger = "[LTRIGGER]";
        private const string Back = "[BACK]";
        private const string DPad = "[DPAD]";
        private const string Guide = "[GUIDE]";
        private const string LeftShoulder = "[LSHOULDER]";
        private const string LeftThumb = "[LTHUMB]";
        private const string RightShoulder = "[RSHOULDER]";
        private const string RightThumb = "[RTHUMB]";
        private const string Start = "[START]";
        #endregion
    }
}