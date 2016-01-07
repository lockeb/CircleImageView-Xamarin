/* 
 * Xamarin C# Port of hdodenhof/CircleImageView
 * Created by Bradley Locke May 2015 - brad.locke@gmail.com
 * https://github.com/blocke79/CircleImageView.Xamarin
 * 
 * 
 * Originally Java version created by hdodenhof
 * https://github.com/hdodenhof/CircleImageView
 * 
 */

using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Uri = Android.Net.Uri;

namespace de.hdodenhof.circleimageview
{
    public class CircleImageView : ImageView
    {
        private const int ColordrawableDimension = 2;

        private const int DefaultBorderWidth = 0;
        private const bool DefaultBorderOverlay = false;
		private static readonly ScaleType SCALE_TYPE = ScaleType.CenterCrop;

        private static readonly Bitmap.Config BitmapConfig = Bitmap.Config.Argb8888;
        private static readonly int DefaultBorderColor = Color.Black;
        private static readonly int DefaultFillColor = Color.Transparent;
        private readonly Paint _mBitmapPaint = new Paint();
        private readonly Paint _mBorderPaint = new Paint();
        private readonly RectF _mBorderRect = new RectF();

        private readonly RectF _mDrawableRect = new RectF();
        private readonly Paint _mFillPaint = new Paint();

        private readonly Matrix _mShaderMatrix = new Matrix();

        private Bitmap _mBitmap;
        private int _mBitmapHeight;
        private BitmapShader _mBitmapShader;
        private int _mBitmapWidth;

        private int _mBorderColor = DefaultBorderColor;
        private bool _mBorderOverlay;
        private float _mBorderRadius;
        private int _mBorderWidth = DefaultBorderWidth;

        private ColorFilter _mColorFilter;

        private float _mDrawableRadius;
        private int _mFillColor = DefaultFillColor;

        private bool _mReady;
        private bool _mSetupPending;

		protected CircleImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CircleImageView(Context context) : base(context)
        {
            Init();
        }

        public CircleImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
			Init();
        }

