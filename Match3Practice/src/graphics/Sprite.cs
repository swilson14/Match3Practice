using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3Practice.src.graphics
{
    public class Sprite
    {
        /// <summary>
        /// Gets or sets the source texture region represented by this sprite.
        /// </summary>
        public TextureRegion Region {  get; set; }

        /// <summary>
        /// Gets or sets the color mas to apply when rendering this sprite.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the amount of rotation, in radians to apply when rendering this sprite.
        /// </summary>
        public float Rotation { get; set; } = 0.0f;

        /// <summary>
        /// Gets or sets the scale factor to apply to the x and y axes when rending this sprite.
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// Gets or sets the xy-coordinate origion point, relative to the top-left corner of this sprite.
        /// </summary>
        public Vector2 Origin {  get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets or sets the sprite effects to apply when rendering this sprite.
        /// </summary>
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Gets or sets the layer depth to apply when rendering this sprite.
        /// </summary>
        public float LayerDepth { get; set; } = 0.0f;

        /// <summary>
        /// Gets the width in pixels, of this sprite.
        /// </summary>
        public float Width => Region.Width * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, of this sprite.
        /// </summary>
        public float Height => Region.Height * Scale.Y;

        /// <summary>
        /// Creates a new sprite.
        /// </summary>
        public Sprite() { }

        /// <summary>
        /// Creates a new sprite using the specified source texture region.
        /// </summary>
        public Sprite(TextureRegion region)
        {
            Region = region;
        }

        /// <summary>
        /// Sets the origin of this sprite to the center. 
        /// </summary>
        public void CenterOrigin()
        {
            Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
        }

        /// <summary>
        /// Submit this sprite for drawing to the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used for batching draw calls.</param>
        /// <param name="position">The xy-coordinate position to render this sprite at.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position) 
        {
            Region.Draw(spriteBatch, position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
        }

    }
}
