using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;
using UXDivers.Grial;

namespace Eleos3
{
    /// <summary>
    /// This extension provides options for progressive animations on top of Xamarin Forms
    /// CarouselView by exposing ScrollProgress, CurrentAnimatedItem and NextAnimatedItem properties.
    /// https://docs.grialkit.com/progressive-animations.html
    /// </summary>
    public class ExtendedCarouselView : CarouselView
    {
        private static readonly BindablePropertyKey ScrollProgressPropertyKey =
            BindableProperty.CreateReadOnly(nameof(ScrollProgress), typeof(double), typeof(ExtendedCarouselView), 0.0);

        public static readonly BindableProperty ScrollProgressProperty = ScrollProgressPropertyKey.BindableProperty;

        public double ScrollProgress
        {
            get => (double)GetValue(ScrollProgressProperty);
            private set => SetValue(ScrollProgressPropertyKey, value);
        }

        private static readonly BindablePropertyKey NextAnimatedItemPropertyKey =
            BindableProperty.CreateReadOnly(nameof(NextAnimatedItem), typeof(object), typeof(ExtendedCarouselView), null);

        public static readonly BindableProperty NextAnimatedItemProperty = NextAnimatedItemPropertyKey.BindableProperty;

        public object NextAnimatedItem
        {
            get => GetValue(NextAnimatedItemProperty); 
            private set => SetValue(NextAnimatedItemPropertyKey, value);
        }

        private static readonly BindablePropertyKey CurrentAnimatedItemPropertyKey =
            BindableProperty.CreateReadOnly(nameof(CurrentAnimatedItem), typeof(object), typeof(ExtendedCarouselView), null);

        public static readonly BindableProperty CurrentAnimatedItemProperty = CurrentAnimatedItemPropertyKey.BindableProperty;

        public object CurrentAnimatedItem
        {
            get => GetValue(CurrentAnimatedItemProperty);
            private set => SetValue(CurrentAnimatedItemPropertyKey, value);
        }

        private INotifyCollectionChanged _itemsSourceAsObservable;
        private IList _itemsSourceAsList;
        private DraggingPosition _draggingPosition;
        private int _itemsCount;
        private int _currentItemIndex;
        private int _nextItemIndex;
        private bool _isHorizontal;
        private bool _isScrolling;
        private double _itemSize;
        private double _startScrollOffset;
        private double _currentScrollOffset;
        private bool _ignoreNextScrollCall;

        public ExtendedCarouselView()
        {
            Loop = false;
        }

        private enum DraggingPosition
        {
            Start,
            BeforeStart,
            AfterStart
        }

        private double AvailableScrollOffset => _itemSize * (_itemsCount - 1);

        private bool IsOutOfBounds => _currentScrollOffset < 0 || _currentScrollOffset > AvailableScrollOffset;

        public void AndroidScrollEnd()
        {
            if (Position == _nextItemIndex)
            {
                HandleScrollEnd();
            }
        }

        public void HandleScrollChange(double offset)
        {
            _isScrolling = true;
            _currentScrollOffset = offset;

            if (!IsOutOfBounds)
            {
                UpdateAnimatedItemsOnScroll();

                UpdateScrollProgress();

                // In Android some offsets are lost so the scroll end is notified
                // explicitly from the ExtendedCarouselViewRenderer custom renderer
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    if (_draggingPosition == DraggingPosition.Start ||
                        _currentScrollOffset >= _startScrollOffset + _itemSize ||
                        _currentScrollOffset <= _startScrollOffset - _itemSize)
                    {
                        HandleScrollEnd();
                    }
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ItemsSource))
            {
                OnItemsSourceChanged();
            }
            else if (propertyName == nameof(ItemsLayout))
            {
                UpdateOrientationAndSize();
            }
        }

        protected override void OnPositionChanged(PositionChangedEventArgs args)
        {
            base.OnPositionChanged(args);

            if (!_isScrolling)
            {
                PositionReset();
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            PositionReset();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateOrientationAndSize();
        }

        protected override void OnScrolled(ItemsViewScrolledEventArgs e)
        {
            base.OnScrolled(e);

            if (_ignoreNextScrollCall)
            {
                _ignoreNextScrollCall = false;
                return;
            }

            // In iOS we can use the offset received here but in Android
            // the value it's not always correct and some values are missing.
            // This is why in Android, the scroll is handled through the
            // ExtendedCarouselViewRenderer custom renderer and not from here
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                HandleScrollChange(_isHorizontal ? e.HorizontalOffset : e.VerticalOffset);
            }
        }

