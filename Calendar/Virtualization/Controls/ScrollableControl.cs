using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Virtualization.Controls
{
    public abstract class ScrollableControl: Control, IScrollInfo
    {
        #region Private fields
        private Size itemSize;

        #endregion

        #region Construction / Desctruction
        protected ScrollableControl()
        {
            this.itemSize = new Size(1, 1);
        }

        #endregion

        #region Public properties
        public bool CanVerticallyScroll { get; set; }

        public bool CanHorizontallyScroll { get; set; }

        public double ExtentWidth { get; private set; }

        public double ExtentHeight { get; private set; }

        public double ViewportWidth { get; private set; }

        public double ViewportHeight { get; private set; }

        public double HorizontalOffset { get; private set; }

        public double VerticalOffset { get; private set; }

        public ScrollViewer ScrollOwner { get; set; }

        #endregion

        #region Public methods

        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.itemSize.Height);
        }

        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.itemSize.Height);
        }

        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.itemSize.Width);
        }

        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.itemSize.Width);
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        }

        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
        }

        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
        }

        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - (this.itemSize.Height * 3));
        }

        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + (this.itemSize.Height * 3));
        }

        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - (this.itemSize.Width * 3));
        }

        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - (this.itemSize.Width * 3));
        }

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || this.ExtentWidth <= this.ViewportWidth)
                offset = 0;
            else if (offset >= this.ExtentWidth - this.ViewportWidth)
                offset = this.ExtentWidth - this.ViewportWidth;
            //else if (offset % this.itemSize.Width != 0)
            //{
            //    var d = (int)(offset / this.itemSize.Width);
            //    offset = d * this.itemSize.Width;
            //}
            this.HorizontalOffset = offset;
            this.UpdateScrollOwner();
            this.OnScroll();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || this.ExtentHeight <= this.ViewportHeight)
                offset = 0;
            else if (offset >= this.ExtentHeight - this.ViewportHeight)
                offset = this.ExtentHeight - this.ViewportHeight;
            //делаем перерасчет
            //else if (offset % this.itemSize.Height != 0)
            //{
            //    var d = (int)(offset / this.itemSize.Height);
            //    offset = d * this.itemSize.Height;
            //}
            this.VerticalOffset = offset;
            this.UpdateScrollOwner();
            this.OnScroll();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }

        #endregion

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (sizeInfo.WidthChanged)
                this.SetHorizontalOffset(this.HorizontalOffset);

            if (sizeInfo.HeightChanged)
                this.SetVerticalOffset(this.VerticalOffset);

            this.ViewportWidth = this.ActualWidth;
            this.ViewportHeight = this.ActualHeight;

            this.UpdateScrollOwner();
            this.OnScroll();
        }

        protected void SetScrollExtent(double itemWidth, double itemHeight, double extentWidth, double extentHeight)
        {
            this.itemSize = new Size(itemWidth, itemHeight);

            this.ExtentWidth = extentWidth;
            this.ExtentHeight = extentHeight;
            this.ViewportWidth = this.ActualWidth;
            this.ViewportHeight = this.ActualHeight;

            this.UpdateScrollOwner();
            this.OnScroll();
        }

        protected virtual void OnScroll() { }

        private void UpdateScrollOwner()
        {
            this.CanHorizontallyScroll = this.ViewportWidth < this.ExtentWidth;
            this.CanVerticallyScroll = this.ViewportHeight < this.ExtentHeight;

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();
        }
    }
}
