using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src.graphics
{
    public class Animation
    {
        /// <summary>
        /// The texture regions that make up the frames of this animation.
        /// The order of the collection should match the order the frames are to be displayed in.
        /// </summary>
        public List<TextureRegion> Frames {  get; set; }

        /// <summary>
        /// The amount of time to delay between each frame before moving to the next frame for this animiation.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Creates a new animiation.
        /// </summary>
        public Animation()
        {
            Frames = new List<TextureRegion>();
            Delay = TimeSpan.FromMilliseconds(100);
        }

        /// <summary>
        /// Creates a new animation with the specified farmes and delay.
        /// </summary>
        /// <param name="frames">An ordered collection of the frames for this animation.</param>
        /// <param name="delay">The amount of time to delay between each frame of this animation.</param>
        public Animation(List<TextureRegion> frames, TimeSpan delay)
        {
            Frames = frames;
            Delay = delay;
        }
    }
}