        private void UpdateAnimatedItemsOnScroll()
        {
            var offsetDiff = _currentScrollOffset - _startScrollOffset;

            var newCurrentIndex = (int)(_startScrollOffset / _itemSize);
            newCurrentIndex += (int)(offsetDiff / _itemSize);
            newCurrentIndex = Math.Max(0, Math.Min(newCurrentIndex, _itemsCount - 1));

            offsetDiff = _currentScrollOffset - (newCurrentIndex * _itemSize);

            _draggingPosition =
                offsetDiff < 0 ? DraggingPosition.BeforeStart :
                offsetDiff > 0 ? DraggingPosition.AfterStart :
                DraggingPosition.Start;

            var newNextCurrentIndex = newCurrentIndex;
            if (_draggingPosition == DraggingPosition.BeforeStart)
            {
                newNextCurrentIndex = Math.Max(newCurrentIndex - 1, 0);
            }
            else if (_draggingPosition == DraggingPosition.AfterStart)
            {
                newNextCurrentIndex = Math.Min(newCurrentIndex + 1, _itemsCount - 1);
            }

            if (newCurrentIndex != _currentItemIndex || newNextCurrentIndex != _nextItemIndex)
            {
                _currentItemIndex = newCurrentIndex;
                _nextItemIndex = newNextCurrentIndex;
                SetAnimatedItems();
            }
        }

        private void UpdateScrollProgress()
        {
            // Position changes its value when the scroll reaches half the trajectory between items
            var progress = (_currentScrollOffset - (_itemSize * Position)) / _itemSize;

            if (_draggingPosition == DraggingPosition.BeforeStart)
            {
                // BeforeStart progress is always negative
                if (progress > 0)
                {
                    progress -= 1;
                }
            }
            else if (_draggingPosition == DraggingPosition.AfterStart)
            {
                // AfterStart progress is always positive
                if (progress < 0)
                {
                    progress += 1;
                }
            }

            ScrollProgress = progress;
        }

        private void HandleScrollEnd()
        {
            _currentScrollOffset = 0;
            _startScrollOffset = _itemSize * Position;
            _isScrolling = false;
            ScrollProgress = 0;
            _nextItemIndex = _currentItemIndex;
            _draggingPosition = DraggingPosition.Start;

            SetAnimatedItems();
        }

        private void UpdateOrientationAndSize()
        {
            // iOS renderer generates a scroll that correspond to the resize but not to an actual event
            // we need to ignore it to prevent wrong offset calculations
            _ignoreNextScrollCall = true;

            if (ItemsLayout != null)
            {
                _isHorizontal = ItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal;
            }

            if (Width != -1 && Height != -1)
            {
                _itemSize = _isHorizontal ?
                    Width - PeekAreaInsets.HorizontalThickness :
                    Height - PeekAreaInsets.VerticalThickness;

                _startScrollOffset = _currentScrollOffset = _itemSize * Position;
            }
        }

        private void PositionReset()
        {
            _currentItemIndex = _nextItemIndex = Position;
            _startScrollOffset = _itemSize * Position;
            SetAnimatedItems();
        }

        private void SetAnimatedItems()
        {
            object current = null, next = null;

            if (_itemsSourceAsList != null)
            {
                if (_currentItemIndex >= 0 && _itemsSourceAsList.Count > _currentItemIndex)
                {
                    current = _itemsSourceAsList[_currentItemIndex];
                }

                if (_nextItemIndex >= 0 && _itemsSourceAsList.Count > _nextItemIndex)
                {
                    next = _itemsSourceAsList[_nextItemIndex];
                }
            }
            else if (ItemsSource != null)
            {
                int i = 0;
                foreach (var item in ItemsSource)
                {
                    if (_currentItemIndex == i)
                    {
                        current = item;
                    }

                    if (_nextItemIndex == i)
                    {
                        next = item;
                    }

                    if (current != null && next != null)
                    {
                        break;
                    }

                    i++;
                }
            }

            CurrentAnimatedItem = current;
            NextAnimatedItem = next;
        }

        private void OnItemsSourceChanged()
        {
            _itemsSourceAsList = null;
            if (_itemsSourceAsObservable != null)
            {
                _itemsSourceAsObservable.CollectionChanged -= OnItemsSourceCollectionChanged;
                _itemsSourceAsObservable = null;
            }

            if (ItemsSource != null)
            {
                if (ItemsSource is ICollection collection)
                {
                    _itemsCount = collection.Count;
                    _itemsSourceAsList = collection as IList;
                    if (collection is INotifyCollectionChanged observableCollection)
                    {
                        _itemsSourceAsObservable = observableCollection;
                        _itemsSourceAsObservable.CollectionChanged += OnItemsSourceCollectionChanged;
                    }
                }
                else
                {
                    _itemsCount = 0;
                    foreach (var item in ItemsSource)
                    {
                        _itemsCount++;
                    }
                }

                SetAnimatedItems();
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _itemsCount = ((ICollection)_itemsSourceAsObservable).Count;
        }
    }
}
