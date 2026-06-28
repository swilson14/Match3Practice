using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3Practice.src.graphics
{
    /// <summary>
    /// Represents a rectangular region within a texture.
    /// </summary>
    public class TextureRegion
    {
        /// <summary>
        /// Gets or sets the source texture this region is a part of.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the source rectangle boundary of this texture region within the source texture.
        /// </summary>
        public Rectangle SourceRectangle { get; set; }

        /// <summary>
        /// Gets the Width, in pixels of this texture region.
        /// </summary>
        public int Width => SourceRectangle.Width;

        /// <summary>
        /// Gets the Height, in pixels of this texture region.
        /// </summary>
        public int Height => SourceRectangle.Height;

        /// <summary>
        /// Gets the top normalized texture coordinate of this region.
        /// </summary>
        public float TopTextureCoordinate => SourceRectangle.Top / (float)Texture.Height;

        /// <summary>
        /// Gets the bottom normalized texture coordinate of this region.
        /// </summary>
        public float BottomTextureCoordinate => SourceRectangle.Bottom / (float)Texture.Height;

        /// <summary>
        /// Gets the left normalized texture coordinate of this region.
        /// </summary>
        public float LeftTextureCoordinate => SourceRectangle.Left / (float)Texture.Width;

        /// <summary>
        /// Gets the right normalized texture coordinate of this region.
        /// </summary>
        public float RightTextureCoordinate => SourceRectangle.Right / (float)Texture.Width;


        /// <summary>
        /// Creates a new texture region.
        /// </summary>
        public TextureRegion() { }

        /// <summary>
        /// Creates a new texture region using the specified source texture.
        /// </summary>
        /// <param name="texture"> The texture to use as the source texture for this texture region.</param>
        /// <param name="x">The x-coordinate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
        /// <param name="y">The y-coordinate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
        /// <param name="width">The width, in pixels of this texture region.</param>
        /// <param name="height">The height, in pixels of this texture region.</param>
        public TextureRegion(Texture2D texture, int x, int y, int width, int height)
        {
            Texture = texture;
            SourceRectangle = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Submit this texture region for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used fot batching draw calls.</param>
        /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
        /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Draw(spriteBatch, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// ubmit this texture region for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used fot batching draw calls.</param>
        /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
        /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
        /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region on screen.</param>
        /// <param name="origin">The center of rotation, scaling and position when drawing this texture region on screen.</param>
        /// <param name="scale">The scale factor to apply when drawing this texture region on screen.</param>
        /// <param name="effects">Specifies if this texture region should be flipped horizonatally, vertically or both when drawing on screen.</param>
        /// <param name="layerDepth">The depth of the layer to use when drawing this texture region on screen.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            Draw(spriteBatch, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }

        /// <summary>
        /// Submit this texture region for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used fot batching draw calls.</param>
        /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
        /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
        /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region on screen.</param>
        /// <param name="origin">The center of rotation, scaling and position when drawing this texture region on screen.</param>
        /// <param name="scale">The scale factor to apply when drawing this texture region on screen.</param>
        /// <param name="effects">Specifies if this texture region should be flipped horizonatally, vertically or both when drawing on screen.</param>
        /// <param name="layerDepth">The depth of the layer to use when drawing this texture region on screen.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(Texture, position, SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

    }
}
