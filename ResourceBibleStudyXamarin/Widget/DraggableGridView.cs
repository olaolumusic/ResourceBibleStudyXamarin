//TO DO:
//
// - improve timer performance (especially on Eee Pad)
// - improve child rearranging


using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Java.Util;
using ResourceBibleStudyXamarin.Widget.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ResourceBibleStudyXamarin.Widget
{
    public sealed class DraggableGridView : ViewGroup, View.IOnTouchListener, View.IOnClickListener, View.IOnLongClickListener
    {
        //layout vars
        public static float childRatio = .9f;
        private int mColCount;
        private int mChildSize;
        private static int _padding;
        private readonly int dpi;
        private static int _scroll = 0;
        private float mLastDelta = 0;

        private readonly Handler mHandler = new Handler();

        //dragging vars
        private static int _dragged = -1;
        private int mLastX = -1;
        private static int _lastY = -1;
        private int mLastTarget = -1;

        private bool mEnabled = true;

        private bool mTouching = false;

        //anim vars
        public static int animT = 150;

        private readonly List<int> mNewPositions = new List<int>();
        //listeners

        private IOnRearrangeListener mOnRearrangeListener;
        private IOnClickListener mSecondaryOnClickListener;
        private AdapterView.IOnItemClickListener mOnItemClickListener;

        //CONSTRUCTOR AND HELPERS
        public DraggableGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            SetListeners();


            var updateTask = new Runnable(() =>
            {
                if (_dragged != -1)
                {
                    if (_lastY < _padding * 3 && _scroll > 0)
                        _scroll -= 20;
                    else if (_lastY > -Top - (_padding * 3) && _scroll < GetMaxScroll())
                        _scroll += 20;
                }
                else if (mLastDelta != 0 && !mTouching)
                {
                    _scroll += (int)mLastDelta;

                    mLastDelta *= 0.9f;

                    if (Math.Abs(mLastDelta) < .25)
                        mLastDelta = 0;
                }
                ClampScroll();
                OnLayout(true, Left, Top, Right, Bottom);


            });
            mHandler.PostDelayed(updateTask, 25);



            mHandler.RemoveCallbacks(updateTask);
            mHandler.PostAtTime(updateTask, SystemClock.UptimeMillis() + 500);

            ChildrenDrawingOrderEnabled = true;

            var metrics = new DisplayMetrics();
            ((Activity)context).WindowManager.DefaultDisplay.GetMetrics(metrics);
            dpi = (int)metrics.DensityDpi;
        }



        private void SetListeners()
        {
            SetOnTouchListener(this);
            base.SetOnClickListener(this);
            SetOnLongClickListener(this);
        }

        public override void SetOnClickListener(IOnClickListener l)
        {
            mSecondaryOnClickListener = l;
        }



        public override void AddView(View child)
        {
            base.AddView(child);
            mNewPositions.Add(-1);
        }

        public override void RemoveViewAt(int index)
        {
            base.RemoveViewAt(index);
            mNewPositions.Remove(index);
        }

        //LAYOUT 
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            //compute width of view, in dp
            var w = (r - l) / (dpi / 160f);

            //determine number of columns, at least 2
            mColCount = 2;
            var sub = 240;
            w -= 280;
            while (w > 0)
            {
                mColCount++;
                w -= sub;
                sub += 40;
            }

            //determine childSize and padding, in px
            mChildSize = (r - l) / mColCount;
            mChildSize = Math.Round(mChildSize * childRatio);
            _padding = ((r - l) - (mChildSize * mColCount)) / (mColCount + 1);

            for (var i = 0; i < ChildCount; i++)
                if (i != _dragged)
                {
                    var xy = GetCoordinateFromIndex(i);
                    GetChildAt(i).Layout(xy.X, xy.Y, xy.X + mChildSize, xy.Y + mChildSize);
                }
        }
        protected override int GetChildDrawingOrder(int childCount, int i)
        {
            if (_dragged == -1)
                return i;
            if (i == childCount - 1)
                return _dragged;
            if (i >= _dragged)
                return i + 1;
            return i;
        }
        public int GetIndexFromCoor(int x, int y)
        {
            int col = GetColOrRowFromCoordinate(x), row = GetColOrRowFromCoordinate(y + _scroll);
            if (col == -1 || row == -1) //touch is between columns or rows
                return -1;
            var index = row * mColCount + col;
            if (index >= ChildCount)
                return -1;
            return index;
        }

        private int GetColOrRowFromCoordinate(int coordinate)
        {
            coordinate -= _padding;
            for (var i = 0; coordinate > 0; i++)
            {
                if (coordinate < mChildSize)
                    return i;
                coordinate -= (mChildSize + _padding);
            }
            return -1;
        }

        private int GetTargetFromColor(int x, int y)
        {
            if (GetColOrRowFromCoordinate(y + _scroll) == -1) //touch is between rows
                return -1;

            var leftPos = GetIndexFromCoor(x - (mChildSize / 4), y);
            var rightPos = GetIndexFromCoor(x + (mChildSize / 4), y);
            if (leftPos == -1 && rightPos == -1) //touch is in the middle of nowhere
                return -1;
            if (leftPos == rightPos) //touch is in the middle of a visual
                return -1;

            var target = -1;
            if (rightPos > -1)
                target = rightPos;
            else if (leftPos > -1)
                target = leftPos + 1;
            if (_dragged < target)
                return target - 1;

            //Toast.makeText(getContext(), "Target: " + target + ".", Toast.LENGTH_SHORT).show();
            return target;
        }

        private Point GetCoordinateFromIndex(int index)
        {
            var col = index % mColCount;
            var row = index / mColCount;
            return new Point(_padding + (mChildSize + _padding) * col,
                _padding + (mChildSize + _padding) * row - _scroll);
        }
        public int IndexOf(View child)
        {
            for (var i = 0; i < ChildCount; i++)
                if (GetChildAt(i) == child)
                    return i;
            return -1;
        }

        //EVENT HANDLERS
        public void OnClick(View view)
        {
            if (mEnabled)
            {
                if (mSecondaryOnClickListener != null)
                    mSecondaryOnClickListener.OnClick(view);
                if (mOnItemClickListener != null && GetLastIndex() != -1)
                    mOnItemClickListener.OnItemClick(null, GetChildAt(GetLastIndex()), GetLastIndex(), GetLastIndex() / mColCount);
            }
        }
        public bool OnLongClick(View view)
        {
            if (!mEnabled)
                return false;
            var index = GetLastIndex();
            if (index != -1)
            {
                _dragged = index;
                AnimateDragged();
                return true;
            }
            return false;
        }

        public bool OnTouch(View view, MotionEvent motionEvent)
        {
            var action = motionEvent.Action;

            switch (action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    mEnabled = true;
                    mLastX = (int)motionEvent.GetX();
                    _lastY = (int)motionEvent.GetY();
                    mTouching = true;
                    break;

                case MotionEventActions.Move:
                    var delta = _lastY - (int)motionEvent.GetY();
                    if (_dragged != -1)
                    {
                        //change draw location of dragged visual
                        int x = (int)motionEvent.GetX(), y = (int)motionEvent.GetY();
                        int l = x - (3 * mChildSize / 4), t = y - (3 * mChildSize / 4);
                        GetChildAt(_dragged).Layout(l, t, l + (mChildSize * 3 / 2), t + (mChildSize * 3 / 2));

                        //check for new target hover
                        var target = GetTargetFromColor(x, y);
                        if (mLastTarget != target)
                        {
                            if (target != -1)
                            {
                                AnimateGap(target);
                                mLastTarget = target;
                            }
                        }
                    }
                    else
                    {
                        _scroll += delta;
                        ClampScroll();
                        if (Math.Abs(delta) > 2)
                            mEnabled = false;
                        OnLayout(true, Left, Top, Right, Bottom);
                    }
                    mLastX = (int)motionEvent.GetX();
                    _lastY = (int)motionEvent.GetY();
                    mLastDelta = delta;
                    break;

                case MotionEventActions.Up:
                    if (_dragged != -1)
                    {
                        var v = GetChildAt(_dragged);
                        if (mLastTarget != -1)
                            ReorderChildren();
                        else
                        {
                            var xy = GetCoordinateFromIndex(_dragged);
                            v.Layout(xy.X, xy.Y, xy.X + mChildSize, xy.Y + mChildSize);
                        }
                        v.ClearAnimation();
                        if (v.GetType() == typeof(ImageView))
                            ((ImageView)v).Alpha = 255;
                        mLastTarget = -1;
                        _dragged = -1;
                    }
                    mTouching = false;
                    break;
            }
            return _dragged != -1;
        }

        //EVENT HELPERS
        private void AnimateDragged()
        {
            var v = GetChildAt(_dragged);
            int x = GetCoordinateFromIndex(_dragged).X + mChildSize / 2, y = GetCoordinateFromIndex(_dragged).Y + mChildSize / 2;
            int l = x - (3 * mChildSize / 4), t = y - (3 * mChildSize / 4);
            v.Layout(l, t, l + (mChildSize * 3 / 2), t + (mChildSize * 3 / 2));
            var animSet = new AnimationSet(true);
            var scale = new ScaleAnimation(.667f, 1, .667f, 1, mChildSize * 3 / 4, mChildSize * 3 / 4) { Duration = animT };
            var alpha = new AlphaAnimation(1, .5f) { Duration = animT };

            animSet.AddAnimation(scale);
            animSet.AddAnimation(alpha);
            animSet.FillEnabled = true;
            animSet.FillAfter = true;

            v.ClearAnimation();
            v.StartAnimation(animSet);
        }

        private void AnimateGap(int target)
        {
            for (var i = 0; i < ChildCount; i++)
            {
                var v = GetChildAt(i);
                if (i == _dragged)
                    continue;
                var newPos = i;
                if (_dragged < target && i >= _dragged + 1 && i <= target)
                    newPos--;
                else if (target < _dragged && i >= target && i < _dragged)
                    newPos++;

                //animate
                var oldPos = i;
                if (mNewPositions[i] != -1)
                    oldPos = mNewPositions[i];
                if (oldPos == newPos)
                    continue;

                var oldXy = GetCoordinateFromIndex(oldPos);
                var newXy = GetCoordinateFromIndex(newPos);
                var oldOffset = new Point(oldXy.X - v.Left, oldXy.Y - v.Top);
                var newOffset = new Point(newXy.X - v.Left, newXy.Y - v.Top);

                var translate = new TranslateAnimation(Dimension.Absolute, oldOffset.X,
                        Dimension.Absolute, newOffset.X,
                        Dimension.Absolute, oldOffset.Y,
                        Dimension.Absolute, newOffset.Y)
                { Duration = animT, FillEnabled = true, FillAfter = true };
                v.ClearAnimation();
                v.StartAnimation(translate);

                mNewPositions.Insert(i, newPos);
            }
        }

        private void ReorderChildren()
        {
            //FIGURE OUT HOW TO REORDER CHILDREN WITHOUT REMOVING THEM ALL AND RECONSTRUCTING THE LIST!!!
            mOnRearrangeListener?.OnRearrange(_dragged, mLastTarget);

            var children = new List<View>();
            for (var i = 0; i < ChildCount; i++)
            {
                GetChildAt(i).ClearAnimation();
                children.Add(GetChildAt(i));
            }
            RemoveAllViews();
            while (_dragged != mLastTarget)
                if (mLastTarget == children.Count) // dragged and dropped to the right of the last element
                {
                    children.Remove(children.ElementAt(_dragged));

                    _dragged = mLastTarget;
                }
                else if (_dragged < mLastTarget) // shift to the right
                {
                    Collections.Swap(new object[] { children }, _dragged, _dragged + 1);
                    _dragged++;
                }
                else if (_dragged > mLastTarget) // shift to the left
                {
                    Collections.Swap(new object[] { children }, _dragged, _dragged - 1);
                    _dragged--;
                }
            for (var i = 0; i < children.Count; i++)
            {
                mNewPositions.Insert(i, -1);
                AddView(children[i]);
            }
            OnLayout(true, Left, Top, Right, Bottom);
        }
        public void ScrollToTop()
        {
            _scroll = 0;
        }
        public void ScrollToBottom()
        {
            _scroll = Integer.MaxValue;
            ClampScroll();
        }

        private void ClampScroll()
        {
            int stretch = 3, overreach = Height / 2;
            var max = GetMaxScroll();
            max = Math.Max(max, 0);

            if (_scroll < -overreach)
            {
                _scroll = -overreach;
                mLastDelta = 0;
            }
            else if (_scroll > max + overreach)
            {
                _scroll = max + overreach;
                mLastDelta = 0;
            }
            else if (_scroll < 0)
            {
                if (_scroll >= -stretch)
                    _scroll = 0;
                else if (!mTouching)
                    _scroll -= _scroll / stretch;
            }
            else if (_scroll > max)
            {
                if (_scroll <= max + stretch)
                    _scroll = max;
                else if (!mTouching)
                    _scroll += (max - _scroll) / stretch;
            }
        }

        private int GetMaxScroll()
        {
            int rowCount = (int)Math.Ceil((double)ChildCount / mColCount), max = rowCount * mChildSize + (rowCount + 1) * _padding - Height;
            return max;
        }
        public int GetLastIndex()
        {
            return GetIndexFromCoor(mLastX, _lastY);
        }

        //OTHER METHODS
        public void SetOnRearrangeListener(IOnRearrangeListener l)
        {
            this.mOnRearrangeListener = l;
        }
        public void SetOnItemClickListener(AdapterView.IOnItemClickListener l)
        {
            this.mOnItemClickListener = l;
        }
    }
}