        public CircleImageView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircleImageView, defStyleAttr, 0);

            _mBorderWidth = a.GetDimensionPixelSize(Resource.Styleable.CircleImageView_civ_border_width,
                DefaultBorderWidth);
            _mBorderColor = a.GetColor(Resource.Styleable.CircleImageView_civ_border_color, DefaultBorderColor);
            _mBorderOverlay = a.GetBoolean(Resource.Styleable.CircleImageView_civ_border_overlay, DefaultBorderOverlay);
            _mFillColor = a.GetColor(Resource.Styleable.CircleImageView_civ_fill_color, DefaultFillColor);

            a.Recycle();
            Init();
        }

        public CircleImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        private void Init()
        {
            base.SetScaleType(SCALE_TYPE);
            _mReady = true;

            if (!_mSetupPending) return;
            Setup();
            _mSetupPending = false;
        }

        public override ScaleType GetScaleType()
        {
            base.GetScaleType();
            return SCALE_TYPE;
        }

        public override void SetScaleType(ScaleType scaleType)
        {
            {
                if (scaleType != SCALE_TYPE)
                {
                    throw new ArgumentException(string.Format("ScaleType {0} not supported.", scaleType));
                }
            }
        }

        public override void SetAdjustViewBounds(bool adjustViewBounds)
        {
            {
                if (adjustViewBounds)
                {
                    throw new ArgumentException("adjustViewBounds not supported.");
                }
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (_mBitmap == null)
            {
                return;
            }

            if (_mFillColor != Color.Transparent)
            {
                canvas.DrawCircle(Width/2.0f, Height/2.0f, _mDrawableRadius, _mFillPaint);
            }
            canvas.DrawCircle(Width/2.0f, Height/2.0f, _mDrawableRadius, _mBitmapPaint);
            if (_mBorderWidth != 0)
            {
                canvas.DrawCircle(Width/2.0f, Height/2.0f, _mBorderRadius, _mBorderPaint);
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            Setup();
        }

        public int BorderColor()
        {
            return _mBorderColor;
        }

        public void SetBorderColor(int borderColor)
        {
            {
                if (borderColor == _mBorderColor)
                {
                    return;
                }

                _mBorderColor = borderColor;
                _mBorderPaint.Color = new Color(_mBorderColor);
                Invalidate();
            }
        }

        public void SetFillColor(int fillColor)
        {
            {
                if (fillColor == _mFillColor)
                {
                    return;
                }

                _mFillColor = fillColor;
                _mFillPaint.Color = new Color(_mFillColor);
                Invalidate();
            }
        }


        public void SetFillColorResource(int fillColorResource)
        {
            {
                SetFillColor(Context.Resources.GetColor(fillColorResource));
            }
        }

        public void SetBorderWidth(int borderWidth)
        {
            {
                if (borderWidth == _mBorderWidth)
                {
                    return;
                }

                _mBorderWidth = borderWidth;
                Setup();
            }
        }

        public bool BorderOverlay()
        {
            return _mBorderOverlay;
        }


        public void SetBorderOverlay(bool borderOverlay)
        {
            {
                if (borderOverlay == _mBorderOverlay)
                {
                    return;
                }

                _mBorderOverlay = borderOverlay;
                Setup();
            }
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            {
                base.SetImageBitmap(bm);
                _mBitmap = bm;
                Setup();
            }
        }

        public override void SetImageDrawable(Drawable drawable)
        {
            {
                base.SetImageDrawable(drawable);
                _mBitmap = GetBitmapFromDrawable(drawable);
                Setup();
            }
        }

        public override void SetImageResource(int resId)
        {
            {
                base.SetImageResource(resId);
                _mBitmap = GetBitmapFromDrawable(Resources.GetDrawable(resId));
                Setup();
            }
        }

        public override void SetImageURI(Uri uri)
        {
            base.SetImageURI(uri);
            var stream = Application.Context.ContentResolver.OpenInputStream(uri);
            var drawable = Drawable.CreateFromStream(stream, uri.ToString());
            _mBitmap = GetBitmapFromDrawable(drawable);
            Setup();
        }

        public override void SetColorFilter(ColorFilter cf)
        {
            base.SetColorFilter(cf);

            if (cf == _mColorFilter)
            {
                return;
            }

            _mColorFilter = cf;
            _mBitmapPaint.SetColorFilter(_mColorFilter);
            Invalidate();
        }


        private Bitmap GetBitmapFromDrawable(Drawable drawable)
        {
            if (drawable == null)
            {
                return null;
            }

            var bitmapDrawable = drawable as BitmapDrawable;
            if (bitmapDrawable != null)
            {
                return bitmapDrawable.Bitmap;
            }

            try
            {
                Bitmap bitmap;

                if (drawable is ColorDrawable)
                {
                    bitmap = Bitmap.CreateBitmap(ColordrawableDimension, ColordrawableDimension, BitmapConfig);
                }
                else
                {
                    bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, BitmapConfig);
                }

                var canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
                return bitmap;
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
        }

        private void Setup()
        {
            if (!_mReady)
            {
                _mSetupPending = true;
                return;
            }

            if (Width == 0 && Height == 0)
            {
                return;
            }

            if (_mBitmap == null)
            {
                Invalidate();
                return;
            }

            _mBitmapShader = new BitmapShader(_mBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

            _mBitmapPaint.AntiAlias = true;
            _mBitmapPaint.SetShader(_mBitmapShader);

            _mBorderPaint.SetStyle(Paint.Style.Stroke);
            _mBorderPaint.AntiAlias = true;
            _mBorderPaint.Color = new Color(_mBorderColor);
            _mBorderPaint.StrokeWidth = _mBorderWidth;

            _mFillPaint.SetStyle(Paint.Style.Fill);
            _mFillPaint.AntiAlias = true;
            _mFillPaint.Color = new Color(_mFillColor);

            _mBitmapHeight = _mBitmap.Height;
            _mBitmapWidth = _mBitmap.Width;

            _mBorderRect.Set(0, 0, Width, Height);
            _mBorderRadius = Math.Min((_mBorderRect.Height() - _mBorderWidth)/2.0f,
                (_mBorderRect.Width() - _mBorderWidth)/2.0f);

            _mDrawableRect.Set(_mBorderRect);
            if (!_mBorderOverlay)
            {
                _mDrawableRect.Inset(_mBorderWidth, _mBorderWidth);
            }
            _mDrawableRadius = Math.Min(_mDrawableRect.Height()/2.0f, _mDrawableRect.Width()/2.0f);

            UpdateShaderMatrix();
            Invalidate();
        }


        private void UpdateShaderMatrix()
        {
            float scale;
            float dx = 0;
            float dy = 0;

            _mShaderMatrix.Set(null);

            if (_mBitmapWidth*_mDrawableRect.Height() > _mDrawableRect.Width()*_mBitmapHeight)
            {
                scale = _mDrawableRect.Height()/_mBitmapHeight;
                dx = (_mDrawableRect.Width() - _mBitmapWidth*scale)*0.5f;
            }
            else
            {
                scale = _mDrawableRect.Width()/_mBitmapWidth;
                dy = (_mDrawableRect.Height() - _mBitmapHeight*scale)*0.5f;
            }

            _mShaderMatrix.SetScale(scale, scale);
            _mShaderMatrix.PostTranslate((int) (dx + 0.5f) + _mDrawableRect.Left, (int) (dy + 0.5f) + _mDrawableRect.Top);

            _mBitmapShader.SetLocalMatrix(_mShaderMatrix);
        }
    }
}