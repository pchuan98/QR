using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace QR.WPF.Attachs;


public class WindowElement
{
    #region 金刚键功能
    // ! 这个属性不包含关闭
    public static readonly DependencyProperty TitleBarButtonStateProperty = DependencyProperty.RegisterAttached(
           "TitleBarButtonState", typeof(WindowState?), typeof(WindowElement),
           new PropertyMetadata(null, OnButtonStateChanged));
    private static void OnButtonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var button = (Button)d;

        if (e.OldValue is WindowState)
        {
            button.Click -= StateButton_Click;

        }

        if (e.NewValue is WindowState)
        {
            button.Click -= StateButton_Click;
            button.Click += StateButton_Click;
        }
    }

    private static void StateButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (DependencyObject)sender;
        var window = Window.GetWindow(button);
        var state = GetTitleBarButtonState(button);
        if (window != null && state != null)
        {
            window.WindowState = state.Value;
        }
    }

    public static WindowState? GetTitleBarButtonState(DependencyObject element)
        => (WindowState?)element.GetValue(TitleBarButtonStateProperty);

    public static void SetTitleBarButtonState(DependencyObject element, WindowState? value)
        => element.SetValue(TitleBarButtonStateProperty, value);

    // ! 关闭按钮
    public static readonly DependencyProperty IsTitleBarCloseButtonProperty = DependencyProperty.RegisterAttached(
           "IsTitleBarCloseButton", typeof(bool), typeof(WindowElement),
           new PropertyMetadata(false, OnIsCloseButtonChanged));

    private static void OnIsCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var button = (Button)d;

        if (e.OldValue is true)
        {
            button.Click -= CloseButton_Click;
        }

        if (e.NewValue is true)
        {
            button.Click -= CloseButton_Click;
            button.Click += CloseButton_Click;
        }
    }

    private static void CloseButton_Click(object sender, RoutedEventArgs e)
        => Window.GetWindow((DependencyObject)sender)?.Close();


    public static bool GetIsTitleBarCloseButton(DependencyObject element)
        => (bool)element.GetValue(IsTitleBarCloseButtonProperty);

    public static void SetIsTitleBarCloseButton(DependencyObject element, bool value)
        => element.SetValue(IsTitleBarCloseButtonProperty, value);
    #endregion

    #region 按钮的显示效果
    public static Visibility GetMinButtonVisibility(DependencyObject obj)
    {
        return (Visibility)obj.GetValue(MinButtonVisibilityProperty);
    }

    public static void SetMinButtonVisibility(DependencyObject obj, Visibility value)
    {
        obj.SetValue(MinButtonVisibilityProperty, value);
    }

    // Using a DependencyProperty as the backing store for MinButtonVisibility.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MinButtonVisibilityProperty =
        DependencyProperty.RegisterAttached("MinButtonVisibility", typeof(Visibility), typeof(WindowElement), new PropertyMetadata(Visibility.Visible));



    public static Visibility GetMaxButtonVisibility(DependencyObject obj)
    {
        return (Visibility)obj.GetValue(MaxButtonVisibilityProperty);
    }

    public static void SetMaxButtonVisibility(DependencyObject obj, Visibility value)
    {
        obj.SetValue(MaxButtonVisibilityProperty, value);
    }

    public static readonly DependencyProperty MaxButtonVisibilityProperty =
        DependencyProperty.RegisterAttached("MaxButtonVisibility", typeof(Visibility), typeof(WindowElement), new PropertyMetadata(Visibility.Visible));



    public static Visibility GetCloseButtonVisibility(DependencyObject obj)
    {
        return (Visibility)obj.GetValue(CloseButtonVisibilityProperty);
    }

    public static void SetCloseButtonVisibility(DependencyObject obj, Visibility value)
    {
        obj.SetValue(CloseButtonVisibilityProperty, value);
    }

    public static readonly DependencyProperty CloseButtonVisibilityProperty =
        DependencyProperty.RegisterAttached("CloseButtonVisibility", typeof(Visibility), typeof(WindowElement), new PropertyMetadata(Visibility.Visible));


    public static Visibility GetResotreVisibility(DependencyObject obj)
    {
        return (Visibility)obj.GetValue(ResotreVisibilityProperty);
    }

    public static void SetResotreVisibility(DependencyObject obj, Visibility value)
    {
        obj.SetValue(ResotreVisibilityProperty, value);
    }

    public static readonly DependencyProperty ResotreVisibilityProperty =
        DependencyProperty.RegisterAttached("ResotreVisibility", typeof(Visibility), typeof(WindowElement), new PropertyMetadata(Visibility.Visible));



    #endregion
}