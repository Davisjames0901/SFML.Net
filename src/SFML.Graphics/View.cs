using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using SFML.System;

namespace SFML.Graphics
{
    /// <summary>
    /// This class defines a view (position, size, etc.) ;
    /// you can consider it as a 2D camera
    /// </summary>
    /// <remarks>
    /// See also the note on coordinates and undistorted rendering in SFML.Graphics.Transformable.
    /// </remarks>
    public class View : ObjectBase
    {
        private Transform? _transform;
        /// <summary>
        /// Create a default view (1000x1000)
        /// </summary>
        public View() :
            base(sfView_create())
        {
        }

        /// <summary>
        /// Construct the view from a rectangle
        /// </summary>
        /// <param name="viewRect">Rectangle defining the position and size of the view</param>
        public View(FloatRect viewRect) :
            base(sfView_createFromRect(viewRect))
        {
        }

        /// <summary>
        /// Construct the view from its center and size
        /// </summary>
        /// <param name="center">Center of the view</param>
        /// <param name="size">Size of the view</param>
        public View(Vector2 center, Vector2 size) :
            base(sfView_create())
        {
            Center = center;
            Size = size;
        }

        /// <summary>
        /// Construct the view from another view
        /// </summary>
        /// <param name="copy">View to copy</param>
        public View(View copy) :
            base(sfView_copy(copy.CPointer))
        {
        }

        /// <summary>
        /// Center of the view
        /// </summary>
        public Vector2 Center
        {
            get { return sfView_getCenter(CPointer); }
            set { sfView_setCenter(CPointer, value); }
        }

        /// <summary>
        /// Half-size of the view
        /// </summary>
        public Vector2 Size
        {
            get { return sfView_getSize(CPointer); }
            set { sfView_setSize(CPointer, value); }
        }

        /// <summary>
        /// Rotation of the view, in degrees
        /// </summary>
        public float Rotation
        {
            get { return sfView_getRotation(CPointer); }
            set { sfView_setRotation(CPointer, value); }
        }

        /// <summary>
        /// Target viewport of the view, defined as a factor of the
        /// size of the target to which the view is applied
        /// </summary>
        public FloatRect Viewport
        {
            get { return sfView_getViewport(CPointer); }
            set { sfView_setViewport(CPointer, value); }
        }

        /// <summary>
        /// Rebuild the view from a rectangle
        /// </summary>
        /// <param name="rectangle">Rectangle defining the position and size of the view</param>
        public void Reset(FloatRect rectangle)
        {
            sfView_reset(CPointer, rectangle);
        }

        /// <summary>
        /// Move the view
        /// </summary>
        /// <param name="offset">Offset to move the view</param>
        public void Move(Vector2 offset)
        {
            sfView_move(CPointer, offset);
        }

        /// <summary>
        /// Rotate the view
        /// </summary>
        /// <param name="angle">Angle of rotation, in degrees</param>
        public void Rotate(float angle)
        {
            sfView_rotate(CPointer, angle);
        }

        /// <summary>
        /// Resize the view rectangle to simulate a zoom / unzoom effect
        /// </summary>
        /// <param name="factor">Zoom factor to apply, relative to the current zoom</param>
        public void Zoom(float factor)
        {
            sfView_zoom(CPointer, factor);
        }

        /// <summary>
        /// Provide a string describing the object
        /// </summary>
        /// <returns>String description of the object</returns>
        public override string ToString()
        {
            return "[View]" +
                   " Center(" + Center + ")" +
                   " Size(" + Size + ")" +
                   " Rotation(" + Rotation + ")" +
                   " Viewport(" + Viewport + ")";
        }

        /// <summary>
        /// Internal constructor for other classes which need to manipulate raw views
        /// </summary>
        /// <param name="cPointer">Direct pointer to the view object in the C library</param>
        internal View(IntPtr cPointer) :
            base(cPointer)
        {
            myExternal = true;
        }

        internal Transform GetTransform()
        {
            if (_transform.HasValue)
                return _transform.Value;

            double angle = Rotation * Math.PI / 180.0f;
            float cosine = (float)Math.Cos(angle);
            float sine = (float)Math.Sin(angle);
            float tx = -Center.X * cosine - Center.Y * sine + Center.X;
            float ty =  Center.X * sine - Center.Y * cosine + Center.Y;

            // Projection components
            float a =  2.0f / Size.X;
            float b = -2.0f / Size.Y;
            float c = -a * Center.X;
            float d = -b * Center.Y;

            // Rebuild the projection matrix
            _transform = new Transform(a * cosine, a * sine, a * tx + c,
                -b * sine,   b * cosine, b * ty + d,
                0.0f,        0.0f,        1.0f);

            return _transform.Value;
        }

        /// <summary>
        /// Handle the destruction of the object
        /// </summary>
        /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
        protected override void Destroy(bool disposing)
        {
            if (!myExternal)
            {
                sfView_destroy(CPointer);
            }
        }

        private readonly bool myExternal = false;

        #region Imports
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfView_create();

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfView_createFromRect(FloatRect Rect);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfView_copy(IntPtr View);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_destroy(IntPtr View);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_setCenter(IntPtr View, Vector2 center);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_setSize(IntPtr View, Vector2 size);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_setRotation(IntPtr View, float Angle);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_setViewport(IntPtr View, FloatRect Viewport);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_reset(IntPtr View, FloatRect Rectangle);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2 sfView_getCenter(IntPtr View);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2 sfView_getSize(IntPtr View);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern float sfView_getRotation(IntPtr View);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern FloatRect sfView_getViewport(IntPtr View);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_move(IntPtr View, Vector2 offset);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_rotate(IntPtr View, float Angle);

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_zoom(IntPtr View, float Factor);
        #endregion
    }
}
